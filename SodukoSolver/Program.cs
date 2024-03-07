Console.Clear();
SodokuGenerator sodokuGenerator = new SodokuGenerator();


//sodokuGenerator.UnsolveSudoku(1);


// int[,] mySudoku = {
//     {0, 0, 0, 3, 0, 0, 0, 0, 0},
//     {0, 1, 0, 0, 0, 0, 3, 0, 6},
//     {0, 0, 0, 0, 9, 8, 0, 0, 0},
//     {0, 0, 8, 0, 5, 0, 2, 6, 0},
//     {0, 3, 0, 1, 0, 2, 0, 0, 8},
//     {7, 2, 0, 0, 4, 9, 0, 0, 0},
//     {0, 0, 5, 0, 0, 0, 0, 0, 0},
//     {0, 9, 0, 0, 0, 0, 0, 0, 4},
//     {0, 0, 0, 0, 3, 0, 9, 0, 0},
//   };
// int[,] mySudoku = {
//     {0, 0, 7, 6, 0, 0, 2, 5, 4},
//     {0, 0, 2, 0, 1, 0, 0, 0, 0},
//     {0, 6, 0, 0, 2, 7, 9, 1, 3},
//     {0, 0, 0, 8, 7, 0, 0, 0, 5},
//     {0, 5, 0, 3, 4, 0, 7, 0, 0},
//     {4, 7, 3, 2, 0, 6, 0, 0, 0},
//     {7, 3, 4, 5, 9, 0, 0, 6, 2},
//     {2, 1, 0, 0, 0, 0, 0, 3, 0},
//     {6, 0, 0, 0, 0, 2, 0, 7, 0},
//   };

// sodokuGenerator.SetSudoku(mySudoku, true);
// sodokuGenerator.Solve(mySudoku, 0, 0);
// Console.WriteLine(SodokuGenerator.soulutuons);


sodokuGenerator.GenerateSudoku();
sodokuGenerator.DrawSudoku(SodokuGenerator.grid);
// Console.WriteLine("");

sodokuGenerator.UnSolveSudoku();

// Console.WriteLine("");
// Console.WriteLine("Done");
sodokuGenerator.DrawSudoku(SodokuGenerator.unsolvedGrid);

// sodokuGenerator.FindAmountOfSolutions();
// Console.WriteLine("Done");
// sodokuGenerator.DrawSudoku();
//sodokuGenerator.DrawSudokuUnsolved();