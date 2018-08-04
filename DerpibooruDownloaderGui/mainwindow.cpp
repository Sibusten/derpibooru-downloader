#include "mainwindow.h"
#include "ui_mainwindow.h"

#include <QNetworkAccessManager>

#include <QSettings>

MainWindow::MainWindow(DownloadManager* manager, QWidget *parent) :
	QMainWindow(parent),
	ui(new Ui::MainWindow),
	manager(manager),
	VERSION(QApplication::applicationVersion()),
	DEFAULT_PRESET("AAAAUXici1ZS0jHUMTUAEgY6aYk5xalAGgSVXPLL83LyE1OK9aszU2r1qlMrSmqVdIDKIapwkLEAxtsZ+A=="),  // TODO remove this, and remove export/import presets
	isRunning(false),
	isPaused(false)
{
	ui->setupUi(this);
	
	setWindowTitle("Derpibooru Downloader v" + VERSION);
	setWindowIcon(QIcon(QPixmap(":/icon128.png")));
	
	// Set up SVG button group
	ui->buttonGroupSVGOptions->setId(ui->radioSaveSVG, 0);
	ui->buttonGroupSVGOptions->setId(ui->radioSavePNG, 1);
	ui->buttonGroupSVGOptions->setId(ui->radioSaveBoth, 2);
	
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
		
		QJsonObject preset = exportPreset();
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
 * Exports the current state of search options to a QJsonObject
 * 
 */
QJsonObject MainWindow::exportPreset()
{
	QJsonObject preset;
	
	preset.insert("query", ui->query->text());
	preset.insert("startPage", ui->startingPage->value());
	preset.insert("perPage", ui->imagesPerPage->value());
	preset.insert("imageLimit", ui->limitImagesDownloaded->value());
	preset.insert("filter", ui->filter->currentIndex());
	preset.insert("useCustomFilter", ui->customFilterCheck->isChecked());
	preset.insert("customFilterID", ui->customFilterNumber->value());
	preset.insert("searchFormat", ui->searchFormat->currentIndex());
	preset.insert("searchDirection", ui->searchDirection->currentIndex());
	preset.insert("imagePathFormat", ui->imageFileNameFormat->text());
	preset.insert("jsonPathFormat", ui->jsonFileNameFormat->text());
	preset.insert("saveJson", ui->saveJson->isChecked());
	preset.insert("updateJson", ui->updateJson->isChecked());
	preset.insert("jsonComments", ui->includeComments->isChecked());
	preset.insert("jsonFavorites", ui->includeFavorites->isChecked());
	preset.insert("limitImages", ui->limitImagesDownloadedCheck->isChecked());
	preset.insert("svgAction", ui->buttonGroupSVGOptions->checkedId());

	return preset;
}

void MainWindow::importPreset(QJsonObject preset)
{
	ui->query->setText(preset["query"].toString(""));
	ui->startingPage->setValue(preset["startPage"].toInt(1));
	ui->imagesPerPage->setValue(preset["perPage"].toInt(50));
	ui->limitImagesDownloaded->setValue(preset["imageLimit"].toInt(1));
	ui->filter->setCurrentIndex(preset["filter"].toInt(0));
	ui->customFilterCheck->setChecked(preset["useCustomFilter"].toBool(false));
	ui->customFilterNumber->setValue(preset["customFilterID"].toInt(0));
	ui->searchFormat->setCurrentIndex(preset["searchFormat"].toInt(0));
	ui->searchDirection->setCurrentIndex(preset["searchDirection"].toInt(0));
	ui->imageFileNameFormat->setText(preset["imagePathFormat"].toString("Downloads/{id}.{ext}"));
	ui->jsonFileNameFormat->setText(preset["jsonPathFormat"].toString("Json/{id}.json"));
	ui->saveJson->setChecked(preset["saveJson"].toBool(false));
	ui->updateJson->setChecked(preset["updateJson"].toBool(false));
	ui->includeComments->setChecked(preset["jsonComments"].toBool(false));
	ui->includeFavorites->setChecked(preset["jsonFavorites"].toBool(false));
	ui->limitImagesDownloadedCheck->setChecked(preset["limitImages"].toBool(false));
	ui->buttonGroupSVGOptions->button(preset["svgAction"].toInt(0))->setChecked(true);
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
		QJsonObject preset = exportPreset();
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

QJsonObject MainWindow::getCurrentPreset()
{
	QString presetName = ui->presetCombobox->currentText();
	return presets[presetName].toObject();
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

QJsonObject MainWindow::convertOldPresetArrayToObject(QJsonArray oldPresetArray) const
{
	// Used to transfer old settings that used arrays to a new format using objects
	
	QJsonObject newPresetObject;
	
	newPresetObject.insert("query", oldPresetArray[0]);
	newPresetObject.insert("startPage", oldPresetArray[1]);
	newPresetObject.insert("perPage", oldPresetArray[2]);
	newPresetObject.insert("imageLimit", oldPresetArray[3]);
	newPresetObject.insert("filter", oldPresetArray[4]);
	newPresetObject.insert("useCustomFilter", oldPresetArray[5]);
	newPresetObject.insert("customFilterID", oldPresetArray[6]);
	newPresetObject.insert("searchFormat", oldPresetArray[7]);
	newPresetObject.insert("searchDirection", oldPresetArray[8]);
	newPresetObject.insert("imagePathFormat", oldPresetArray[9]);
	newPresetObject.insert("jsonPathFormat", oldPresetArray[10]);
	newPresetObject.insert("saveJson", oldPresetArray[11]);
	newPresetObject.insert("updateJson", oldPresetArray[12]);
	newPresetObject.insert("jsonComments", oldPresetArray[13]);
	newPresetObject.insert("jsonFavorites", oldPresetArray[14]);
	newPresetObject.insert("limitImages", oldPresetArray[12]);

	return newPresetObject;
}

void MainWindow::reportError(QString errorMessage)
{
	QString time = QTime::currentTime().toString("HH:mm:ss");
	ui->errorLog->append("[" + time + "] - " + errorMessage);
}

void MainWindow::saveSettings()
{
	QSettings settings("DerpibooruDownloader.ini", QSettings::IniFormat);
	settings.setValue("currentOptions", QString(QJsonDocument(exportPreset()).toJson(QJsonDocument::Compact)));
	settings.setValue("showAdditionalInfo", ui->showAdditionalInfo->isChecked());
	settings.setValue("suppressWarnings", ui->suppressWarnings->isChecked());
	settings.setValue("apiKey", apiKey);
	
	QJsonObject tempPresets = presets;
	tempPresets.remove("-Default-");  // Do not save the default preset in the settings file
	settings.setValue("presets", QString(QJsonDocument(tempPresets).toJson(QJsonDocument::Compact)));
	
	settings.setValue("currentPreset", ui->presetCombobox->currentIndex());
	settings.setValue("windowGeometry", QString(saveGeometry().toBase64()));
}

void MainWindow::loadSettings()
{
	// Local .ini settings file
	QSettings settings("DerpibooruDownloader.ini", QSettings::IniFormat);
	
	// If there are settings stored in the .ini file
	if (settings.contains("windowGeometry")) {
		// Load settings
		
		QString settingCurrentOptions = settings.value("currentOptions", QString()).toString();
		if (!settingCurrentOptions.isEmpty())
		{
			QJsonDocument currentOptionsDoc = QJsonDocument::fromJson(settingCurrentOptions.toUtf8());
			
			// Convert the options to an object if the settings file is old, then import
			if (currentOptionsDoc.isArray())
				importPreset(convertOldPresetArrayToObject(currentOptionsDoc.array()));
			else
				importPreset(currentOptionsDoc.object());
		}
			
		apiKey = settings.value("apiKey", QString()).toString();
		
		QString settingPresets = settings.value("presets", QString()).toString();
		if (!settingPresets.isEmpty()) {
			// Add saved presets
			QJsonObject savedPresets = QJsonDocument::fromJson(settingPresets.toUtf8()).object();
			for (QString key : savedPresets.keys()) {
				
				// Prevent loading the default preset from old settings files
				if (key != "-Default-") {
					// Convert the preset to an object if the settings file is old, then add it to the preset list
					if (savedPresets[key].isArray())
						presets.insert(key, convertOldPresetArrayToObject(savedPresets[key].toArray()));
					else
						presets.insert(key, savedPresets[key]);
				}
				
			}
		}
		// Add default preset
		presets["-Default-"] = manager->getDefaultPreset();
		
		updatePresetCombobox();
		
		ui->showAdditionalInfo->setChecked(settings.value("showAdditionalInfo", false).toBool());
		ui->presetCombobox->setCurrentIndex(settings.value("currentPreset", 0).toInt());
		ui->suppressWarnings->setChecked(settings.value("suppressWarnings", false).toBool());
		
		QString settingGeometry = settings.value("windowGeometry", QString()).toString();
		if (!settingGeometry.isEmpty())
			restoreGeometry(QByteArray::fromBase64(settingGeometry.toUtf8()));
	} else {
		// There are no settings stored in the .ini file.
		// Check if there are settings stored using the old method, in the default platform location
		QSettings oldSettings("Sibusten", "DerpibooruArchiver");
		
		if (oldSettings.contains("geometry")) {
			// Load old settings
			
			QString settingsString = oldSettings.value("mainSettings", "").toString();
		
			//If options are saved, load them
			if(!settingsString.isEmpty())
			{
				QJsonObject settingsObj = decodeJson(settingsString).object();
				importPreset(convertOldPresetArrayToObject(settingsObj["currentOptions"].toArray()));
				apiKey = settingsObj["apiKey"].toString();
				
				QJsonObject savedPresets = settingsObj["presets"].toObject();
				for (QString key : savedPresets.keys()) {
					// Convert the preset to an object and add it to the preset list. All settings in this location are old and using arrays.
					presets.insert(key, convertOldPresetArrayToObject(savedPresets[key].toArray()));
				}
				
				updatePresetCombobox();
				ui->showAdditionalInfo->setChecked(settingsObj["showAdditional"].toBool());
				ui->presetCombobox->setCurrentIndex(settingsObj["currentPreset"].toInt());
				ui->suppressWarnings->setChecked(settingsObj["suppressWarnings"].toBool());
			}
			
			restoreGeometry(oldSettings.value("geometry").toByteArray());
			
			// Now that the old settings are loaded, save them in the new format
			saveSettings();
		}
	}
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
		if (json.isArray())
			importPreset(convertOldPresetArrayToObject(json.array()));
		else
			importPreset(json.object());
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
	
	manager->start(getSearchSettings(), ui->imageFileNameFormat->text(), limitImagesDownloaded, ui->saveJson->isChecked(),
				   ui->updateJson->isChecked(), ui->jsonFileNameFormat->text(), static_cast<DownloadManager::SVGMode>(ui->buttonGroupSVGOptions->checkedId()));
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

DerpiJson::SearchSettings MainWindow::getSearchSettings()
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
