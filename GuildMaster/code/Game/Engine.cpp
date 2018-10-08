// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
#include "stdafx.h"
#include "Engine.h"

Color DefaultColor = White;
//Color DefaultBKColor = MidnightBlue;
Color DefaultBKColor = Black;

bool GameClose = false;

void Clear()
{
	terminal_clear();
}

void SetColor()
{
	SetColor(DefaultColor);
}

void SetColor(const Color &color)
{
	terminal_color(color_from_argb(color.a, color.r, color.g, color.b));
}

void SetColor(uint8_t r, uint8_t g, uint8_t b, uint8_t a)
{
	terminal_color(color_from_argb(a, r, g, b));
}

void SetBkColor()
{
	SetBkColor(DefaultBKColor);
}

void SetBkColor(const Color &color)
{
	terminal_bkcolor(color_from_argb(color.a, color.r, color.g, color.b));
}

void SetBkColor(uint8_t r, uint8_t g, uint8_t b, uint8_t a)
{
	terminal_bkcolor(color_from_argb(a, r, g, b));
}

void Put(int x, int y, wchar_t c)
{
	terminal_put(x, y, c);
}

int Print(int x, int y, const std::wstring &text)
{
	return terminal_print(x, y, text.c_str()).width;
}

void Print(int x, int y, int w, int h, const std::wstring &text, AlignmentText horisontalAlign, AlignmentText verticalAlign)
{
	int align = (int)horisontalAlign | (int)verticalAlign;
	terminal_print_ext(x, y, w, h, align, text.c_str());
}

void Layer(int layer)
{
	terminal_layer(layer);
}

void DrawFrame(int x, int y, int w, int h)
{
	terminal_clear_area(x, y, w, h);

	for ( int i = x; i<x + w; i++ )
	{
		terminal_put(i, y, L'═');
		terminal_put(i, y + h - 1, L'═');
	}

	for ( int j = y; j<y + h; j++ )
	{
		terminal_put(x, j, L'║');
		terminal_put(x + w - 1, j, L'║');
	}

	terminal_put(x, y, L'╔');
	terminal_put(x + w - 1, y, L'╗');
	terminal_put(x, y + h - 1, L'╚');
	terminal_put(x + w - 1, y + h - 1, L'╝');



}