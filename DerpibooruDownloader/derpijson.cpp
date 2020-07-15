#include "derpijson.h"

QVector<DerpiJson*> DerpiJson::splitArray(QJsonArray jsonArray) {
	QVector<DerpiJson*> images;

	// Iterate through the array and create DerpiJson objects for each entry
	for (int i = 0; i < jsonArray.size(); i++) {
		images.append(new DerpiJson(jsonArray.at(i).toObject()));
	}

	return images;
}

/*
 * A function to create a url to retrieve image metadata based on search constraints.
 *
 * query:				The search string
 * page:				The page to get. Default: 1
 * perPage:				The number of images per page. Maximum of 50. Default: 15
 * showComments:		Whether to include comments in the metadata. Default: false
 * showFavorites:		Whether to include favorites in the metadata. Default: false
 * searchFormat:		Defines the main method of sorting matching images. Valid options are CreationDate, Score, Relevance, Width, Height, Comments, Random. Default: CreationDate
 *						Note that there is no (known) way to preserve the order of images when using Random, so only one page can be guaranteed to be truly random.
 * searchDirection:		The direction to sort images. Options are Desc and Asc. Default: Desc
 * apiKey:				The user's api key. Needed to use constraints such as faves, upvotes, uploads, and watched. Also affects the filter used when getting images. Default: none
 *
 * filterId:			The id of the filter to be used when searching. -1 uses default filter (or current user filter if a key is provided) Default: -1
 *						Filter ids can be found by examining the url of the filter page. Ex: https://derpibooru.org/filters/100073, where 100073 is the id of the default filter.
 *
 * NOTE: as of February 2017, search by faves/upvotes/uploads/watched, and other options are only done from the search query, not by individual html parameters.
 */
QUrl DerpiJson::getSearchUrl(DerpiJson::SearchSettings settings) {
	// Convenience arrays to convert enums into their proper string codes
	QString searchFormats[] = {"created_at", "score", "_score", "width", "height", "comment_count", "random"};
	QString searchDirections[] = {"desc", "asc"};

	//Spaces are replaced with + in search string
	QString temp = settings.query.replace(" ", "+");

	// Prefer to use id constraints over paging when possible
	if (settings.searchFormat == SearchFormat::CreationDate && settings.lastIdFound != -1)
	{
		// Use less than for descending order, and greater than for ascending order
		QString comparisonString = (settings.searchDirection == SearchDirection::Desc ? "lt" : "gt");

		temp = QString("(%1),id.%2:%3").arg(temp, comparisonString, QString::number(settings.lastIdFound));
	}
	else {
		temp += "&page=" + QString::number(settings.page);
	}

	temp = "https://www.derpibooru.org/api/v1/json/search/images?q=" + temp;

	temp += "&per_page=" + QString::number(settings.perPage);

	if (settings.showComments) {
		temp += "&comments=";
	}

	if (settings.showFavorites) {
		temp += "&fav=";
	}

	temp += "&sf=" + searchFormats[settings.searchFormat];

	// If random order is chosen, add the provided random seed to the search.
	// This will allow multiple pages of random images to be downloaded without fear of duplicates.
	if (settings.searchFormat == Random) {
		temp += ":" + QString::number(settings.random_seed);
	}

	temp += "&sd=" + searchDirections[settings.searchDirection];

	if (!settings.apiKey.isEmpty()) {
		temp += "&key=" + settings.apiKey;
	}

	if(settings.filterId != -1) {
		temp += "&filter_id=" + QString::number(settings.filterId);
	}

	// qDebug() << temp;

	return temp;
}

DerpiJson::DerpiJson(QByteArray jsonData, QObject *parent) : QObject(parent) {
	this->json = QJsonDocument::fromJson(jsonData);
}

DerpiJson::DerpiJson(QJsonObject jsonObject, QObject* parent) : QObject(parent) {
	this->json = QJsonDocument(jsonObject);
}

int DerpiJson::getId() {
	// July 30 2016: changed from id_number to id and became a string for some reason...
	// February 25, 2018: changed from a string back to an int...
	//   Going to add a check so this shouldn't need to be changed again
	if (json.object()["id"].isString()) {
		return json.object()["id"].toString().toInt();
	}
	else if (json.object()["id"].isDouble()) {  // Also refers to ints
		return json.object()["id"].toInt();
	}
	else {
		return -1;  // Something went wrong, and the ID can't be identified
	}
}

QUrl DerpiJson::getDownloadUrl(bool getSVG) {
	QString url_string = json.object()["representations"].toObject()["full"].toString();

	// If this image is an SVG, and the .svg file is requested
	if (getSVG && getFormat() == "svg") {
		// Change the extension to get the actual svg file, instead of the rasterized png
		url_string = url_string.left(url_string.lastIndexOf(".")) + ".svg";
	}

	return QUrl(url_string);
}

QUrl DerpiJson::getViewUrl() {
	return QUrl(json.object()["view_url"].toString());
}

QString DerpiJson::getName() {
	QString name = getViewUrl().fileName();
	return name.left(name.lastIndexOf("."));
}

QString DerpiJson::getOriginalName() {
	QString name = json.object()["name"].toString();
	return name.left(name.lastIndexOf("."));
}

QString DerpiJson::getUploader() {
	return json.object()["uploader"].toString();
}

QString DerpiJson::getFormat() {
	return json.object()["format"].toString();
}

QString DerpiJson::getSha512Hash() {
	return json.object()["sha512_hash"].toString();
}

QDateTime DerpiJson::getCreationDate() {
	QString dateString = json.object()["created_at"].toString();
	return QDateTime::fromString(dateString, Qt::ISODate);
}

int DerpiJson::getYear() {
	return getCreationDate().date().year();
}

int DerpiJson::getMonth() {
	return getCreationDate().date().month();
}

int DerpiJson::getDay() {
	return getCreationDate().date().day();
}

int DerpiJson::getScore() {
	return json.object()["score"].toInt();
}

int DerpiJson::getUpvotes() {
	return json.object()["upvotes"].toInt();
}

int DerpiJson::getDownvotes() {
	return json.object()["downvotes"].toInt();
}

int DerpiJson::getFaves() {
	return json.object()["faves"].toInt();
}

int DerpiJson::getComments() {
	return json.object()["comment_count"].toInt();
}

int DerpiJson::getWidth() {
	return json.object()["width"].toInt();
}

int DerpiJson::getHeight() {
	return json.object()["height"].toInt();
}

double DerpiJson::getAspectRatio() {
	return json.object()["aspect_ratio"].toDouble();
}

QStringList DerpiJson::getTags() {
	QStringList tags;

	QJsonArray tagsJsonArray = json.object()["tags"].toArray();

	// Convert all tag json values to strings
	for (QJsonValue tagVal : tagsJsonArray) {
		tags.append(tagVal.toString());
	}

	return tags;
}

QJsonDocument DerpiJson::getJson() {
	return json;
}

bool DerpiJson::isRendered() {
	return json.object()["thumbnails_generated"].toBool();
}

bool DerpiJson::isOptimized() {
	return json.object()["processed"].toBool();
}

DerpiJson::SearchSettings::SearchSettings(QString query, int page, int perPage, bool showComments, bool showFavorites, int searchFormat,
		int searchDirection, QString apiKey, int filterId, int random_seed, int lastIdDownloaded) {
	this->query = query;
	this->page = page;
	this->perPage = perPage;
	this->showComments = showComments;
	this->showFavorites = showFavorites;
	this->searchFormat = searchFormat;
	this->searchDirection = searchDirection;
	this->apiKey = apiKey;
	this->filterId = filterId;
	this->random_seed = random_seed;
	this->lastIdFound = lastIdDownloaded;
}
