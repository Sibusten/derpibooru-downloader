TEMPLATE = subdirs

SUBDIRS += \
    DerpibooruDownloaderGui \
    DerpibooruDownloader \
    DerpibooruDownloaderCmd

DerpibooruDownloaderGui.depends = DerpibooruDownloader
DerpibooruDownloaderCmd.depends = DerpibooruDownloader
