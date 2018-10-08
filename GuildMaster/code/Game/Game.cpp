// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
#include "stdafx.h"
#include "Game.h"
#include "GuildWindow.h"

bool Game::Start()
{
	m_mainWindow = std::make_unique<GuildWindow>(&m_world);
	if ( !m_mainWindow->Start() ) return false;

	m_world.Start();

	return true;
}

void Game::Frame()
{
	m_mainWindow->Frame();
	m_world.Frame();
}
void Game::Update(int currentKey)
{
	m_mainWindow->Update(currentKey);
	m_world.Update(currentKey);
}