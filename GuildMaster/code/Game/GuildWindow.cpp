// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
#include "stdafx.h"
#include "GuildWindow.h"
#include "Data.h"

bool GuildWindow::Start()
{
	m_mainCmd.Init(51, 4, White, DarkMagenta);
	m_mainCmd.AddCommand({ L"[color=red](1):[/color] Сводка дня", std::bind(&GuildWindow::test1, this), TK_1 });
	m_mainCmd.AddCommand({ L"[color=red](2):[/color] Состав Гильдии", std::bind(&GuildWindow::test2, this), TK_2 });
	m_mainCmd.AddCommand({ L"[color=red](3):[/color] Ресурсы Гильдии", std::bind(&GuildWindow::test3, this), TK_3 });
	m_mainCmd.AddCommand({ L"[color=red](4):[/color] Доска заданий", std::bind(&GuildWindow::test4, this), TK_4 });

	return true;
}

void GuildWindow::Frame()
{
	//-------------------------------------------------------------------------
	// интерфейс окна
	//-------------------------------------------------------------------------
	DrawFrame(0, 2, 80, 23);
	DrawFrame(50, 2, 30, 23);

	Put(0, 0, L'╔');
	Put(0, 1, L'║');
	Put(0, 2, L'╠');

	Put(79, 0, L'╗');
	Put(79, 1, L'║');
	Put(79, 2, L'╣');

	for ( int i = 1; i < 79; i++ ) Put(i, 0, L'═');

	Put(50, 2, L'╦');
	Put(50, 24, L'╩');

	Print(0, 1, 80, 1, L" ЗАЛ ГИЛЬДИИ ", Center);
	Print(0, 2, 50, 1, L" Информация ", Center);
	Print(50, 2, 30, 1, L" Управление ", Center);

	//-------------------------------------------------------------------------
	// Вывод списка команд
	//-------------------------------------------------------------------------
	m_mainCmd.Draw();
}
void GuildWindow::Update(int currentKey)
{
	m_mainCmd.Update(currentKey);
}
