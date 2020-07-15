#ifndef DERPIJSON_H
#define DERPIJSON_H

#include <QObject>

#include <QVector>

#include <QJsonDocument>
#include <QJsonObject>
#include <QJsonArray>
#include <QJsonValue>

#include <QDateTime>

#include <QUrl>

/*
    Site Filters:
    Everything:          56027
    18+ R34:             37432
    18+ Dark:            37429
    Default:             100073
    Maximum Spoilers:    37430
    Legacy Default:      37431
*/

class DerpiJson : public QObject {
    Q_OBJECT
public:
    enum FlagType {
        Ignore,
        Only,
        Not
    };

    enum SearchFormat {
        CreationDate,
        Score,
        Relevance,
        Width,
        Height,
        Comments,
        Random
    };

    enum SearchDirection {
        Desc,
        Asc
    };

    class SearchSettings {
    public:
        SearchSettings(QString query, int page = 1, int perPage = 15, bool showComments = false, bool showFavorites = false,
                int searchFormat = CreationDate, int searchDirection = Desc, QString apiKey = 0, int filterId = -1,
                int random_seed = 0, QString booruUrl = DEFAULT_BOORU, int lastIdFound = -1);
        QString query;
        int page;
        int perPage;
        bool showComments;
        bool showFavorites;
        int searchFormat;
        int searchDirection;
        QString apiKey;
        int filterId;
        int random_seed;
        QString booruUrl;
        int lastIdFound;
    };

    const static QString DEFAULT_BOORU;

    static QVector<DerpiJson*> splitArray(QJsonArray jsonArray);

    static QUrl getSearchUrl(QString query, int page = 1, int perPage = 15, bool showComments = false, bool showFavorites = false,
            int searchFormat = CreationDate, int searchDirection = Desc, QString apiKey = 0, int filterId = -1,
            int random_seed = 0);
    static QUrl getSearchUrl(SearchSettings settings);

    explicit DerpiJson(QByteArray jsonData, QObject *parent = 0);
    explicit DerpiJson(QJsonObject jsonObject, QObject *parent = 0);

    int getId();
    QUrl getDownloadUrl(bool getSVG = true);
    QUrl getViewUrl();
    QString getName();
    QString getOriginalName();
    QString getUploader();
    QString getFormat();
    QString getSha512Hash();

    QDateTime getCreationDate();
    int getYear();
    int getMonth();
    int getDay();

    int getScore();
    int getUpvotes();
    int getDownvotes();
    int getFaves();
    int getComments();

    int getWidth();
    int getHeight();
    double getAspectRatio();

    QStringList getTags();

    QJsonDocument getJson();

    bool isRendered();
    bool isOptimized();

private:
    QJsonDocument json;
};

#endif // DERPIJSON_H
