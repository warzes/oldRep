// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
#include "stdafx.h"
#include "Data.h"

void Guild::Init()
{
	m_characters.push_back(Character());
	m_characters.push_back(Character());
	m_characters.push_back(Character());
	m_characters.push_back(Character());
	m_characters.push_back(Character());
	m_characters.push_back(Character());
}

void Guild::CalculateExpenses()
{
	// платим персонажам
	for ( int i = 0; i < m_characters.size(); i++ )
	{
		m_storage.m_goldMoney -= 5;
	}
}

void World::Start()
{
	m_pauseWorld = false;
	m_guild.Init();
	m_date.AddTriggerDay(std::bind(&World::newDay, this), 7);
}

void World::Update(int currentKey)
{
	const std::chrono::steady_clock::time_point currentTime = std::chrono::steady_clock::now();
	const auto diff = currentTime - m_prevTime;
	if ( diff > std::chrono::seconds(1) ) // Каждые 1 секунд
	{
		m_prevTime = currentTime;

		if ( !m_pauseWorld )
		{
			m_date.Calculated(7 * 60);
		}		
	}

	if ( m_windowNewDay )
	{
		if ( currentKey == TK_ESCAPE )
		{
			m_pauseWorld = false;
			m_windowNewDay = false;
		}
	}
}

void World::Frame()
{
	m_date.Draw();

	Print(1, 6, L"Численность гильдии: " + std::to_wstring(m_guild.GetNumCharacters()));
	Print(1, 7, L"Золото гильдии: " + std::to_wstring(m_guild.GetStorage().GetGoldMoney()));

	if ( m_windowNewDay )
	{
		DrawFrame(2, 2, 76, 23);
		Print(2, 2, 76, 1, L" Новый день ", Center);

		Print(2, 3, 76, 1, L" (нажмите ESC чтобы закрыть это окно) ", Center);


	}
}

void World::newDay()
{
	//m_pauseWorld = true;
	//m_windowNewDay = true;
	
	//m_guild.CalculateExpenses();
}