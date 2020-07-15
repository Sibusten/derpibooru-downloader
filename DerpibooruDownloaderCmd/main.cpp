#include <QCoreApplication>
#include <QCommandLineParser>
#include <QNetworkAccessManager>
#include <QSettings>

#include "downloadmanager.h"
#include "derpiboorudownloadercmd.h"

/**
 * @brief convertToInt Converts a value to an int, reporting and exiting on failure
 * @param value The value to convert
 * @param name The name of this value
 * @return The converted value
 */
int convertToInt(QString value, QString name)
{
    bool conversionCheck;
    int intVal = value.toInt(&conversionCheck);
    if (!conversionCheck) {
        QTextStream(stderr) << QString("Could not convert value for %1 to an int! Value was '%2'").arg(name, value) << endl;
        exit(1);
    }

    return intVal;
}

int main(int argc, char *argv[])
{
    QTextStream qStdOut(stdout);
    QTextStream qStdErr(stderr);

    QCoreApplication a(argc, argv);
    a.setApplicationName("Derpibooru Downloader");
    a.setApplicationVersion(APP_VERSION);

    QCommandLineParser parser;
    parser.setApplicationDescription("Downloads images from derpibooru.org");
    parser.addHelpOption();
    parser.addVersionOption();

    parser.addOptions({
        /*{
            {"O", "log-level"},
            QCoreApplication::translate("main", "How much information to log to console. One of (silent, normal, verbose)"),
            "log-level"
        }, */{
            {"p", "preset"},
            QCoreApplication::translate("main", "What preset to use as a base for this search. Must be the exact name of a preset defined in the GUI app. "
            "If no preset is given, the default preset will be used. All other arguments will override the settings in the given preset."),
            "preset"
        }, {
            {"q", "query"},
            QCoreApplication::translate("main", "Search query to use for downloading."),
            "query"
        }, {
            {"s", "start-page"},
            QCoreApplication::translate("main", "Page to start downloading from. Image offset depends on the number of images per page."),
            "start-page"
        }, {
            {"P", "per-page"},
            QCoreApplication::translate("main", "How many images should be on each page (1 - 50)."),
            "per-page"
        }, {
            {"l", "limit"},
            QCoreApplication::translate("main", "How many images to download (defaults to all in search)."),
            "limit"
        }, {
            {"f", "filter"},
            QCoreApplication::translate("main", "Filter ID to use when downloading. Does not work if an API key is given."),
            "filter"
        }, {
            {"S", "search-format"},
            QCoreApplication::translate("main", "How to sort images in the search. One of (creationdate, score, relevance, width, height, comments, random)."),
            "search-format"
        }, {
            {"D", "search-direction"},
            QCoreApplication::translate("main", "The direction to sort images in the search. One of (ascending, descending)."),
            "search-direction"
        }, {
            {"I", "image-path"},
            QCoreApplication::translate("main", "Where to save images and how to name them."), // TODO explain path tags
            "image-path"
        }, {
            {"J", "json-path"},
            QCoreApplication::translate("main", "Where to save json files and how to name them."),
            "json-path"
        }, {
            {"j", "save-json"},
            QCoreApplication::translate("main", "Enables saving json files.")
        }, {
            {"u", "update-json"},
            QCoreApplication::translate("main", "Enables overwritting old json files with new data.")
        }, {
            {"x", "json-only"},
            QCoreApplication::translate("main", "Disables image downloading. Only json will be saved if set to do so.")
        }, {
            {"C", "json-comments"},
            QCoreApplication::translate("main", "Enables fetching comments in the json data.")
        }, {
            {"F", "json-faves"},
            QCoreApplication::translate("main", "Enables fetching favorites in the json data.")
        }, {
            {"g", "svg-mode"},
            QCoreApplication::translate("main", "Whether to save .svg, .png, or both for SVG images. One of (svg, png, both)."),
            "svg-mode"
        }, {
            {"a", "api-key"},
            QCoreApplication::translate("main", "API key to use for this search."),
            "api-key"
        }, {
            {"A", "use-saved-api-key"},
            QCoreApplication::translate("main", "The program will use the saved API key instead.")
        }, {
            {"b", "booru-url"},
            QCoreApplication::translate("main", "The booru to download from. If not specified, uses the saved booru. Should be a url such as https://derpibooru.org/"),
            "booru-url"
        }
    });

    // Process arguments
    parser.process(a);

    // Load program settings
    QSettings settings(QCoreApplication::applicationDirPath() + "/DerpibooruDownloader.ini", QSettings::IniFormat);

    // If there are no settings stored in the .ini file
    if (!settings.contains("windowGeometry")) { // TODO generate settings in the cmd version as well. Also change this check because window geometry is only relevant to the gui version
        qStdErr << "The settings file appears to be empty! Please run the gui version of the program to generate the settings" << endl;
        exit(1);
    }

    // Get the API key. Uses the saved key if the flag is set. If not, uses the provided key. If no key is provided, it will not be used.
    QString apiKey;
    if (parser.isSet("use-saved-api-key")) {
        apiKey = settings.value("apiKey", QString()).toString();
        if (apiKey.isEmpty()) {
            qStdOut << "Warning: No API key is saved!" << endl;
        }
    }
    else if (parser.isSet("api-key")) {
        QString tempApiKey = parser.value("api-key");
        if (!(tempApiKey.length() == 20)) {
            qStdErr << QString("API Key length invalid. Expected 20, got (") + QString::number(tempApiKey.length()) + ")" << endl;
            exit(1);
        }
        else {
            apiKey = tempApiKey;
        }
    }

    // Get the booru url. Uses the saved url if the booru flag is not set.
    QString booruUrl;
    if (parser.isSet("booru-url")) {
        // TODO: validation?
        booruUrl = parser.value("booru-url");
    }
    else {
        booruUrl = settings.value("booru-url", DerpiJson::DEFAULT_BOORU).toString();
    }

    QJsonObject basePreset;
    if (parser.isSet("preset")) {
        // Attempt to load the given preset
        QString tempPresetName = parser.value("preset");

        // Get saved presets
        QString settingPresets = settings.value("presets", QString()).toString();
        if (settingPresets.isEmpty()) {
            qStdErr << "Error loading presets from the settings file!" << endl;
            exit(1);
        }
        QJsonObject savedPresets = QJsonDocument::fromJson(settingPresets.toUtf8()).object();

        // Check if the given preset exists
        if (!savedPresets.contains(tempPresetName)) {
            qStdErr << QString("Preset '%1' does not exist! Check that the name is spelled exactly as it was saved in the GUI app.").arg(tempPresetName) << endl;
            exit(1);
        }

        // Load the preset
        basePreset = savedPresets.value(tempPresetName).toObject();
    }
    else {
        // Use the default preset
        basePreset = DownloadManager::getDefaultPreset();
    }

    /*
    QString logLevel = "normal";
    if (parser.isSet("log-level")) {
        QString tempLogLevel = parser.value("log-level").toLower();
        if (QStringList({"silent", "normal", "verbose"}).contains(tempLogLevel)) {
            logLevel = tempLogLevel;
        } else {
            qStdErr << QString("Invalid log-level given (%1)").arg(tempLogLevel) << endl;
            exit(1);
        }
    }
    */

    if (parser.isSet("query")) {
        basePreset.insert("query", parser.value("query"));
    }
    if (parser.isSet("start-page")) {
        basePreset.insert("startPage", convertToInt(parser.value("start-page"), "start-page"));
    }
    if (parser.isSet("per-page")) {
        basePreset.insert("perPage", convertToInt(parser.value("per-page"), "per-page"));
    }
    if (parser.isSet("limit")) {
        basePreset.insert("imageLimit", convertToInt(parser.value("limit"), "limit"));
        basePreset.insert("limitImages", true);
    }
    if (parser.isSet("filter")) {
        basePreset.insert("customFilterID", convertToInt(parser.value("filter"), "filter"));
        basePreset.insert("useCustomFilter", true);
    }
    if (parser.isSet("search-format")) {
        QStringList searchFormats = {"creationdate", "score", "relevance", "width", "height", "comments", "random"};
        int searchFormat = searchFormats.indexOf(parser.value("search-format").toLower());

        if (searchFormat == -1) {
            qStdErr << QString("Invalid search-format: '%1'").arg(parser.value("search-format")) << endl;
            exit(1);
        }

        basePreset.insert("searchFormat", searchFormat);
    }
    if (parser.isSet("search-direction")) {
        QStringList searchDirections = {"descending", "ascending"};
        int searchDirection = searchDirections.indexOf(parser.value("search-direction").toLower());

        if (searchDirection == -1) {
            qStdErr << QString("Invalid search-direction: '%1'").arg(parser.value("search-direction")) << endl;
            exit(1);
        }

        basePreset.insert("searchDirection", searchDirection);
    }
    if (parser.isSet("image-path")) {
        basePreset.insert("imagePathFormat", parser.value("image-path"));
    }
    if (parser.isSet("json-path")) {
        basePreset.insert("jsonPathFormat", parser.value("json-path"));
    }
    if (parser.isSet("save-json")) {
        basePreset.insert("saveJson", true);
    }
    if (parser.isSet("update-json")) {
        basePreset.insert("updateJson", true);
    }
    if (parser.isSet("json-only")) {
        basePreset.insert("jsonOnly", true);
    }
    if (parser.isSet("json-comments")) {
        basePreset.insert("jsonComments", true);
    }
    if (parser.isSet("json-faves")) {
        basePreset.insert("jsonFavorites", true);
    }
    if (parser.isSet("svg-mode")) {
        QStringList svgModes = {"svg", "png", "both"};
        int svgMode = svgModes.indexOf(parser.value("svg-mode").toLower());

        if (svgMode == -1) {
            qStdErr << QString("Invalid svg-mode: '%1'").arg(parser.value("svg-mode")) << endl;
            exit(1);
        }

        basePreset.insert("svgAction", svgMode);
    }

    // Post-process the preset options

    // Set query to "*" if it is empty
    if (basePreset.value("query").toString().isEmpty()) {
        basePreset.insert("query", "*");
    }

    // Set max images to -1 if no limit is set
    if (!basePreset.value("limitImages").toBool()) {
        basePreset.insert("imageLimit", -1);
    }

    // Get the true filter ID for this download
    int filterID;
    if (basePreset.value("useCustomFilter").toBool()) {
        // Use custom filter ID if set
        filterID = basePreset.value("customFilterID").toInt();
    } else {
        // Otherwise, use the filter variable that is set in the preset. (This is an index for the actual filter ID)

        //-User Default-, Everything, 18+ R34, 18+ Dark, Default, Maximum Spoilers, Legacy Default
        int defaultFilterIds[] = {-1, 56027, 37432, 37429, 100073, 37430, 37431};
        filterID = defaultFilterIds[basePreset.value("filter").toInt()];
    }

    // Set up the download manager and cmd downloader
    QNetworkAccessManager* netManager = new QNetworkAccessManager();
    DownloadManager manager(netManager);
    DerpibooruDownloaderCmd dlcmd(&manager);

    // Gather data and start the download
    DerpiJson::SearchSettings searchSettings(
        basePreset.value("query").toString(), basePreset.value("startPage").toInt(),
        basePreset.value("perPage").toInt(), basePreset.value("jsonComments").toBool(),
        basePreset.value("jsonFaves").toBool(), basePreset.value("searchFormat").toInt(),
        basePreset.value("searchDirection").toInt(), apiKey, filterID,
        qrand(), booruUrl
    );

    dlcmd.start(
        searchSettings, basePreset.value("imagePathFormat").toString(), basePreset.value("imageLimit").toInt(),
        basePreset.value("saveJson").toBool(), basePreset.value("updateJson").toBool(),
        basePreset.value("jsonPathFormat").toString(), static_cast<DownloadManager::SVGMode>(basePreset.value("svgAction").toInt()),
        basePreset.value("jsonOnly").toBool()
    );

    return a.exec();
}
