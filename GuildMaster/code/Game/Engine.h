#pragma once

#include "Color.h"

enum AlignmentText
{
	Left = 1,
	Right = 2,
	Center = 3,
	Top = 4,
	Bottom = 8,
	Middle = 12
};

void Clear();
	
void SetColor();
void SetColor(const Color &color);
void SetColor(uint8_t r, uint8_t g, uint8_t b, uint8_t a = 255);
void SetBkColor();
void SetBkColor(const Color &color);
void SetBkColor(uint8_t r, uint8_t g, uint8_t b, uint8_t a = 255);

void Put(int x, int y, wchar_t c);
int Print(int x, int y, const std::wstring &text);
void Print(int x, int y, int w, int h, const std::wstring &text, AlignmentText horisontalAlign, AlignmentText verticalAlign = AlignmentText::Top);

void Layer(int layer);

void DrawFrame(int x, int y, int w, int h);

extern Color DefaultColor;
extern Color DefaultBKColor;

extern bool GameClose;