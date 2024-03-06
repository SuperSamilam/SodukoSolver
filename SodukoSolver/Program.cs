SodokuGenerator sodokuGenerator = new SodokuGenerator();


//sodokuGenerator.UnsolveSudoku(1);

sodokuGenerator.GenerateSudoku();


Console.WriteLine("Done");
sodokuGenerator.DrawSudoku();
Console.WriteLine("");
//sodokuGenerator.DrawSudokuUnsolved();