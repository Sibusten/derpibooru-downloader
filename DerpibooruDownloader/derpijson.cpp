#include "derpijson.h"

#include <QDebug>

QVector<DerpiJson*> DerpiJson::splitArray(QJsonArray jsonArray)
{
	QVector<DerpiJson*> images;
	
	//Iterate through the array and create DerpiJson objects for each entry
	for(int i = 0; i < jsonArray.size(); i++)
	{
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
QUrl DerpiJson::getSearchUrl(QString query, int page, int perPage, bool showComments, bool showFavorites, 
							 int searchFormat, int searchDirection, QString apiKey, int filterId, int random_seed)
{
	//Convenience arrays to convert enums into their proper string codes
	QString searchFormats[] = {"created_at", "score", "relevance", "width", "height", "comments", "random"};
	QString searchDirections[] = {"desc", "asc"};
	
	//Spaces are replaced with + in search string
	QString temp = query.replace(" ", "+");
	
	temp = "https://derpibooru.org/search.json?q=" + temp;
	temp += "&page=" + QString::number(page);
	temp += "&perpage=" + QString::number(perPage);
	if(showComments) temp += "&comments=";
	if(showFavorites) temp += "&fav=";
	temp += "&sf=" + searchFormats[searchFormat];
	
	// If random order is chosen, add the provided random seed to the search.
	// This will allow multiple pages of random images to be downloaded without fear of duplicates.
	if(searchFormat == Random)
	{
		temp += ":" + QString::number(random_seed);
	}
	
	temp += "&sd=" + searchDirections[searchDirection];
	if(!apiKey.isEmpty())
	{
		temp += "&key=" + apiKey;
	}
	
	if(filterId != -1) temp += "&filter_id=" + QString::number(filterId);
	
	// qDebug() << temp;
	
	return temp;
}

/*
 * Convenience function that uses the settings object to generate a url
 * 
 */
QUrl DerpiJson::getSearchUrl(DerpiJson::SearchSettings settings)
{
	return getSearchUrl(settings.query, settings.page, settings.perPage, settings.showComments, settings.showFavorites, 
						settings.searchFormat, settings.searchDirection, settings.apiKey, settings.filterId, settings.random_seed);
}

DerpiJson::DerpiJson(QByteArray jsonData, QObject *parent) : QObject(parent)
{
	this->json = QJsonDocument::fromJson(jsonData);
}

DerpiJson::DerpiJson(QJsonObject jsonObject, QObject* parent) : QObject(parent)
{
	this->json = QJsonDocument(jsonObject);
}

int DerpiJson::getId()
{
	// July 30 2016: changed from id_number to id and became a string for some reason...
	// February 25, 2018: changed from a string back to an int...
	//   Going to add a check so this shouldn't need to be changed again
	if (json.object()["id"].isString())
		return json.object()["id"].toString().toInt();
	else if (json.object()["id"].isDouble())  // Also refers to ints
		return json.object()["id"].toInt();
	else
		return -1;  // Something went wrong, and the ID can't be identified
}

QUrl DerpiJson::getImageUrl(bool getSVG)
{
	QString url_string = QString("https:") + json.object()["image"].toString();  // image link begins with '//'
	
	// If this image is an SVG, and the .svg file is requested
	if (getSVG && getFormat() == "svg") {
		// Change the extension to get the actual svg file, instead of the rasterized png
		url_string = url_string.left(url_string.lastIndexOf(".")) + ".svg";
	}
	
	return QUrl(url_string);
}

QString DerpiJson::getName()
{
	QString name = getImageUrl().fileName();
	return name.left(name.lastIndexOf("."));
}

QString DerpiJson::getOriginalName()
{
	QString name = json.object()["file_name"].toString();
	return name.left(name.lastIndexOf("."));
}

QString DerpiJson::getUploader()
{
	return json.object()["uploader"].toString();
}

QString DerpiJson::getFormat()
{
	return json.object()["original_format"].toString();
}

QString DerpiJson::getSha512Hash()
{
	return json.object()["sha512_hash"].toString();
}

QDateTime DerpiJson::getCreationDate()
{
	QString dateString = json.object()["created_at"].toString();
	return QDateTime::fromString(dateString, Qt::ISODate);
}

int DerpiJson::getYear()
{
	return getCreationDate().date().year();
}

int DerpiJson::getMonth()
{
	return getCreationDate().date().month();
}

int DerpiJson::getDay()
{
	return getCreationDate().date().day();
}

int DerpiJson::getScore()
{
	return json.object()["score"].toInt();
}

int DerpiJson::getUpvotes()
{
	return json.object()["upvotes"].toInt();
}

int DerpiJson::getDownvotes()
{
	return json.object()["downvotes"].toInt();
}

int DerpiJson::getFaves()
{
	return json.object()["faves"].toInt();
}

int DerpiJson::getComments()
{
	return json.object()["comments"].toInt();
}

int DerpiJson::getWidth()
{
	return json.object()["width"].toInt();
}

int DerpiJson::getHeight()
{
	return json.object()["height"].toInt();
}

int DerpiJson::getAspectRatio()
{
	return json.object()["aspect_ratio"].toInt();
}

QStringList DerpiJson::getTags()
{
	// Get the string of comma-separated tags
	QString tagsString = json.object()["tags"].toString();
	
	// Split the string and return the list
	return tagsString.split(", ");
}

QJsonDocument DerpiJson::getJson()
{
	return json;
}

bool DerpiJson::isRendered()
{
	return json.object()["is_rendered"].toBool();
}

bool DerpiJson::isOptimized()
{
	return json.object()["is_optimized"].toBool();
}

DerpiJson::SearchSettings::SearchSettings(QString query, int page, int perPage, bool showComments, bool showFavorites,
										  int searchFormat, int searchDirection, QString apiKey, int filterId, int random_seed)
{
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
}
