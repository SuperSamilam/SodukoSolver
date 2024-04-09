using Sudoku;

Random rand = new Random();
SudokuGenerator sudokuGenerator = new SudokuGenerator();

while (true)
{
    Console.Clear();
    Console.WriteLine("Welcome to Sudoku");
    Console.WriteLine("1: Play easy");
    Console.WriteLine("2: Play medium");
    Console.WriteLine("3: Play hard");
    Console.WriteLine("5: Quit");

    string resp = Console.ReadLine() ?? "";

    if (resp[0] == '1')
        sudokuGenerator.Play(80);
    else if(resp[0] == '2')
        sudokuGenerator.Play(rand.Next(27,31));
    else if(resp[0] == '3')
        sudokuGenerator.Play(rand.Next(22,28));
    else if(resp[0] == '5')
        return;
}
