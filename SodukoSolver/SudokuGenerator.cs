public class SudokuGenerator
{
    public static int[,] grid = new int[9, 9];
    public static int[,] unsolvedGrid = new int[9, 9];

    public static int soulutuons = 0;
    static int holes = 0;

    private Random rand;

    public SudokuGenerator()
    {
        grid = new int[9, 9];
        unsolvedGrid = new int[9, 9];
        rand = new Random();
    }

    //Loads the sudoku and makes the gameplay
    public static void Play(int clues)
    {
        int hints = Settings.hints;
        char[] row = new char[9] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };
        char[] col = new char[9] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        GenerateWholeSudoku(clues);
        Console.WriteLine("You are playing Sudoku");
        Console.WriteLine("Select what location to place at with Row(A-H)Col(1-8) folloed by number");
        Console.WriteLine("Ready? Press any button");
        Console.ReadKey();

        int[,] playerGrid = new int[9, 9];
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                playerGrid[i, j] = unsolvedGrid[i, j];
            }
        }

        while (true)
        {
            Console.Clear();
            DrawSudokuGame(grid, playerGrid, unsolvedGrid);
            if (IsSudokuEqual(grid, playerGrid))
            {
                Console.WriteLine("You solved it great work");
                Console.WriteLine("Press any key to go back");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("1: hint, 2 quit");

            string resp = Console.ReadLine() ?? "";



            if (row.Contains(resp[0]))
            {

                if (resp.Length < 2)
                {
                    continue;
                }

                if (col.Contains(resp[1]))
                {
                    if (resp.Length < 3)
                    {
                        continue;
                    }
                    if (col.Contains(resp[3]))
                    {
                        if (char.IsDigit(resp[3]))
                        {
                            int value = (int)char.GetNumericValue(resp[3]);
                            int rowIndex = Array.IndexOf(row, resp[0]);
                            int colIndex = Array.IndexOf(col, resp[1]);

                            if (unsolvedGrid[colIndex, rowIndex] != 0)
                            {
                                Console.WriteLine("Cant change that");
                                Thread.Sleep(3000);
                                continue;
                            }

                            playerGrid[colIndex, rowIndex] = value;
                            Console.WriteLine("Value changed");
                            Thread.Sleep(3000);
                        }
                    }
                }

            }
            else if (resp[0] == '1')
            {
                if (hints == 0)
                {
                    Console.WriteLine("No hints left");
                    Thread.Sleep(3000);
                    continue;
                }
                else
                {
                    bool gavehint = false;
                    for (int i = 0; i < 9 && !gavehint; i++)
                    {
                        for (int j = 0; j < 9 && !gavehint; j++)
                        {
                            if (playerGrid[i, j] != grid[i, j])
                            {
                                playerGrid[i, j] = grid[i, j];
                                Console.WriteLine("Changed the value on " + row[i] + (j + 1) + ", hints left: " + hints);
                                Thread.Sleep(3000);
                                hints--;
                                gavehint = true;
                            }
                        }
                    }
                }
            }
            else if (resp[0] == '2')
            {
                return;
            }

        }
    }

    //Generates a whole Sudou
    public static void GenerateWholeSudoku()
    {
        GenerateSudoku();
        UnSolveSudoku();
    }

    public static void GenerateWholeSudoku(int clues)
    {
        GenerateSudoku();
        UnSolveSudoku(clues);
    }

    #region Generation

    //Generates a solved Sudoku
    public static void GenerateSudoku()
    {
        //Prepers the area and calls the reccurisve method
        Cell[,] cells = new Cell[9, 9];
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                cells[i, j] = new Cell();
            }
        }
        FillGrid(cells);
    }

    //A recursive method to fill the sudoku with values that can be in that position
    static bool FillGrid(Cell[,] cells)
    {
        //Gets the cells with the least amount of possible values left and shuffels is so the placement will be random
        List<(int, int)> pos = ShuffleList<(int, int)>(FindCell(cells));
        for (int i = 0; i < pos.Count; i++)
        {
            cells[pos[i].Item1, pos[i].Item2].possibleValues = ShuffleList<int>(cells[pos[i].Item1, pos[i].Item2].possibleValues);
            for (int j = 0; j < cells[pos[i].Item1, pos[i].Item2].possibleValues.Count; j++)
            {
                CollapseCell(pos[i].Item1, pos[i].Item2, cells[pos[i].Item1, pos[i].Item2].possibleValues[j], cells);
                //If the grid is complete return true
                if (CheckGrid())
                {
                    return true;
                }
                else
                {
                    if (FillGrid(cells))
                        return true;
                    else
                    {
                        //This vaue didint work so reset it
                        cells[pos[i].Item1, pos[i].Item2].value = 0;
                        cells[pos[i].Item1, pos[i].Item2].colapsed = false;
                        grid[pos[i].Item1, pos[i].Item2] = 0;
                        RecalculateAllCells(cells);
                    }

                }
            }
        }
        return false;
    }

    //Finds the cells with the least amount of possible values left
    static List<(int, int)> FindCell(Cell[,] cells)
    {
        int threshold = 9;
        List<(int, int)> values = new List<(int, int)>();
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (cells[i, j].colapsed)
                    continue;

                if (cells[i, j].possibleValues.Count < threshold)
                {
                    values.Clear();
                    values.Add((i, j));
                    threshold = cells[i, j].possibleValues.Count;
                }
                else if (cells[i, j].possibleValues.Count == threshold)
                {
                    values.Add((i, j));
                }
            }
        }

        return values;
    }

    //Marks the cell as completet
    static bool CollapseCell(int row, int col, int value, Cell[,] cells)
    {
        cells[row, col].value = value;
        cells[row, col].colapsed = true;
        cells[row, col].possibleValues.Clear();
        grid[row, col] = cells[row, col].value;
        UpdateCorrsponingCells(row, col, grid[row, col], cells);

        return true;
    }

    //Updates all the cells next to this to only be able to contain the right values
    static void UpdateCorrsponingCells(int row, int col, int value, Cell[,] cells)
    {
        //Checks the collum and row
        for (int i = 0; i < 9; i++)
        {
            if (!cells[row, i].colapsed)
                if (cells[row, i].possibleValues.Contains(value))
                    cells[row, i].possibleValues.Remove(value);

            if (!cells[i, col].colapsed)
                if (cells[i, col].possibleValues.Contains(value))
                    cells[i, col].possibleValues.Remove(value);
        }

        int startRow = 0;
        int startCol = 0;

        if (row < 3)
        {
            if (col < 3)
                startCol = 0;
            else if (col < 6)
                startCol = 3;
            else
                startCol = 6;
            startRow = 0;
        }
        else if (row < 6)
        {
            if (col < 3)
                startCol = 0;
            else if (col < 6)
                startCol = 3;
            else
                startCol = 6;
            startRow = 3;
        }
        else
        {
            if (col < 3)
                startCol = 0;
            else if (col < 6)
                startCol = 3;
            else
                startCol = 6;
            startRow = 6;
        }

        //Checks the grid it is in
        for (int i = startRow; i < startRow + 3; i++)
        {
            for (int j = startCol; j < startCol + 3; j++)
            {
                if (!cells[i, j].colapsed)
                    if (cells[i, j].possibleValues.Contains(value))
                        cells[i, j].possibleValues.Remove(value);
            }
        }
    }

    #endregion

    #region UnSolve

    //Adds the holes in the sudoku
    public static void UnSolveSudoku(int clues = 25)
    {
        holes = 81 - clues;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                unsolvedGrid[i, j] = grid[i, j];
            }
        }
        RemoveFromGrid();
    }

    //A reccursive method the remove holes if it can
    static bool RemoveFromGrid()
    {
        List<(int, int)> nonEmptyCells = ShuffleList<(int, int)>(GetNonEmptyCells());

        int backUp;
        //Goes trhough all cells that it can to try set them to 0 if it cant reste
        for (int i = 0; i < nonEmptyCells.Count; i++)
        {
            backUp = unsolvedGrid[nonEmptyCells[i].Item1, nonEmptyCells[i].Item2];
            unsolvedGrid[nonEmptyCells[i].Item1, nonEmptyCells[i].Item2] = 0;
            holes--;

            //Finds if it is still solvable
            if (!FindAmountOfSolutions())
            {
                unsolvedGrid[nonEmptyCells[i].Item1, nonEmptyCells[i].Item2] = backUp;
                holes++;
                continue;
            }

            if (holes == 0)
            {
                return true;
            }
            else
            {
                if (RemoveFromGrid())
                    return true;
                else
                {
                    unsolvedGrid[nonEmptyCells[i].Item1, nonEmptyCells[i].Item2] = backUp;
                    holes++;
                }
            }

        }
        return false;
    }

    //Get all cells that does not have a 0
    static List<(int, int)> GetNonEmptyCells()
    {
        List<(int, int)> nonEmptyCells = new List<(int, int)>();
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (unsolvedGrid[i, j] != 0)
                    nonEmptyCells.Add((i, j));
            }
        }
        return nonEmptyCells;
    }

    //Finds the amount of solutions a sudoku have
    public static bool FindAmountOfSolutions()
    {
        soulutuons = 0;

        GetSoultionAmount(unsolvedGrid, 0, 0);

        if (soulutuons >= 2)
            return false;
        return true;
    }

    //A recusrive method that solves a sudoku
    static bool GetSoultionAmount(int[,] grid, int row, int col)
    {
        if (row == 9)
        {
            soulutuons++;
            return false;
        }
        else if (col == 9)
        {
            GetSoultionAmount(grid, row + 1, 0);
        }
        else if (grid[row, col] != 0)
        {
            GetSoultionAmount(grid, row, col + 1);
        }
        else
        {
            //if it can place try to place it if it doesnt work from there reset
            for (int i = 1; i < 10; i++)
            {
                if (IsValid(grid, row, col, i))
                {
                    grid[row, col] = i;
                    if (GetSoultionAmount(grid, row, col + 1))
                        return false;
                    else
                        grid[row, col] = 0;
                }
            }
            return false;
        }

        return false;
    }

    //Finds if a value can be placed at a cell
    static bool IsValid(int[,] grid, int row, int col, int value)
    {
        //Checks the collum and row
        for (int i = 0; i < 9; i++)
        {
            if (i != col)
                if (grid[row, i] == value)
                    return false;

            if (i != row)
                if (grid[i, col] == value)
                    return false;
        }

        int startRow = 0;
        int startCol = 0;

        if (row < 3)
        {
            if (col < 3)
                startCol = 0;
            else if (col < 6)
                startCol = 3;
            else
                startCol = 6;
            startRow = 0;
        }
        else if (row < 6)
        {
            if (col < 3)
                startCol = 0;
            else if (col < 6)
                startCol = 3;
            else
                startCol = 6;
            startRow = 3;
        }
        else
        {
            if (col < 3)
                startCol = 0;
            else if (col < 6)
                startCol = 3;
            else
                startCol = 6;
            startRow = 6;
        }

        //Checks the subgrid
        for (int i = startRow; i < startRow + 3; i++)
        {
            for (int j = startCol; j < startCol + 3; j++)
            {
                if (i != row && j != col)
                {
                    if (grid[i, j] == value)
                        return false;
                }
            }
        }
        return true;
    }

    #endregion

    #region Generic

    //Shuffels a generic list
    static List<T> ShuffleList<T>(List<T> list)
    {
        Random random = new Random();

        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = random.Next(0, i + 1);

            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }

    //Recaulculates all the values in the grid to know what possible numbers can be there
    static void RecalculateAllCells(Cell[,] cells)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (!cells[i, j].colapsed)
                {
                    cells[i, j].possibleValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                }
            }
        }

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (cells[i, j].colapsed)
                {
                    UpdateCorrsponingCells(i, j, cells[i, j].value, cells);
                }
            }
        }
    }

    //Checks if there are any zeros in the grid
    static bool CheckGrid()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (grid[i, j] == 0)
                    return false;
            }
        }
        return true;
    }

    //Draws the grid
    public static void DrawSudoku(int[,] grid)
    {
        char[] col = new char[9] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        Console.WriteLine("   A B C D E F G H I");
        for (int i = 0; i < 9; i++)
        {
            Console.Write(col[i] + ": ");
            for (int j = 0; j < 9; j++)
            {
                Console.Write(grid[i, j] + " ");
            }
            Console.WriteLine();
        }
    }

    //Draws the sudoku with different colors for clarificatoin for the user
    public static void DrawSudokuGame(int[,] grid, int[,] playergrid, int[,] unsolvedGrid)
    {
        char[] col = new char[9] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        Console.WriteLine("   A B C D E F G H I");
        for (int i = 0; i < 9; i++)
        {
            Console.Write(col[i] + ": ");
            for (int j = 0; j < 9; j++)
            {
                if (unsolvedGrid[i, j] != 0)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(grid[i, j] + " ");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else 
                {
                    if (grid[i, j] != playergrid[i, j] && playergrid[i, j] != 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(playergrid[i, j] + " ");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                        Console.Write(playergrid[i, j] + " ");
                }

            }
            Console.WriteLine();
        }
    }

    //Sets the sudoku to the users input
    public static void SetSudoku(int[,] soduko, bool unsolved)
    {
        if (unsolved)
            unsolvedGrid = soduko;
        else
            grid = soduko;
    }

    //checks if the sudoku is done
    static bool IsSudokuEqual(int[,] grid, int[,] check)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (grid[i, j] != check[i, j])
                    return false;
            }
        }
        return true;
    }

    #endregion

}

struct Cell
{
    public int value;
    public List<int> possibleValues;
    public bool colapsed;

    public Cell()
    {
        value = 0;
        possibleValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        colapsed = false;
    }
}