#include "mainwindow.h"
#include <QObject>
#include <QApplication>

#include <QNetworkAccessManager>

#include "downloadmanager.h"

int main(int argc, char *argv[])
{
	QApplication a(argc, argv);
	a.setApplicationVersion(APP_VERSION);
	QNetworkAccessManager* netManager = new QNetworkAccessManager();
	DownloadManager manager(netManager);
	
	MainWindow w(&manager);
	w.show();
	
	return a.exec();
}
