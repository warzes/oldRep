#pragma once

#define START_YEAR 987

struct Date
{
	std::wstring GetDateText();
	std::wstring GetTimeText();

	uint8_t minutes = 0;
	uint8_t hours = 0;

	uint8_t day = 1;
	uint8_t month = 1;
	uint32_t year = START_YEAR;
};

struct TriggerDay
{
	std::function<void()> func;

	uint8_t hour = 0;
	uint8_t day = 0;
};

class DateManager
{
public:
	void Calculated(uint32_t skipMinute);
	void Draw();

	void AddTriggerDay(const std::function<void()>&func, uint8_t hour);

private:
	Date m_date;

	std::vector<TriggerDay> m_trigger;
};