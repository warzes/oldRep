// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
#include "stdafx.h"
#include "Menu.h"

void Menu::Init(int x, int y, const Color &clr, const Color &bkColor)
{
	m_x = x;
	m_y = y;
	m_color = clr;
	m_bkColor = bkColor;

	m_commands.clear();

	m_selectIndex = 0;
}

void Menu::AddCommand(const CommandMenu &command)
{
	m_commands.push_back(command);
}

void Menu::Draw()
{
	int currentY = m_y;
	SetColor(m_color);
	for ( int i = 0; i < m_commands.size(); i++ )
	{
		if ( m_active )
		{
			if ( m_selectIndex == i ) SetBkColor(m_bkColor);
			else SetBkColor();
		}		
		
		Print(m_x, currentY, m_commands[i].text.c_str());
		currentY++;
	}
	SetBkColor();
	SetColor();
}

void Menu::Update(int currentKey)
{
	if ( m_active )
	{
		if ( currentKey == TK_UP || currentKey == TK_W || currentKey == TK_KP_8 )
		{
			if ( m_selectIndex > 0 ) m_selectIndex--;
			else m_selectIndex = m_commands.size() - 1;
		}
		if ( currentKey == TK_DOWN || currentKey == TK_S || currentKey == TK_KP_2 )
		{
			m_selectIndex++;
			if ( m_selectIndex >= m_commands.size() ) m_selectIndex = 0;
		}
		if ( currentKey == TK_ENTER || currentKey == TK_SPACE )
		{
			m_commands[m_selectIndex].callback();
		}
		if ( currentKey == TK_ESCAPE )
		{
			SetActive(false);
		}
	}

	for ( int i = 0; i < m_commands.size(); i++ )
	{
		if ( currentKey == m_commands[i].bindKey && m_commands[i].callback )
		{
			m_commands[i].callback();
			break;
		}
	}
}

void Menu::SetIndex(uint8_t index)
{ 
	if ( m_commands.empty() )
	{
		m_selectIndex = 0;
	}
	else
	{
		m_selectIndex = index;
		if ( m_selectIndex >= m_commands.size() )
			m_selectIndex = m_commands.size() - 1;
	}
}