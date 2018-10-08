#pragma once

#include "IMainWindow.h"

class World;

class GuildWindow : public IMainWindow
{
public:
	GuildWindow(World *world) : m_world(world) { m_mainWindowType = MainWindow::guild; }

	bool Start() final;
	void Frame() final;
	void Update(int currentKey) final;

private:
	void test1() { m_mainCmd.SetIndex(0); }
	void test2() { m_mainCmd.SetIndex(1); }
	void test3() { m_mainCmd.SetIndex(2); }
	void test4() { m_mainCmd.SetIndex(3); }

	Menu m_mainCmd;

	World *m_world = nullptr;
};