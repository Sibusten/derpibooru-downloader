#include "timehelper.h"

TimeHelper::TimeHelper(int maxSamples, DataType dataType, QObject *parent) : QObject(parent) {
	this->maxSamples = maxSamples;
	this->dataType = dataType;

	timer.start();
}

bool TimeHelper::hasReading(int samplesUntilReading) {
	return samples.size() >= samplesUntilReading;
}

int TimeHelper::getPerSecond() {
	return perSecond();
}

int TimeHelper::getPerMinute() {
	return perSecond() * 60;
}

int TimeHelper::getPerHour() {
	return perSecond() * 3600;
}

qint64 TimeHelper::getElapsedTime() {
	return timer.elapsed();
}

QString TimeHelper::getElapsedTimeFormatted() {
	return format(timer.elapsed());
}

qint64 TimeHelper::getETA(int amount) {
	double perSec = perSecond();
	if (qRound(perSec) == 0) {
		return -1;
	}
	qint64 millisToCompletion = 1000 * (amount / perSec);
	return millisToCompletion;
}

QString TimeHelper::getETAFormatted(int amount) {
	qint64 eta = getETA(amount);
	if(eta == -1) {
		return "--:--:--";
	}
	return format(eta);
}

/*
 * Adds a new sample to the list. Removes the first sample if needed
 *
 */
void TimeHelper::update(int value) {
	samples.append(value);
	if (samples.size() > maxSamples) {
		samples.removeFirst();
	}
}

void TimeHelper::start() {
	timer.start();
}

void TimeHelper::restart() {
	timer.restart();
	samples.clear();
}

double TimeHelper::perSecond() {
	double total = 0;
	if (dataType == Difference) {
		total = samples.last() - samples.first();
	}
	else {
		// Average
		for (int i = 0; i < samples.size(); i++) {
			total += samples.at(i);
		}
	}

	return total / samples.size();
}

/*
 * Formats the time in miliseconds to HH:MM:SS
 *
 */
QString TimeHelper::format(qint64 time) {
	int seconds = time/1000;
	int minutes = (seconds / 60) % 60;
	int hours = seconds / 3600;
	seconds %= 60;

	return QString("%1:%2:%3")
		.arg(hours, 2, 10, QLatin1Char('0'))
		.arg(minutes, 2, 10, QLatin1Char('0'))
		.arg(seconds, 2, 10, QLatin1Char('0'));
}
