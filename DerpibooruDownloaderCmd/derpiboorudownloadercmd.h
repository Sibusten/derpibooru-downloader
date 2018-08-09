#ifndef DERPIBOORUDOWNLOADERCMD_H
#define DERPIBOORUDOWNLOADERCMD_H

#include <QObject>

#include "downloadmanager.h"

class DerpibooruDownloaderCmd : public QObject
{
	Q_OBJECT
public:
	explicit DerpibooruDownloaderCmd(DownloadManager *manager, QObject *parent = nullptr);
	
	void start(DerpiJson::SearchSettings searchSettings, QString imageFileNameFormat, int maxImages, bool saveJson, bool updateJson, QString jsonFileNameFormat, DownloadManager::SVGMode svgMode);
	
signals:
	
private slots:
	void refreshOutput();
	
	void finished();
	
	void setCurrentDownloadProgress(qint64 bytesReceived, qint64 bytesTotal);
	void setTotalDownloadProgress(qint64 imagesDownloaded, qint64 imagesTotal);
	
	void networkError(int errorCode, QString errorDesc, QUrl url);
	void fileError(int errorCode, QString errorDesc, QFile* file);
	void reportError(QString errorMessage);
	
	void setMetadataTimeoutReading(int time);
	void setImageTimeoutReading(int time);
	void setTimingInformation(QString elapsed, QString eta, int imagesPerMinute);
	void setQueueSize(int size);
	void setCurrentlyDownloading(int id);
	
private:
	DownloadManager *manager;
	
	qint64 currentBytesReceived;
	qint64 currentBytesTotal;
	qint64 imagesDownloaded;
	qint64 imagesTotal;
	
	int queueSize;
	int currentlyDownloadingID;
	
	QString elapsed;
	QString eta;
	int imagesPerMinute;
};

#endif // DERPIBOORUDOWNLOADERCMD_H
