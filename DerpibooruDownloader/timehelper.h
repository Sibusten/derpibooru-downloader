#ifndef TIMEHELPER_H
#define TIMEHELPER_H

#include <QObject>

#include <QVector>

#include <QElapsedTimer>

/*
 * A helper class for keeping track of elapsed time, progress per legnth of time, and expected completion time
 *
 */
class TimeHelper : public QObject {
	Q_OBJECT
public:
	/*
	 * The method of calculating the amount per time.
	 * Example of Difference would be number of downloads completed. It would use the last entry minus the first entry to calculate how many downloads happened in that timeframe.
	 * Example of Average would be download speed. It would average all of the entries to calculate the average download speed in that timeframe.
	 *
	 */
	enum DataType {
		Difference,
		Average
	};

	explicit TimeHelper(int maxSamples, DataType dataType, QObject *parent = 0);

	bool hasReading(int samplesUntilReading);  // Checks whether enough samples have been taken to get an accurate reading.
	int getPerSecond();
	int getPerMinute();
	int getPerHour();

	qint64 getElapsedTime();
	QString getElapsedTimeFormatted();

	qint64 getETA(int amount);
	QString getETAFormatted(int amount);

	void update(int value);

public slots:
	void start();
	void restart();

public:
	QElapsedTimer timer;			// Timer to calculate elapsed time
	QVector<int> samples;			// Vector to store samples. Each entry represents one second.

	int maxSamples;					// Number of samples to keep. Each sample represents one second.
	DataType dataType;				// Determines how the samples are treated.

	double perSecond();
	QString format(qint64 time);	// Formats the time in miliseconds to HH:MM:SS
};

#endif // TIMEHELPER_H
