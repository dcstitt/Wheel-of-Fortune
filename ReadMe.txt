Description: This is a small Wheel of Fortune desktop application I wrote in C# for my family to play.  I primarily work with C++, so I wanted more experience in C#.  There is no digital Wheel of Fortune board to spin because I employed a friend to create a physical Wheel of Fortune board as I thought my family would enjoy that more.     

It was requested that I make the source code available for others to play as well.  After making it available, others requested the ability to change the player names and puzzle phrases without having to recompile.  For this reason, player names and phrases are now read directly from text files within the directory.

To Play:
1. Adjust Player Names in the ..\bin\Debug\playerNames.txt file
2. Adjust Puzzles (as many as you want!) in the ..\bin\Deug\puzzles.txt
3. Double-Click on Wheel of Fortune.exe
4. Click 'Load Next Puzzle' to load in the next puzzle from the puzzles.txt file.
5. Select the name of the player who is currently guessing a letter from the drop-down
6. Enter a monetary amount (without the $) in the box next to the player's name
	Note: There is no digital board.  I'd recommend being creative or just finding a 	random number generator online
7. Select a letter from the list of buttons underneath the game board
8. If a player is ready to solve, type the phrase into the text box below the player's name and select 'Solve Puzzle'