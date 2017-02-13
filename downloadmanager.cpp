#include "downloadmanager.h"

#include <QThread>
#include <QTimer>

#include <QJsonDocument>
#include <QJsonObject>
#include <QJsonArray>

#include <QDir>

DownloadManager::DownloadManager(QNetworkAccessManager* netManager, QObject* parent) : QObject(parent), imagesToBuffer(8000), delayBetweenImages(200), delayBetweenMetadata(200), delayWaitingForImages(200), delayWhilePaused(500),
	metaDownloader(netManager), imageDownloader(netManager), searchSettings("*"), timeHelper(10, TimeHelper::Difference)
{
	//Connect downloaders to their result slots
	connect(&metaDownloader, SIGNAL(finished()), this, SLOT(getMetadataResults()));
	connect(&imageDownloader, SIGNAL(finished()), this, SLOT(getImageResults()));
	
	//Forward downloadProgress of the image downloader to track progress of individual images.
	connect(&imageDownloader, SIGNAL(downloadProgress(qint64,qint64)), this, SIGNAL(currentDownloadProgress(qint64,qint64)));
	
	//Forward error signals
	connect(&imageDownloader, SIGNAL(fileError(int,QString,QFile*)), this, SIGNAL(fileError(int,QString,QFile*)));
	connect(&imageDownloader, SIGNAL(networkError(int,QString,QUrl)), this, SIGNAL(networkError(int,QString,QUrl)));
	connect(&metaDownloader, SIGNAL(fileError(int,QString,QFile*)), this, SIGNAL(fileError(int,QString,QFile*)));
	connect(&metaDownloader, SIGNAL(networkError(int,QString,QUrl)), this, SIGNAL(networkError(int,QString,QUrl)));
}

//TODO possibly fix error reporting to have the manager emit instead of just forwarding signals from the downloaders.
void DownloadManager::start(DerpiJson::SearchSettings searchSettings, QString imageFileNameFormat, int maxImages, bool saveJson, bool updateJson, QString jsonFileNameFormat)
{
	this->searchSettings = searchSettings;
	this->imageFileNameFormat = imageFileNameFormat;
	this->jsonFileNameFormat = jsonFileNameFormat;
	this->maxImagesToDownload = maxImages;
	this->saveJson = saveJson;
	this->updateJson = updateJson;
	
	//Reset downloading variables
	stoppingDownload = false;
	imageDownloaderStopped = false;
	downloadPaused = false;
	firstMetadata = true;
	metadataWaiting = false;
	imageWaiting = false;
	noMoreImages = false;
	metadataAttempts = 0;
	imageAttempts = 0;
	imagesQueued = 0;
	imagesDownloaded = 0;
	imagesTotal = 0;
	timeHelper.restart();
	
	//QTimer is used as a way to call a slot after a set amount of time. This allows control to return to the event loop, and prevents the gui from locking.
	//Slots will be called after all other tasks in the event loop are done.
	QTimer::singleShot(0, this, SLOT(getMetadata()));
	QTimer::singleShot(1000, this, SLOT(getImages()));
	QTimer::singleShot(1000, this, SLOT(calculateTiming()));
}

void DownloadManager::calculateTiming()
{
	//Check if stopping
	if(stoppingDownload && imageDownloaderStopped)
	{
		//TODO cleanup with signals that could be emitted?
		return;
	}
	
	timeHelper.update(imagesDownloaded);
	if(timeHelper.hasReading(5))
	{
		emit timingUpdate(timeHelper.getElapsedTimeFormatted(), timeHelper.getETAFormatted(imagesTotal - imagesDownloaded), timeHelper.getPerMinute());
	}
	else
	{
		emit timingUpdate(timeHelper.getElapsedTimeFormatted(), "---", -1);
	}
	
	//Trigger every second
	QTimer::singleShot(1000, this, SLOT(calculateTiming()));
}

/*
 * Handles queuing metadata downloads //and keeping the image function supplied with images to download
 * 
 */
void DownloadManager::getMetadata()
{
	qDebug() << "Get Metadata called";
	//Check if stopping
	if(stoppingDownload && imageDownloaderStopped)
	{
		//Clear the queued list and delete the objects.
		while(!queuedImages.isEmpty())
		{
			delete queuedImages.at(0);
			queuedImages.remove(0);
		}
		emit queueSizeChanged(queuedImages.size());
		emit finished();
		return;
	}
	
	//Check if finished. Function will continue to be called to allow it to stop properly if needed.
	if(noMoreImages)
	{
		QTimer::singleShot(200, this, SLOT(getMetadata()));
		return;
	}
	
	//Check if paused
	if(downloadPaused)
	{
		QTimer::singleShot(delayWhilePaused, this, SLOT(getMetadata()));
		return;
	}
	
	//Check if waiting due to a network error
	if(metadataWaiting)
	{
		emit metadataTimeoutRemaining(metadataTimeout);
		
		//If there is time left to wait
		if(metadataTimeout > 0)
		{
			QTimer::singleShot(1000, this, SLOT(getMetadata()));
			metadataTimeout--;
			return;
		}
		else
		{
			//Stop waiting and try again
			metadataWaiting = false;
		}
	}
	
	//Check if there are enough images already
	if(queuedImages.size() >= imagesToBuffer)
	{
		QTimer::singleShot(delayBetweenMetadata, this, SLOT(getMetadata()));
		return;
	}
	
	//Download more if needed
	//This will automatically call getMetadataResults when it is finished.
	QUrl searchUrl = DerpiJson::getSearchUrl(searchSettings);
	metaDownloader.download(searchUrl);
}

void DownloadManager::getMetadataResults()
{
	//Check if there was an error
	if(metaDownloader.hasError())
	{
		//If it was a file error
		if(metaDownloader.getErrorType() == Downloader::FileError)
		{
			//Stop the download
			stoppingDownload = true;
			return;
		}
		else //If it was a network error
		{
			metadataAttempts++;
			metadataTimeout = expDelay(1, metadataAttempts, 32);
			metadataWaiting = true;
			QTimer::singleShot(0, this, SLOT(getMetadata()));
			return;
		}
	}
	
	//Reset attempts after a successful download
	metadataAttempts = 0;
	
	//Get downloaded data and convert it to a QJsonDocument.
	QJsonDocument json = QJsonDocument::fromJson(metaDownloader.getData());
	
	//If this is the first metadata download, set the imagesTotal variable used to report progress.
	if(firstMetadata)
	{
		//Calculate the total number of images possible to download if it were not limited.
		int total = json.object()["total"].toInt();
		int imagesSkipped = (searchSettings.page - 1) * searchSettings.perPage;		//If the page were set to something other than 1 for the first download, those images aren't counted.
		int totalPossible = total - imagesSkipped;
		
		//If no limit was set to how many images should be downloaded
		if(maxImagesToDownload == -1)
		{
			//Then the limit is the most possible
			imagesTotal = totalPossible;
		}
		else
		{
			//maxImagesToDowload will be the total number of images downloaded, unless it isn't possible to get that many.
			if(totalPossible < maxImagesToDownload)
			{
				imagesTotal = totalPossible;
			}
			else
			{
				imagesTotal = maxImagesToDownload;
			}
		}
		firstMetadata = false;
	}
	
	//Get the QJsonArray "search" and pass it to DerpiJson::splitArray to get a QVector of the image json files.
	QVector<DerpiJson*> newImages = DerpiJson::splitArray(json.object()["search"].toArray());
	
	//If the vector is empty, set noMoreImages to true and return
	if(newImages.size() == 0)
	{
		noMoreImages = true;
		QTimer::singleShot(0, this, SLOT(getMetadata()));
		return;
	}
	
	//Create a temporary vector to store all the ids on the current page.
	QVector<int> newIds;
	
	//Loop over every image in the vector
	for(int i = 0; i < newImages.size(); i++)
	{
		//Check every image and see if its id is in lastPageIds. If so, delete the object and remove it from the vector.
		int id = newImages.at(i)->getId();
		newIds.append(id);
		if(lastPageIds.contains(id))
		{
			delete newImages.at(i);
			newImages.remove(i);
			i--;
			continue;
		}
		
		//Check if more images should be added.
		//If there is a limit to how many images should be downloaded
		if(maxImagesToDownload != -1)
		{
			//And if no more images should be added
			if(maxImagesToDownload - imagesQueued == 0)
			{
				//Let the image downloader know that no more images are going to be added.
				noMoreImages = true;
				
				//Delete the image and continue. At this point, all images will just get deleted.
				delete newImages.at(i);
				newImages.remove(i);
				i--;
				continue;
			}
		}
		
		//If neither of those applied, then add the image to the queue and increment imagesQueued
		queuedImages.append(newImages.at(i));
		imagesQueued++;
	}
	
	emit queueSizeChanged(queuedImages.size());
	
	//Increment page by 1 in the searchSettings
	searchSettings.page++;
	
	//Use Qtimer to queue calling getMetadata after a delay of delayBetweenMetadata
	QTimer::singleShot(delayBetweenMetadata, this, SLOT(getMetadata()));
}

/*
 * Handles queueing images to download
 * 
 */
void DownloadManager::getImages()
{
	//Emit the total progress of the download session
	emit totalDownloadProgress(imagesDownloaded, imagesTotal);
	
	//Check if stopping
	if(stoppingDownload)
	{
		imageDownloaderStopped = true;
		return;
	}
	
	//Check if paused
	if(downloadPaused)
	{
		QTimer::singleShot(delayWhilePaused, this, SLOT(getImages()));
		return;
	}
	
	//Check if waiting due to a network error
	if(imageWaiting)
	{
		emit imageTimeoutRemaining(imageTimeout);
		
		//If there is time left to wait
		if(imageTimeout > 0)
		{
			QTimer::singleShot(1000, this, SLOT(getImages()));
			imageTimeout--;
			return;
		}
		else
		{
			//Stop waiting and try again
			imageWaiting = false;
		}
	}
	
	//If there are no images in the queue
	if(queuedImages.isEmpty())
	{
		//If there are no more images left to get
		if(noMoreImages)
		{
			//The session is complete
			stoppingDownload = true;
			imageDownloaderStopped = true;
			return;
		}
		else
		{
			//Wait for more images
			QTimer::singleShot(delayWaitingForImages, this, SLOT(getImages()));
			return;
		}
	}
	
	//Get the url and download location for the image
	QUrl imageUrl = queuedImages.at(0)->getImageUrl();
	QString filePath = parseFormat(imageFileNameFormat, queuedImages.at(0));
	
	QFile* imageFile = new QFile(filePath);
	
	//If the image already exists, skip it
	if(imageFile->exists())
	{
		//If set to save json, do so now.
		if(saveJson)
		{
			saveJsonToFile();
		}
		
		imagesDownloaded++;
		emit downloaded(queuedImages.at(0)->getId());
		delete queuedImages.at(0);
		queuedImages.remove(0);
		emit queueSizeChanged(queuedImages.size());
		QTimer::singleShot(0, this, SLOT(getImages()));
		return;
	}
	
	//Try to create the path and open the file for writing
	try
	{
		//Get the absolute path
		QDir directoryPath = QFileInfo(*imageFile).absoluteDir();
		//Create all directories leading up to the current one.
		directoryPath.mkpath(".");
		//Open the image file for writing
		imageFile->open(QFile::WriteOnly);
	}
	catch(...) //If there's a problem
	{
		//Stop the download
		stoppingDownload = true;
		return;
	}
	
	//Download the first image in the queue
	//This will automatically call getImageResults when it is finished.
	emit currentlyDownloading(queuedImages.at(0)->getId());
	imageDownloader.download(imageUrl, true, imageFile);
}

void DownloadManager::getImageResults()
{
	//Flag the QFile object for deletion to prevent memory leaks
	imageDownloader.getFile()->deleteLater();
	
	//No longer downloading anything
	emit currentlyDownloading(-1);
	
	//Check if there was an error
	if(imageDownloader.hasError())
	{
		//Delete the image to prevent corruption
		imageDownloader.getFile()->remove();
		
		//If it was a file error
		if(imageDownloader.getErrorType() == Downloader::FileError)
		{
			//Stop the download
			stoppingDownload = true;
			return;
		}
		else //If it was a network error
		{
			imageAttempts++;
			imageTimeout = expDelay(1, imageAttempts, 32);
			imageWaiting = true;
			QTimer::singleShot(0, this, SLOT(getImages()));
			return;
		}
	}
	
	//If set to save json, do so now.
	if(saveJson)
	{
		saveJsonToFile();
	}
	
	//Reset attempts after a successful download
	imageAttempts = 0;
	
	//Increment number of images downloaded and remove the downloaded image from the queue
	imagesDownloaded++;
	emit downloaded(queuedImages.at(0)->getId());
	delete queuedImages.at(0);
	queuedImages.remove(0);
	emit queueSizeChanged(queuedImages.size());
	
	//Use Qtimer to queue calling getImages after a delay of delayBetweenImages
	QTimer::singleShot(delayBetweenImages, this, SLOT(getImages()));
}

void DownloadManager::stopDownload()
{
	stoppingDownload = true;
}

void DownloadManager::pauseDownload()
{
	downloadPaused = true;
}

void DownloadManager::unpauseDownload()
{
	downloadPaused = false;
}

/*
 * Parses the file naming format to create a file path.
 * 
 * Filename formatting:
 * 
 * {id} -						Image id
 * {name} -						Full image name
 * {original_name} -			Original file name
 * {uploader} -					Name of uploader
 * {ext} -						Image extension (format)
 * {year}, {month}, {day} -		Year/month/day the image was posted
 * {###} -						Image id floored to the number of digits provided. Examples follow
 *								{###}: 12 -> 12. 155 -> 100. 2573 -> 2500.
 *								{######}: 456 -> 456. 207624 -> 200000
 *								Combination of multiple tags:
 *								{######}/{####}/{id}.{ext}: 505597 -> 500000/505000/505597.jpeg
 */
QString DownloadManager::parseFormat(QString format, DerpiJson* json)
{
	QMap<QString, QString> tags;
	tags["{id}"] = QString::number(json->getId());
	tags["{name}"] = json->getName();
	tags["{original_name}"] = json->getOriginalName();
	tags["{uploader}"] = json->getUploader();
	tags["{ext}"] = json->getFormat();
	tags["{year}"] = QString::number(json->getYear());
	tags["{month}"] = QString::number(json->getMonth());
	tags["{day}"] = QString::number(json->getDay());
	tags["{score}"] = QString::number(json->getScore());
	tags["{upvotes}"] = QString::number(json->getUpvotes());
	tags["{downvotes}"] = QString::number(json->getDownvotes());
	tags["{faves}"] = QString::number(json->getFaves());
	tags["{comments}"] = QString::number(json->getComments());
	tags["{width}"] = QString::number(json->getWidth());
	tags["{height}"] = QString::number(json->getHeight());
	tags["{aspect_ratio}"] = QString::number(json->getAspectRatio());
	
	QMapIterator<QString, QString> i(tags);
	while(i.hasNext())
	{
		i.next();
		format.replace(i.key(), i.value());
	}
	
	QRegularExpression re("{#+}");
	
	//Find all # tag matches
	QRegularExpressionMatch match = re.match(format);
	while(match.hasMatch())
	{
		//Get the number of '#'s
		int places = match.capturedEnd(0) - match.capturedStart(0) - 2;
		
		int tagContent = floorNumber(json->getId(), places);
		
		//cut out the tag and replace it with the new number
		format = format.left(match.capturedStart()) + QString::number(tagContent) + format.right(format.length() - match.capturedEnd());
		
		//Find additional matches
		match = re.match(format);
	}
	
	qDebug() << format;
	return format;
}

int DownloadManager::floorNumber(int number, int places)
{
	int div = qPow(10, places - 1);
	return (number / div) * div;
}

/*
 * Determines the time to wait on failure (network issue, etc.) in an exponential manner.
 * Ex: 1 sec, then 2 sec, then 4 sec, then 8 sec...
 * 
 */
int DownloadManager::expDelay(int base, int attempts, int maxDelay)
{
	int delay = base * qPow(2, attempts - 1);
	if(delay < maxDelay)
		return delay;
	else
		return maxDelay;
}

void DownloadManager::saveJsonToFile()
{
	//Create the file object
	QFile jsonFile(parseFormat(jsonFileNameFormat, queuedImages.at(0)));
	
	//Try to create the file and directories leading up to it, then write the data
	try
	{
		//Get the absolute path
		QDir directoryPath = QFileInfo(jsonFile).absoluteDir();
		
		//Create all directories leading up to the current one.
		directoryPath.mkpath(".");
		
		//If file doesn't exist write data. If it does exist and updateJson is true, do the same. Otherwise skip writing data.
		if(!jsonFile.exists() || (jsonFile.exists() && updateJson))
		{
			//Open the file for writing
			jsonFile.open(QFile::WriteOnly);
			jsonFile.write(queuedImages.at(0)->getJson().toJson());
			jsonFile.close();
		}
	}
	catch(...)
	{
		//Stop the download
		stoppingDownload = true;
		emit fileError((int) jsonFile.error(), jsonFile.errorString(), 0);
		return;
	}
}
