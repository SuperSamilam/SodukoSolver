Console.Clear();
SodokuGenerator sodokuGenerator = new SodokuGenerator();


//sodokuGenerator.UnsolveSudoku(1);


sodokuGenerator.GenerateSudoku();
sodokuGenerator.DrawSudoku();
Console.WriteLine("");

sodokuGenerator.UnSolveSudoku();
sodokuGenerator.DrawSudokuUnsolved();




Console.WriteLine("Done");
// sodokuGenerator.DrawSudoku();
//sodokuGenerator.DrawSudokuUnsolved();