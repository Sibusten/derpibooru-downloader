#ifndef DOWNLOAD_H
#define DOWNLOAD_H

#include <QObject>

#include <QByteArray>
#include <QCryptographicHash>
#include <QNetworkAccessManager>
#include <QNetworkRequest>
#include <QNetworkReply>
#include <QFile>


/*
 * A class for downloading files on the internet.
 *
 * Requires a QNetworkAccessManager to be given on construction to use for downloads.
 * One QNetworkAccessManager is enough for up to 6 parallel Downloader objects.
 *
 */
class Downloader : public QObject {
	Q_OBJECT
public:
	enum ErrorType {
		NoError,
		FileError,
		NetworkError
	};

	explicit Downloader(QNetworkAccessManager* netManager, QObject* parent = 0);
	virtual ~Downloader();
	QByteArray getData() const;

	bool hasError();
	ErrorType getErrorType();
	int getErrorCode();
	QString getErrorDesc();

	QFile* getFile();
	QString getHash();

signals:
	void networkError(int errorCode, QString errorDesc, QUrl url);
	void fileError(int error, QString errorDesc, QFile* file);
	void downloadProgress(qint64 bytesReceived, qint64 bytesTotal);
	void downloaded();
	void finished();

public slots:
	void download(const QUrl url, bool streamToFile = false, QFile* file = 0);

private slots:
	void hasReadyRead();
	void downloadFinished();

private:
	int defaultBufferSize;
	int bufferSize;
	QNetworkAccessManager* netManager;
	bool streamToFile;
	QFile* file;
	QByteArray downloadedData;
	QCryptographicHash sha512Hash;

	bool _hasError;
	ErrorType errorType;
	int errorCode;
	QString errorDesc;

	void processData(QNetworkReply* reply);
};

#endif // DOWNLOAD_H
