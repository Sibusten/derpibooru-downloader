#include "downloader.h"

Downloader::Downloader(QNetworkAccessManager* netManager, QObject* parent) : QObject(parent), sha512Hash(QCryptographicHash::Sha512) {
	this->netManager = netManager;
	// this->defaultBufferSize = defaultBufferSize;
}

Downloader::~Downloader() {

}

QByteArray Downloader::getData() const {
	return downloadedData;
}

bool Downloader::hasError() {
	return _hasError;
}

Downloader::ErrorType Downloader::getErrorType() {
	return errorType;
}

int Downloader::getErrorCode() {
	return errorCode;
}

QString Downloader::getErrorDesc() {
	return errorDesc;
}

QFile*Downloader::getFile() {
	return file;
}

QString Downloader::getHash() {
	return sha512Hash.result().toHex();
}

void Downloader::download(const QUrl url, bool streamToFile, QFile* file) {
	// Empty any previous data
	downloadedData.clear();
	sha512Hash.reset();
	_hasError = false;
	errorType = NoError;
	errorCode = -1;
	errorDesc = "";

	// Assign private variables
	this->streamToFile = streamToFile;
	this->file = file;

	// Open file if needed
	if (streamToFile && !file->isOpen()) {
		try {
			file->open(QFile::WriteOnly);
		}
		catch(...) {
			_hasError = true;
			errorType = FileError;
			errorCode = (int)file->error();
			errorDesc = file->errorString();

			emit fileError(errorCode, errorDesc, file);
			emit finished();
			return;
		}
	}

	// Construct and send the request
	QNetworkRequest request(url);
	QNetworkReply* reply = netManager->get(request);

	// Forward a signal from the reply to allow progress tracking
	connect(reply, SIGNAL(downloadProgress(qint64,qint64)), this, SIGNAL(downloadProgress(qint64, qint64)));

	// Connect signals to process data as it is available
	connect(reply, SIGNAL(readyRead()), this, SLOT(hasReadyRead()));

	// Connect signal to process data once the download is complete
	connect(reply, SIGNAL(finished()), this, SLOT(downloadFinished()));
}

void Downloader::hasReadyRead() {
	// Obtain the reply (sender)
	QNetworkReply* reply = qobject_cast<QNetworkReply*>(sender());

	processData(reply);
}

void Downloader::downloadFinished() {
	// Obtain the reply (sender)
	QNetworkReply* reply = qobject_cast<QNetworkReply*>(sender());

	// Flag the reply to be deleted when this function is complete
	reply->deleteLater();

	// Check if there was an error with the request
	if (reply->error()) {
		_hasError = true;
		errorType = NetworkError;
		errorCode = reply->error();
		errorDesc = reply->errorString();

		emit networkError(errorCode, errorDesc, reply->url());
		emit finished();
		return;
	}

	processData(reply);

	// Close file if it was used
	if (streamToFile) {
		file->close();
	}

	emit downloaded();
	emit finished();
}

void Downloader::processData(QNetworkReply* reply) {
	// Collect available data from the reply
	QByteArray data = reply->readAll();

	// Update hash
	this->sha512Hash.addData(data);

	// If the data should be streamed to a file
	if(streamToFile) {
		try {
			// Write data to file
			file->write(data);
		}
		catch(...) {
			// Risky? Maybe avoid closing, or add another try/catch?
			file->close();

			// Stop current download. Disconnect the signal because abort() will cause reply to emit finished()
			disconnect(reply, SIGNAL(finished()), this, SLOT(downloadFinished()));
			reply->abort();

			_hasError = true;
			errorType = FileError;
			errorCode = (int)file->error();
			errorDesc = file->errorString();

			emit fileError(errorCode, errorDesc, file);
			emit finished();
			return;
		}
	}
	else {
		//Write data to memory
		downloadedData.append(data);
	}
}
