

Random rand = new Random();
Settings.lang = "EN";
Settings.hints = 3;

while (true)
{
    Console.Clear();
    Console.WriteLine("Welcome to Sudoku");
    Console.WriteLine("1: Play easy");
    Console.WriteLine("2: Play medium");
    Console.WriteLine("3: Play hard");
    Console.WriteLine("4: Edit settings");
    Console.WriteLine("5: Quit");

    string resp = Console.ReadLine() ?? "";

    if (resp[0] == '1')
        SudokuGenerator.Play(80);
        // SudokuGenerator.Play(rand.Next(30,36));
    else if(resp[0] == '2')
        SudokuGenerator.Play(rand.Next(27,31));
    else if(resp[0] == '3')
        SudokuGenerator.Play(rand.Next(22,28));
    else if(resp[0] == '4')
        Settings.EditSettings();
    else if(resp[0] == '5')
        return;
}
