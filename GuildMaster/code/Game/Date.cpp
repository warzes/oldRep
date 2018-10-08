// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
#include "stdafx.h"
#include "Date.h"

std::wstring Date::GetDateText()
{
	std::wstring date = L"Дата: ";
	if ( day < 10 ) date += L'0';
	date += std::to_wstring(day);
	date += L'.';
	if ( month < 10 ) date += L'0';
	date += std::to_wstring(month);
	date += L'.';
	date += std::to_wstring(year);

	return date;
}

std::wstring Date::GetTimeText()
{
	std::wstring time = L"Время: ";
	if ( hours < 10 ) time += L'0';
	time += std::to_wstring(hours);
	time += L':';
	if ( minutes < 10 ) time += L'0';
	time += std::to_wstring(minutes);

	return time;
}

void DateManager::Draw()
{
	Print(1, 4, 50, 22, (m_date.GetDateText() + L' ' + m_date.GetTimeText()).c_str(), AlignmentText::Center);
}

void DateManager::AddTriggerDay(const std::function<void()>&func, uint8_t hour)
{
	TriggerDay tday;
	tday.func = func;
	tday.hour = hour;

	m_trigger.push_back(tday);
}

void DateManager::Calculated(uint32_t skipMinute)
{
	uint32_t tempMin = m_date.minutes + skipMinute;
	while ( tempMin > 60 )
	{
		tempMin -= 60;
		m_date.hours++;
	}

	m_date.minutes = tempMin;

	if ( m_date.minutes >= 60 )
	{
		m_date.minutes = 0;
		m_date.hours++;
	}
	if ( m_date.hours >= 24 )
	{
		m_date.hours = 0;
		m_date.day++;
	}
	if ( m_date.day > 30 )
	{
		m_date.day = 1;
		m_date.month++;
	}
	if ( m_date.month > 12 )
	{
		m_date.month = 1;
		m_date.year++;
	}


	// каждые сутки
	for ( int i = 0; i < m_trigger.size(); i++ )
	{
		if ( m_trigger[i].day != m_date.day )
		{
			uint8_t th = m_date.hours;

			while ( th >= m_trigger[i].hour )
			{
				m_trigger[i].day = m_date.day;
				th -= m_trigger[i].hour;
				
				if ( m_trigger[i].func ) m_trigger[i].func();
			}
		}
	}
}