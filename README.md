# Derpibooru Downloader
Downloads images from derpibooru.org using a variety of options

## Screenshots
![Main Window](/Screenshots/MainWindow.png?raw=true)
![Main Window](/Screenshots/Running.png?raw=true)

## Features
* Download any search that can be made on the site
  * View the "Search terms" on the [Search Page](https://derpibooru.org/search) and the advanced [Search Help]() page for all of the queries you can make
* Skips images that are already downloaded
  * Save yourself and the server bandwidth by only downloading what's needed. If a file were to have the same name as an existing one, it is not downloaded.
* Choose how files will be named and where they will be saved
  * Use special tags in the file naming scheme to save them how you want. See the section on naming tags for more info
* Limit the number of images downloaded
  * Don't want all 36,000 Derpy images? Limit your search to only the first *x* results. Good if you, say, want to only grab the top 100 scoring
* Easilly save search presets and load them later
  * Useful if you have a search you want to update frequently. Works well with the image skipping: only newly found images will actually be downloaded.
* Get an estimate of how long the download will take, as well as track it's progress
  * View the progress of the current download, and the search in whole
* Export presets to save or share with others
  * Generates a short string that can be imported to use the same settings. <sub>(Likely to not work over different versions of the program. You could, of course, simply share the search query since pretty much everything is done through that now)</sub>
* Save image json data
  * If you would like to save the raw image data for some reason, you are able to. Uses the same file naming tags as images, except for the extension, which has to be ```.json```. Options are there for you to include full comments and favorite lists for each image, but as the api page states: *"Only enable this if you really need it"*

## Installation
Head over to the [Releases](../../releases) page to download the compiled binary (Windows only), or build it yourself by downloading the source or cloning with git.

The binary was built using Qt 5.3.0, and MinGW 4.8.2 32bit

## File Naming Tags
The field "Save Images As" (as well as "Save Json As") allow for special tags to be used that will be filled with information unique to each image. The image [derpibooru.org/8584](https://derpibooru.org/8584) will be used as a reference

* ```{id}``` -> 8584
  * The id of the image on the site
* ```{name}``` -> 8584__safe_solo_derpy+hooves_parody_artist-colon-kloudmutt_not+a+clever+pony_buttersafe
  * The full name of the file, without extension
* ```{original_name}``` -> 8584__safe_derpy+hooves_artist-colon-kloudmutt_not+a+clever+pony
  * The original file name of the uploaded image, without extension. ***Note: This is not guaranteed to exist for every image, and may cause issues or crashes. Not recommended to use***
* ```{ext}``` -> jpg
  * The extension of the image. Required if you want your downloads to be the proper file type
* ```{year}``` -> 2012, ```{month}``` -> 06, ```{day}``` -> 18
  * The year, month, and day that the image was first uploaded
* ```{width}``` -> 700, ```{height}``` -> 700, ```{aspect_ratio}``` -> 1.0
  * Various image size properties
* ```{score}``` -> 70, ```{upvotes}``` -> 72, ```{downvotes}``` -> 2, ```{comments}``` -> 4
  * Miscellaneous information about the image ***Note that these tags are not guaranteed to be unique for each download, and may change at any time. This will cause issues with images being downloaded multiple times as the file names will not match. Provided only to be thorough, not reccommended for use in most cases***
* ```{#}``` -> 8584, ```{##}``` -> 8580, ```{###}``` -> 8500, ```{####)``` -> 8000, ```{#####}``` -> 0, ...
  * Any number of '#' symbols will work. Takes the image id and floors it to the specified number of places. (If four #'s are used, anything before the thousands place is ignored.) For example, if you wanted to group the results into folders based on their id, and have a max of 1000 images per folder (ids 0-999 in the first, ids 1000-1999 in the second, and so on), you could use the following: ```Downloads\{####}\{id}.{ext}``` Files could then be saved as follows:
    * ```Downloads\0\100.jpg```
    * ```Downloads\1000\1234.png```
    * ```Downloads\8000\8584.jpg```
