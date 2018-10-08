#pragma once

struct CommandMenu
{
	CommandMenu(const std::wstring &ntext, const std::function<void()> &ncallback, int nbindKey) : text(ntext), callback(ncallback), bindKey(nbindKey) {}

	std::wstring text;
	std::function<void()> callback;
	int bindKey = 0;
};

class Menu
{
public:
	void Init(int x, int y, const Color &clr, const Color &bkColor);
	void AddCommand(const CommandMenu &command);

	void Draw();
	void Update(int currentKey);

	void SetIndex(uint8_t index);

	void SetActive(bool active) { m_active = active; }

private:
	std::vector<CommandMenu> m_commands;

	int m_x = 0;
	int m_y = 0;
	Color m_color;
	Color m_bkColor;

	uint8_t m_selectIndex = 0;

	bool m_active = true;
};