#include "derpiboorudownloadercmd.h"

#include <QCoreApplication>

DerpibooruDownloaderCmd::DerpibooruDownloaderCmd(DownloadManager* manager, QObject *parent) : QObject(parent),
	manager(manager)
{
	//Connect finished signal
	connect(manager, SIGNAL(finished()), this, SLOT(finished()));
	
	//Connect download progress signals
	connect(manager, SIGNAL(currentDownloadProgress(qint64,qint64)), this, SLOT(setCurrentDownloadProgress(qint64,qint64)));
	connect(manager, SIGNAL(totalDownloadProgress(qint64,qint64)), this, SLOT(setTotalDownloadProgress(qint64,qint64)));
	
	//Connect error signals
	connect(manager, SIGNAL(networkError(int,QString,QUrl)), this, SLOT(networkError(int,QString,QUrl)));
	connect(manager, SIGNAL(fileError(int,QString,QFile*)), this, SLOT(fileError(int,QString,QFile*)));
	connect(manager, SIGNAL(reportError(QString)), this, SLOT(reportError(QString)));
	
	//Connect other information signals
	connect(manager, SIGNAL(metadataTimeoutRemaining(int)), this, SLOT(setMetadataTimeoutReading(int)));
	connect(manager, SIGNAL(imageTimeoutRemaining(int)), this, SLOT(setImageTimeoutReading(int)));
	connect(manager, SIGNAL(timingUpdate(QString,QString,int)), this, SLOT(setTimingInformation(QString,QString,int)));
	connect(manager, SIGNAL(queueSizeChanged(int)), this, SLOT(setQueueSize(int)));
	connect(manager, SIGNAL(currentlyDownloading(int)), this, SLOT(setCurrentlyDownloading(int)));
}

void DerpibooruDownloaderCmd::start(DerpiJson::SearchSettings searchSettings, QString imageFileNameFormat, int maxImages, bool saveJson, bool updateJson, QString jsonFileNameFormat, DownloadManager::SVGMode svgMode)
{
	manager->start(searchSettings, imageFileNameFormat, maxImages, saveJson, updateJson, jsonFileNameFormat, svgMode);
}

void DerpibooruDownloaderCmd::refreshOutput()
{
	QTextStream(stdout) << QString::number(imagesDownloaded + 1) + "/" + QString::number(imagesTotal) + " - " + QString::number(currentlyDownloadingID) << endl;
}

void DerpibooruDownloaderCmd::finished()
{
	QTextStream(stdout) << "Finished";
	QCoreApplication::quit();
}

void DerpibooruDownloaderCmd::setCurrentDownloadProgress(qint64 bytesReceived, qint64 bytesTotal)
{
	this->currentBytesReceived = bytesReceived;
	this->currentBytesTotal = bytesTotal;
}

void DerpibooruDownloaderCmd::setTotalDownloadProgress(qint64 imagesDownloaded, qint64 imagesTotal)
{
	this->imagesDownloaded = imagesDownloaded;
	this->imagesTotal = imagesTotal;
	refreshOutput();
}

void DerpibooruDownloaderCmd::networkError(int errorCode, QString errorDesc, QUrl url)
{
	
}

void DerpibooruDownloaderCmd::fileError(int errorCode, QString errorDesc, QFile* file)
{
	
}

void DerpibooruDownloaderCmd::reportError(QString errorMessage)
{
	
}

void DerpibooruDownloaderCmd::setMetadataTimeoutReading(int time)
{
	
}

void DerpibooruDownloaderCmd::setImageTimeoutReading(int time)
{
	
}

void DerpibooruDownloaderCmd::setTimingInformation(QString elapsed, QString eta, int imagesPerMinute)
{
	this->elapsed = elapsed;
	this->eta = eta;
	this->imagesPerMinute = imagesPerMinute;
}

void DerpibooruDownloaderCmd::setQueueSize(int size)
{
	this->queueSize = size;
}

void DerpibooruDownloaderCmd::setCurrentlyDownloading(int id)
{
	this->currentlyDownloadingID = id;
}
