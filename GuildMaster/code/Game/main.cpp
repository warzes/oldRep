// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
#include "stdafx.h"
//#include "GameLoop.h"
#include "Game.h"

//GameLoop gameLoop;
Game gameLoop;

TERMINAL_TAKE_CARE_OF_WINMAIN
int main()
{
	if ( terminal_open() )
	{
		terminal_set(
			"window: size=80x25, cellsize=auto, title='Guild Master';"
			"font: default;"
			"input: filter={keyboard}"
			);
		//terminal_set("window.resizeable=true;");
		//terminal_set("window.fullscreen=true;");
		//terminal_set("U+E100: CoreTile.png, size=8x16");
		//terminal_set("U+E200: test.png, size=8x16");
		
		SetColor(DefaultColor);
		SetBkColor(DefaultBKColor);

		gameLoop.Start();
		
		for ( bool proceed = true; proceed; )
		{
			if ( GameClose ) break;

			Clear();
			gameLoop.Frame();
			terminal_refresh();

			int currentKey = 0;
			
			while ( proceed && terminal_has_input() )
			{
				// TODO: в будущем проверить currentKey
				currentKey = terminal_read();
				if ( currentKey == TK_CLOSE ) proceed = false;
			}
			gameLoop.Update(currentKey);
		}

		terminal_close();
	}	

	return 0;
}