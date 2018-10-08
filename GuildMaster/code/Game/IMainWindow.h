#pragma once

enum class MainWindow
{
	guild,
};

class IMainWindow
{
public:
	virtual ~IMainWindow() = default;

	virtual bool Start() = 0;
	virtual void Frame() = 0;
	virtual void Update(int currentKey) = 0;

protected:
	IMainWindow() {};

	MainWindow m_mainWindowType = MainWindow::guild;
};