#pragma once

#include "Date.h"
#include "Quest.h"
#include "StorageGuild.h"

class Character
{
public:
};

class Guild
{
public:
	void Init();

	uint32_t GetNumCharacters() const { return m_characters.size(); }

	StorageGuild& GetStorage() { return m_storage; }

	void CalculateExpenses();
	
private:
	std::vector<Character> m_characters;
	std::vector<std::shared_ptr<IQuest>> m_activeQuests;

	StorageGuild m_storage;
};

class World
{
public:
	void Start();
	void Update(int currentKey);
	void Frame();
	
private:
	void newDay();

	std::chrono::steady_clock::time_point m_prevTime = std::chrono::steady_clock::now();

	DateManager m_date;
	Guild m_guild;

	bool m_pauseWorld = false;

	bool m_windowNewDay = false;
};