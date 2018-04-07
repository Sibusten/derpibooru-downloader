#-------------------------------------------------
#
# Project created by QtCreator 2016-03-26T16:44:06
#
#-------------------------------------------------

QT       += core gui network

VERSION = 1.3.5
DEFINES += APP_VERSION=\\\"$$VERSION\\\"

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

TARGET = DerpibooruDownloader
TEMPLATE = app


SOURCES += main.cpp\
        mainwindow.cpp \
    downloader.cpp \
    downloadmanager.cpp \
    derpijson.cpp \
    timehelper.cpp

HEADERS  += mainwindow.h \
    downloader.h \
    downloadmanager.h \
    derpijson.h \
    timehelper.h

FORMS    += mainwindow.ui

win32: RC_ICONS = "icon.ico"

RESOURCES += \
    resources.qrc
