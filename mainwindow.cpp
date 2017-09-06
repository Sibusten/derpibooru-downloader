#include "mainwindow.h"
#include "ui_mainwindow.h"

#include <QNetworkAccessManager>

#include <QSettings>

MainWindow::MainWindow(DownloadManager* manager, QWidget *parent) :
	QMainWindow(parent),
	ui(new Ui::MainWindow),
	manager(manager),
	VERSION(QApplication::applicationVersion()),
	DEFAULT_PRESET("AAAAUXici1ZS0jHUMTUAEgY6aYk5xalAGgSVXPLL83LyE1OK9aszU2r1qlMrSmqVdIDKIapwkLEAxtsZ+A=="),
	isRunning(false),
	isPaused(false)
{
	ui->setupUi(this);
	
	setWindowTitle("Derpibooru Downloader v" + VERSION);
	setWindowIcon(QIcon(QPixmap(":/icon128.png")));
	
	//Connect finished signal
	connect(manager, SIGNAL(finished()), this, SLOT(finished()));
	
	//Connect download progress signals
	connect(manager, SIGNAL(currentDownloadProgress(qint64,qint64)), this, SLOT(setCurrentDownloadProgress(qint64,qint64)));
	connect(manager, SIGNAL(totalDownloadProgress(qint64,qint64)), this, SLOT(setTotalDownloadProgress(qint64,qint64)));
	
	//Connect error signals
	connect(manager, SIGNAL(networkError(int,QString,QUrl)), this, SLOT(networkError(int,QString,QUrl)));
	connect(manager, SIGNAL(fileError(int,QString,QFile*)), this, SLOT(fileError(int,QString,QFile*)));
	
	//Connect other information signals
	connect(manager, SIGNAL(metadataTimeoutRemaining(int)), this, SLOT(setMetadataTimeoutReading(int)));
	connect(manager, SIGNAL(imageTimeoutRemaining(int)), this, SLOT(setImageTimeoutReading(int)));
	connect(manager, SIGNAL(timingUpdate(QString,QString,int)), this, SLOT(setTimingInformation(QString,QString,int)));
	connect(manager, SIGNAL(queueSizeChanged(int)), this, SLOT(setQueueSize(int)));
	connect(manager, SIGNAL(currentlyDownloading(int)), this, SLOT(setCurrentlyDownloading(int)));
	
	//connect about action
	connect(ui->actionAbout, SIGNAL(triggered(bool)), this, SLOT(showAbout()));
	
	//Toggle hidden areas before loading settings to make sure they're hidden if needed
	on_saveJson_toggled(false);
	on_showAdditionalInfo_toggled(false);
	
	//Load settings from disk
	loadSettings();
	
	//Set whether an api key was saved
	setHasAPIKey(!apiKey.isEmpty());
	
	//Update combobox to show presets
	updatePresetCombobox();
}

MainWindow::~MainWindow()
{
	delete ui;
	manager->deleteLater();
}

void MainWindow::closeEvent(QCloseEvent* event)
{
	saveSettings();
	if(isRunning)
	{
		event->ignore();
		
		//Ask for confirmation of close
		if(!ui->suppressWarnings->isChecked())
		{
			QMessageBox* messageBox = new QMessageBox(this);
			messageBox->setAttribute(Qt::WA_DeleteOnClose);
			messageBox->setStandardButtons(QMessageBox::Yes | QMessageBox::No);
			messageBox->setDefaultButton(QMessageBox::No);
			messageBox->setWindowTitle("Confirm Exit");
			messageBox->setText("Are you sure you want to exit?\nThe current download will be stopped.");
			messageBox->open(this, SLOT(confirmCloseWhileRunning(QAbstractButton*)));
		}
		else
		{
			confirmCloseWhileRunning(0);
		}
		
	}
	else
	{
		event->accept();
	}
}

void MainWindow::finished()
{
	ui->startButton->setEnabled(true);
	ui->pauseButton->setEnabled(false);
	ui->stopButton->setEnabled(false);
	ui->groupSearchOptions->setEnabled(true);
	ui->groupFileOptions->setEnabled(true);
	isRunning = false;
	resetInformation();
}

void MainWindow::setCurrentDownloadProgress(qint64 bytesReceived, qint64 bytesTotal)
{
	ui->currentDownloadProgress->setMaximum(bytesTotal);
	ui->currentDownloadProgress->setValue(bytesReceived);
}

void MainWindow::setTotalDownloadProgress(qint64 imagesDownloaded, qint64 imagesTotal)
{
	ui->totalDownloadProgress->setMaximum(imagesTotal);
	ui->totalDownloadProgress->setValue(imagesDownloaded);
	ui->totalDownloadProgressLabel->setText(QString::number(imagesDownloaded) + "/" + QString::number(imagesTotal));
}

void MainWindow::networkError(int errorCode, QString errorDesc, QUrl url)
{
	reportError(QString("Network Error - ") + QString::number(errorCode) + " - " + errorDesc);
}

void MainWindow::fileError(int errorCode, QString errorDesc, QFile* file)
{
	reportError(QString("File Error - ") + QString::number(errorCode) + " - " + errorDesc);
}

void MainWindow::setMetadataTimeoutReading(int time)
{
	if(time != 0)
	{
		ui->metadataTimeout->setText(QString::number(time));
	}
	else
	{
		ui->metadataTimeout->setText("---");
	}
}

void MainWindow::setImageTimeoutReading(int time)
{
	if(time != 0)
	{
		ui->imageTimeout->setText(QString::number(time));
	}
	else
	{
		ui->imageTimeout->setText("---");
	}
}

void MainWindow::setTimingInformation(QString elapsed, QString eta, int imagesPerMinute)
{
	ui->elapsed->setText(elapsed);
	ui->eta->setText(eta);
	if(imagesPerMinute != -1)
	{
		ui->imagesPerMinute->setText(QString::number(imagesPerMinute));
	}
}

void MainWindow::setQueueSize(int size)
{
	ui->queueSize->setText(QString::number(size));
}

void MainWindow::setCurrentlyDownloading(int id)
{
	if(id != -1)
	{
		ui->currentDownloadProgressLabel->setText(QString::number(id));
	}
}

//If user says yes, overwrite the preset
void MainWindow::confirmAddPreset(QAbstractButton* button)
{
	bool willDoAction = false;
	//If warnings are not suppressed, check which button the user clicked. Otherwise, just do the action
	if(!ui->suppressWarnings->isChecked())
	{
		QMessageBox* messageBox = qobject_cast<QMessageBox*>(sender());
		QMessageBox::StandardButton btn = messageBox->standardButton(button);
		
		if(btn == QMessageBox::Yes)
		{
			willDoAction = true;
		}
	}
	else
	{
		willDoAction = true;
	}
	
	if(willDoAction)
	{
		QString presetName = ui->presetName->text();
		
		//If the text field is empty, use the currently selected preset as the name
		if(presetName.isEmpty())
		{
			presetName = ui->presetCombobox->currentText();
		}
		
		QJsonArray preset = exportPreset();
		presets[presetName] = preset;
		ui->presetName->clear();
		updatePresetCombobox();
		//Scroll to new preset
		ui->presetCombobox->setCurrentIndex(ui->presetCombobox->findText(presetName));
		saveSettings();
	}
}

//If user says yes, delete the preset
void MainWindow::confirmRemoveCurrentPreset(QAbstractButton* button)
{
	bool willDoAction = false;
	//If warnings are not suppressed, check which button the user clicked. Otherwise, just do the action
	if(!ui->suppressWarnings->isChecked())
	{
		QMessageBox* messageBox = qobject_cast<QMessageBox*>(sender());
		QMessageBox::StandardButton btn = messageBox->standardButton(button);
		
		if(btn == QMessageBox::Yes)
		{
			willDoAction = true;
		}
	}
	else
	{
		willDoAction = true;
	}
	
	if(willDoAction)
	{
		QString presetName = ui->presetCombobox->currentText();
		presets.remove(presetName);
		updatePresetCombobox();
		saveSettings();
	}
}

//If user says yes, stop the download and wait for it to stop before closing
void MainWindow::confirmCloseWhileRunning(QAbstractButton* button)
{
	bool willDoAction = false;
	//If warnings are not suppressed, check which button the user clicked. Otherwise, just do the action
	if(!ui->suppressWarnings->isChecked())
	{
		QMessageBox* messageBox = qobject_cast<QMessageBox*>(sender());
		QMessageBox::StandardButton btn = messageBox->standardButton(button);
		
		if(btn == QMessageBox::Yes)
		{
			willDoAction = true;
		}
	}
	else
	{
		willDoAction = true;
	}
	
	if(willDoAction)
	{
		if(!isRunning)
		{
			close();
		}
		else
		{
			//close when done
			connect(manager, SIGNAL(finished()), this, SLOT(close()));
		}
		manager->stopDownload();
		setEnabled(false);
	}
}

void MainWindow::showAbout()
{
	QMessageBox* messageBox = new QMessageBox(this);
	messageBox->setAttribute(Qt::WA_DeleteOnClose);
	messageBox->setStandardButtons(QMessageBox::Ok);
	messageBox->setDefaultButton(QMessageBox::Ok);
	messageBox->setWindowTitle("About");
	messageBox->setText("Derpibooru Downloader v" + VERSION + "\n\nBy Sibusten");
	messageBox->setText("<html><body>Derpibooru Downloader v" + VERSION +
						"<br><br>By <a href='https://github.com/Sibusten'>Sibusten</a><br><br>" +
						"<a href='https://github.com/Sibusten/derpibooru-downloader'>GitHub Page</a></body></html>");
	messageBox->setIconPixmap(QPixmap(":/icon128.png"));
	messageBox->show();
}

void MainWindow::on_saveJson_toggled(bool checked)
{
    ui->frameSaveJson->setVisible(checked);
	ui->frameJsonOptions->setVisible(checked);
}

void MainWindow::on_limitImagesDownloadedCheck_toggled(bool checked)
{
    ui->limitImagesDownloaded->setEnabled(checked);
}

void MainWindow::on_customFilterCheck_toggled(bool checked)
{
    ui->customFilterNumber->setEnabled(checked);
	ui->filter->setEnabled(!checked);
	ui->filterLabel->setEnabled(!checked);
}

void MainWindow::on_showAdditionalInfo_toggled(bool checked)
{
	
	if(checked)
	{
		ui->verticalSpacer_2->changeSize(0, 0, QSizePolicy::Ignored, QSizePolicy::Ignored);
		
	}
	else
	{
		ui->verticalSpacer_2->changeSize(0, 0, QSizePolicy::Ignored, QSizePolicy::Expanding);
	}
	ui->frameAdditionalInfo->setVisible(checked);
}

void MainWindow::setHasAPIKey(bool hasKey)
{
	ui->apiKey->setDisabled(hasKey);
	ui->enterAPIKeyButton->setDisabled(hasKey);
	
	//Disable filter selection if an api key is given.
	//As of Feb 13, 2017, The site will *always* use your selected filter on the site, and ignore the custom one sent with the search query.
	ui->filterFrame->setEnabled(!hasKey);
	
	//Set text to show if a key is given
	if(hasKey)
	{
		ui->apiKeyStatus->setText("Key given");
		ui->apiKeyStatus->setStyleSheet("QLabel { color: green; }");
	}
	else
	{
		ui->apiKeyStatus->setText("Key not given");
		ui->apiKeyStatus->setStyleSheet("QLabel { color: red; }");
	}
}

/*
 * Exports the current state of search options to a QJsonArray
 * 
 */
QJsonArray MainWindow::exportPreset()
{
	QJsonArray preset;
	preset.append(ui->query->text());
	preset.append(ui->startingPage->value());
	preset.append(ui->imagesPerPage->value());
	preset.append(ui->limitImagesDownloaded->value());
	preset.append(ui->filter->currentIndex());
	preset.append(ui->customFilterCheck->isChecked());
	preset.append(ui->customFilterNumber->value());
	preset.append(ui->searchFormat->currentIndex());
	preset.append(ui->searchDirection->currentIndex());
	preset.append(ui->imageFileNameFormat->text());
	preset.append(ui->jsonFileNameFormat->text());
	preset.append(ui->saveJson->isChecked());
	preset.append(ui->updateJson->isChecked());
	preset.append(ui->includeComments->isChecked());
	preset.append(ui->includeFavorites->isChecked());
	preset.append(ui->limitImagesDownloadedCheck->isChecked());
	return preset;
}

void MainWindow::importPreset(QJsonArray preset)
{
	ui->query->setText(preset[0].toString());
	ui->startingPage->setValue(preset[1].toInt());
	ui->imagesPerPage->setValue(preset[2].toInt());
	ui->limitImagesDownloaded->setValue(preset[3].toInt());
	ui->filter->setCurrentIndex(preset[4].toInt());
	ui->customFilterCheck->setChecked(preset[5].toBool());
	ui->customFilterNumber->setValue(preset[6].toInt());
	ui->searchFormat->setCurrentIndex(preset[7].toInt());
	ui->searchDirection->setCurrentIndex(preset[8].toInt());
	ui->imageFileNameFormat->setText(preset[9].toString());
	ui->jsonFileNameFormat->setText(preset[10].toString());
	ui->saveJson->setChecked(preset[11].toBool());
	ui->updateJson->setChecked(preset[12].toBool());
	ui->includeComments->setChecked(preset[13].toBool());
	ui->includeFavorites->setChecked(preset[14].toBool());
	ui->limitImagesDownloadedCheck->setChecked(preset[15].toBool());
}

void MainWindow::addPreset()
{
	QString presetName = ui->presetName->text().trimmed();
	
	//If the text field is empty, use the currently selected preset as the name
	if(presetName.isEmpty())
	{
		presetName = ui->presetCombobox->currentText();
	}
	
	//Prevent overwriting the default preset
	if(presetName == "-Default-")
	{
		reportError("The default preset cannot be overwritten");
		return;
	}
	
	//If the save would overwrite an existing preset
	if(ui->presetCombobox->findText(presetName) != -1)
	{
		//Ask for confirmation of overwrite
		if(!ui->suppressWarnings->isChecked())
		{
			QMessageBox* messageBox = new QMessageBox(this);
			messageBox->setAttribute(Qt::WA_DeleteOnClose);
			messageBox->setStandardButtons(QMessageBox::Yes | QMessageBox::No);
			messageBox->setDefaultButton(QMessageBox::No);
			messageBox->setWindowTitle("Confirm Preset Save");
			messageBox->setText("Are you sure you want to save preset \"" + presetName + "\"?\nThe currently saved preset will be overwritten.");
			messageBox->open(this, SLOT(confirmAddPreset(QAbstractButton*)));
		}
		else
		{
			confirmAddPreset(0);
		}
	}
	else
	{
		//Save preset
		QJsonArray preset = exportPreset();
		presets[presetName] = preset;
		ui->presetName->clear();
		updatePresetCombobox();
		//Scroll to new preset
		ui->presetCombobox->setCurrentIndex(ui->presetCombobox->findText(presetName));
		saveSettings();
	}
}

void MainWindow::removeCurrentPreset()
{
	//If the user is trying to delete the default preset
	if(ui->presetCombobox->currentIndex() == 0)
	{
		reportError("The default preset cannot be deleted");
		return;
	}
	
	QString presetName = ui->presetCombobox->currentText();
	
	//Ask for confirmation of deletion
	if(!ui->suppressWarnings->isChecked())
	{
		QMessageBox* messageBox = new QMessageBox(this);
		messageBox->setAttribute(Qt::WA_DeleteOnClose);
		messageBox->setStandardButtons(QMessageBox::Yes | QMessageBox::No);
		messageBox->setDefaultButton(QMessageBox::No);
		messageBox->setWindowTitle("Confirm Preset Deletion");
		messageBox->setText("Are you sure you want to delete preset \"" + presetName + "\"?");
		messageBox->open(this, SLOT(confirmRemoveCurrentPreset(QAbstractButton*)));
	}
	else
	{
		confirmRemoveCurrentPreset(0);
	}
}

QJsonArray MainWindow::getCurrentPreset()
{
	QString presetName = ui->presetCombobox->currentText();
	return presets[presetName].toArray();
}

void MainWindow::updatePresetCombobox()
{
	//Get preset names
	QStringList keys = presets.keys();
	
	//Get current preset selection to preserve after the refresh
	QString currentSelection = ui->presetCombobox->currentText();
	int currentIndex = ui->presetCombobox->currentIndex();
	
	//Add default preset first
	ui->presetCombobox->clear();
	ui->presetCombobox->addItem("-Default-");
	
	//Do not sort the default preset
	keys.removeOne("-Default-");
	
	//Sort and add the rest
	keys.sort();
	for(int i = 0; i < keys.size(); i++)
	{
		ui->presetCombobox->addItem(keys.at(i));
	}
	
	//If the combobox was not set, set it to 0
	if(currentIndex == -1)
	{
		ui->presetCombobox->setCurrentIndex(0);
		return;
	}
	
	//Try to return to what the user had selected, if possible
	int index = ui->presetCombobox->findText(currentSelection);
	if(index != -1)
	{
		ui->presetCombobox->setCurrentIndex(index);
	}
	else
	{
		//Fallback, if it's not found, move to the index that would be right above it
		int newIndex = currentIndex - 1;
		if(newIndex >= 0)
		{
			ui->presetCombobox->setCurrentIndex(newIndex);
		}
	}
}

QString MainWindow::encodeJson(QJsonDocument doc)
{
	return QString::fromUtf8(qCompress(doc.toJson(QJsonDocument::Compact)).toBase64());
}

QJsonDocument MainWindow::decodeJson(QString encodedJson)
{
	return QJsonDocument::fromJson(qUncompress(QByteArray::fromBase64(encodedJson.toUtf8())));
}

void MainWindow::reportError(QString errorMessage)
{
	QString time = QTime::currentTime().toString("HH:mm:ss");
	ui->errorLog->append("[" + time + "] - " + errorMessage);
}

void MainWindow::saveSettings()
{
	QSettings settings("Sibusten", "DerpibooruArchiver");
	QJsonObject settingsObj;
	settingsObj["currentOptions"] = exportPreset();
	settingsObj["apiKey"] = apiKey;
	settingsObj["presets"] = presets;
	settingsObj["showAdditional"] = ui->showAdditionalInfo->isChecked();
	settingsObj["currentPreset"] = ui->presetCombobox->currentIndex();
	settingsObj["suppressWarnings"] = ui->suppressWarnings->isChecked();
	
	QString settingsString = encodeJson(QJsonDocument(settingsObj));
	
	settings.setValue("mainSettings", settingsString);
	settings.setValue("geometry", saveGeometry());
}

void MainWindow::loadSettings()
{
	QSettings settings("Sibusten", "DerpibooruArchiver");
	
	//settings.remove("mainSettings");
	QString settingsString = settings.value("mainSettings", "").toString();

	//If options are saved, load them
	if(!settingsString.isEmpty())
	{
		QJsonObject settingsObj = decodeJson(settingsString).object();
		importPreset(settingsObj["currentOptions"].toArray());
		apiKey = settingsObj["apiKey"].toString();
		presets = settingsObj["presets"].toObject();
		updatePresetCombobox();
		ui->showAdditionalInfo->setChecked(settingsObj["showAdditional"].toBool());
		ui->presetCombobox->setCurrentIndex(settingsObj["currentPreset"].toInt());
		ui->suppressWarnings->setChecked(settingsObj["suppressWarnings"].toBool());
	}
	else
	{
		//Add default preset
		presets["-Default-"] = decodeJson(DEFAULT_PRESET).array();
	}
	
	restoreGeometry(settings.value("geometry").toByteArray());
}

void MainWindow::on_enterAPIKeyButton_clicked()
{
    QString key = ui->apiKey->text().trimmed();
	
	if(key.size() == 20)	//API Keys are 20 characters long
	{
		ui->apiKey->clear();
		apiKey = key;
		setHasAPIKey(true);
	}
	else
	{
		reportError("API Key length invalid. Expected 20, got (" + QString::number(key.size()) + ")");
	}
}

//Ease of use signal forward
void MainWindow::on_apiKey_returnPressed()
{
    on_enterAPIKeyButton_clicked();
}

void MainWindow::on_removeAPIKeyButton_clicked()
{
    apiKey = "";
	setHasAPIKey(false);
}

void MainWindow::on_exportButton_clicked()
{
    ui->exportImportCode->setText(encodeJson(QJsonDocument(exportPreset())));
}

void MainWindow::on_importButton_clicked()
{
	QJsonDocument json = decodeJson(ui->exportImportCode->text());
	if(json.isEmpty())
	{
		reportError("Import data is corrupted");
	}
	else
	{
		importPreset(json.array());
		ui->exportImportCode->clear();
	}
}

void MainWindow::on_clearErrorLogButton_clicked()
{
    ui->errorLog->clear();
}

void MainWindow::on_loadPreset_clicked()
{
    importPreset(getCurrentPreset());
}

void MainWindow::on_deletePreset_clicked()
{
    removeCurrentPreset();
}

void MainWindow::on_savePreset_clicked()
{
    addPreset();
}

//Ease of use signal forward
void MainWindow::on_presetName_returnPressed()
{
    on_savePreset_clicked();
}

void MainWindow::on_startButton_clicked()
{
    saveSettings();
	ui->startButton->setEnabled(false);
	ui->pauseButton->setEnabled(true);
	ui->stopButton->setEnabled(true);
	ui->groupSearchOptions->setEnabled(false);
	ui->groupFileOptions->setEnabled(false);
	isRunning = true;
	
	int limitImagesDownloaded = ui->limitImagesDownloaded->value();
	if(!ui->limitImagesDownloadedCheck->isChecked())
	{
		limitImagesDownloaded = -1;
	}
	
	manager->start(getSearchSetttings(), ui->imageFileNameFormat->text(), limitImagesDownloaded, ui->saveJson->isChecked(), ui->updateJson->isChecked(), ui->jsonFileNameFormat->text());
}

void MainWindow::on_pauseButton_clicked()
{
	if(!isPaused)
	{
		manager->pauseDownload();
		ui->pauseButton->setText("Unpause");
		isPaused = true;
	}
    else
	{
		manager->unpauseDownload();
		ui->pauseButton->setText("Pause");
		isPaused = false;
	}
}

void MainWindow::on_stopButton_clicked()
{
    manager->stopDownload();
	manager->unpauseDownload();
	ui->pauseButton->setText("Pause");
	ui->pauseButton->setEnabled(false);
	ui->stopButton->setEnabled(false);
}

DerpiJson::SearchSettings MainWindow::getSearchSetttings()
{
	//Get filter id
	int filterId = 0;
	if(ui->customFilterCheck->isChecked())
	{
		filterId = ui->customFilterNumber->value();
	}
	else
	{
		//-User Default-, Everything, 18+ R34, 18+ Dark, Default, Maximum Spoilers, Legacy Default
		int defaultFilterIds[] = {-1, 56027, 37432, 37429, 100073, 37430, 37431};
		filterId = defaultFilterIds[ui->filter->currentIndex()];
	}
	
	//if search query is empty, assume '*'
	QString query = ui->query->text();
	if(query.isEmpty())
	{
		query = "*";
	}
	
	return DerpiJson::SearchSettings(query, ui->startingPage->value(), ui->imagesPerPage->value(), ui->includeComments->isChecked(), ui->includeFavorites->isChecked(),
									 ui->searchFormat->currentIndex(), ui->searchDirection->currentIndex(), apiKey, filterId, qrand());
}

void MainWindow::resetInformation()
{
	ui->currentDownloadProgress->setValue(0);
	ui->totalDownloadProgress->setValue(0);
	ui->totalDownloadProgress->setMaximum(100);
	
	ui->currentDownloadProgressLabel->setText("---");
	ui->totalDownloadProgressLabel->setText("---/---");
	
	ui->eta->setText("---:---:---");
	ui->imagesPerMinute->setText("---");
	
	ui->queueSize->setText("---");
}
