#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QCloseEvent>
#include <QMessageBox>

#include "derpijson.h"
#include "downloadmanager.h"

namespace Ui {
class MainWindow;
}

class MainWindow : public QMainWindow
{
	Q_OBJECT
	
public:
	explicit MainWindow(DownloadManager* manager, QWidget *parent = 0);
	~MainWindow();
	
protected:
	void closeEvent(QCloseEvent* event);
	
private slots:
	void finished();
	
	void setCurrentDownloadProgress(qint64 bytesReceived, qint64 bytesTotal);
	void setTotalDownloadProgress(qint64 imagesDownloaded, qint64 imagesTotal);
	
	void networkError(int errorCode, QString errorDesc, QUrl url);
	void fileError(int errorCode, QString errorDesc, QFile* file);
	
	void setMetadataTimeoutReading(int time);
	void setImageTimeoutReading(int time);
	void setTimingInformation(QString elapsed, QString eta, int imagesPerMinute);
	void setQueueSize(int size);
	void setCurrentlyDownloading(int id);
	
	void confirmAddPreset(QAbstractButton* button);
	void confirmRemoveCurrentPreset(QAbstractButton* button);
	void confirmCloseWhileRunning(QAbstractButton* button);
	
	void showAbout();
	
	void on_saveJson_toggled(bool checked);
	void on_scoreConstraint_toggled(bool checked);
	void on_limitImagesDownloadedCheck_toggled(bool checked);
	void on_customFilterCheck_toggled(bool checked);
	void on_showAdditionalInfo_toggled(bool checked);
	void on_enterAPIKeyButton_clicked();
	void on_apiKey_returnPressed();
	void on_removeAPIKeyButton_clicked();
	void on_exportButton_clicked();
	void on_importButton_clicked();
	void on_clearErrorLogButton_clicked();
	void on_loadPreset_clicked();
	void on_deletePreset_clicked();
	void on_savePreset_clicked();
	void on_presetName_returnPressed();
	void on_startButton_clicked();
	void on_pauseButton_clicked();
	void on_stopButton_clicked();
	
private:
	Ui::MainWindow* ui;
	DownloadManager* manager;
	
	const QString VERSION;
	const QString DEFAULT_PRESET;
	
	QString apiKey;
	QJsonObject presets;
	bool isRunning;
	bool isPaused;
	
	DerpiJson::SearchSettings getSearchSetttings();
	
	void resetInformation();
	
	void setHasAPIKey(bool hasKey);
	QJsonArray exportPreset();
	void importPreset(QJsonArray preset);
	
	void addPreset();
	void removeCurrentPreset();
	QJsonArray getCurrentPreset();
	void updatePresetCombobox();
	
	
	
	QString encodeJson(QJsonDocument doc);
	QJsonDocument decodeJson(QString encodedJson);
	
	void reportError(QString errorMessage);
	
	void saveSettings();
	void loadSettings();
};

#endif // MAINWINDOW_H
