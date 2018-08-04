#ifndef DOWNLOADMANAGER_H
#define DOWNLOADMANAGER_H

#include <QObject>

#include <QVector>

#include <QtMath>

#include <QRegularExpression>

#include "derpijson.h"
#include "downloader.h"
#include "timehelper.h"

/*
 * For automatic archiving, keep track of latest image downloaded and use it in the search query: id_number.gt:{} with sorting by creation date ascending.
 * 
 */

class DownloadManager : public QObject
{
	Q_OBJECT
private:
	enum SVGDownloadState { notCheckingSVG, checkingSVG, checkingPNG };
	
public:
	enum SVGMode { saveSVG = 0, savePNG = 1, saveSVGAndPNG = 2 };
	
	explicit DownloadManager(QNetworkAccessManager* netManager, QObject* parent = 0);
    
    QJsonObject getDefaultPreset();
	
signals:
	void finished();															//Reports when the session is completed
	void totalDownloadProgress(qint64 imagesComplete, qint64 imagesTotal);		//Reports the progress of the current download session
	void currentDownloadProgress(qint64 bytesReceived, qint64 bytesTotal);		//Reports the progress of the current image download
	
	void networkError(int errorCode, QString errorDesc, QUrl url);				//Reports network errors
	void fileError(int error, QString errorDesc, QFile* file);					//Reports file errors
	void reportError(QString errorMessage);
	
	void metadataTimeoutRemaining(int time);									//Reports metaDownloader timeout due to network error.
	void imageTimeoutRemaining(int time);										//Reports imageDownloader timeout due to network error
	
	void timingUpdate(QString elapsed, QString eta, int imagesPerMinute);		//Reports timing information
	
	void queueSizeChanged(int size);											//Reports the size of the queue
	
	void currentlyDownloading(int id);											//Reports the image that's currently being downloaded
	void downloaded(int id);													//Reports the last image successfully downloaded
	
public slots:
	void start(DerpiJson::SearchSettings searchSettings, QString imageFileNameFormat, int maxImages = -1, bool saveJson = false, bool updateJson = false, QString jsonFileNameFormat = "", SVGMode svgMode = saveSVG);
	
	void calculateTiming();
	
	void getMetadata();
	void getMetadataResults();
	void getImages();
	void getImageResults();
	
	void stopDownload();
	void pauseDownload();
	void unpauseDownload();
	
private:
	//Constants
	const int imagesToBuffer;
	const int delayBetweenImages;
	const int delayBetweenMetadata;
	const int delayWaitingForImages;
	const int delayWhilePaused;
	
	//Used to maintain downloads
	Downloader metaDownloader;						//The downloader that handles metadata
	Downloader imageDownloader;						//The downloader that handles images
	QVector<DerpiJson*> queuedImages;				//A list of images ready to be downloaded
	DerpiJson::SearchSettings searchSettings;		//Stores settings related to the search constraints.
	TimeHelper timeHelper;							//Used to maintain elapsed time and eta, along with images per minute
	
	int imagesQueued;		//Number of DerpiJson objects queued to download. Lets the metadata downloader know when to stop.
	int imagesDownloaded;	//Number of images actually downloaded. Used to update progress
	int imagesTotal;		//Number of images that are planned to be downloaded in the session. Used to update progress only
	
	bool firstMetadata;		//Whether the current metadata is the first. Used to set imagesTotal to keep track of progress.
	bool metadataWaiting;	//Whether the metadata function is waiting due to a network error.
	bool imageWaiting;		//Whetehr the image function is waiting due to a network error.
	
	int metadataAttempts;	//The number of attempts at the current metadata download. Used to create an exponential delay.
	int imageAttempts;		//The number of attempts at the current image download. Used to create an exponential delay.
	int metadataTimeout;	//Seconds remaining to wait in the metadata function.
	int imageTimeout;		//Seconds remaining to wait in the image function.
	
	QVector<int> lastPageIds;	//A list of the image ids that were on the last page. Used to make sure an image isn't downloaded twice in the same session.
								//Mostly an issue with creation date descending. If a new image is posted, the final image on the last page is pushed to the first image on the current page.
	
	//Set once per session
	int maxImagesToDownload;			//How many images should be downloaded. Lets the metadata downloader know when to stop adding more.
	QString imageFileNameFormat;		//The format for naming images
	QString jsonFileNameFormat;			//The format for naming json files
	bool saveJson;						//Whether to save json files or not
	bool updateJson;					//Whether to overwrite json even if it exists already
	SVGMode svgMode;					//Whether .svg files, .png files, or both are downloaded for images with SVG format.
	
	//Triggers
	bool stoppingDownload;				//Whether the download is stopping
	bool imageDownloaderStopped;		//Whether the image function has stopped. It is important that the image function finishes first as the metadata function clears the queue on stop.
	bool downloadPaused;				//Whether the download is paused
	bool noMoreImages;					//When the metadata downloader has no more images to add. When the queuedImages vector is empty and this is true, the download is complete.
	SVGDownloadState svgDownloadState;	//Whether the downloader is checking the .svg or .png version of images with SVG format.
	
private:
	QString parseFormat(QString format, DerpiJson* json, bool saveSVG = true);
	int floorNumber(int number, int places);
	int expDelay(int base, int attempts, int maxDelay);
	
	void saveJsonToFile();
};

#endif // DOWNLOADMANAGER_H
