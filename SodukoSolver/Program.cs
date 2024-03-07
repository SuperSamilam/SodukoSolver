



Console.Clear();
SudokuGenerator sodokuGenerator = new SudokuGenerator();

SudokuGenerator.GenerateWholeSudoku();
SudokuGenerator.DrawSudoku(SudokuGenerator.grid);

Settings.lang = "EN";
Settings.EditSettings();

//ska göra en play method och sen är det typ klart

