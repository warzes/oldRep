#pragma once

#include "Data.h"
#include "IMainWindow.h"

class Game
{
public:
	bool Start();
	void Frame();
	void Update(int currentKey);

private:
	std::unique_ptr<IMainWindow> m_mainWindow;
	World m_world;
};