#pragma once

// хранилище гильдии
class StorageGuild
{
	friend class Guild;
public:
	uint64_t GetGoldMoney() const { return m_goldMoney; }

private:
	uint64_t m_goldMoney = ULLONG_MAX - 1;
};