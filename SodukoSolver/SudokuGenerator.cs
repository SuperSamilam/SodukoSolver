using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

public class SodokuGenerator
{
    public static int[,] grid;
    public static int[,] unsolvedGrid;

    private Random rand;
    public static int soulutuons = 0;
    static int holes = 0;


    public SodokuGenerator()
    {
        grid = new int[9, 9];
        unsolvedGrid = new int[9, 9];
        rand = new Random();
    }

    public void GenerateSudoku()
    {
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

    #region Generation

    bool FillGrid(Cell[,] cells)
    {
        List<(int, int)> pos = ShuffleList<(int, int)>(FindCell(cells));
        for (int i = 0; i < pos.Count; i++)
        {
            cells[pos[i].Item1, pos[i].Item2].possibleValues = ShuffleList<int>(cells[pos[i].Item1, pos[i].Item2].possibleValues);
            for (int j = 0; j < cells[pos[i].Item1, pos[i].Item2].possibleValues.Count; j++)
            {
                CollapseCell(pos[i].Item1, pos[i].Item2, cells[pos[i].Item1, pos[i].Item2].possibleValues[j], cells);
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
    List<(int, int)> FindCell(Cell[,] cells)
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
    bool CollapseCell(int row, int col, int value, Cell[,] cells)
    {
        cells[row, col].value = value;
        cells[row, col].colapsed = true;
        cells[row, col].possibleValues.Clear();
        grid[row, col] = cells[row, col].value;
        UpdateCorrsponingCells(row, col, grid[row, col], cells);

        return true;
    }

    #endregion

    #region UnSolve
    public void UnSolveSudoku(int clues = 25)
    {
        holes = 81 - clues;
        Console.WriteLine(holes);
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                unsolvedGrid[i, j] = grid[i, j];
            }
        }
        RemoveFromGrid();
    }

    bool RemoveFromGrid()
    {
        // Console.WriteLine(holes);
        List<(int, int)> nonEmptyCells = ShuffleList<(int, int)>(GetNonEmptyCells());

        int backUp;
        for (int i = 0; i < nonEmptyCells.Count; i++)
        {
            backUp = unsolvedGrid[nonEmptyCells[i].Item1, nonEmptyCells[i].Item2];
            unsolvedGrid[nonEmptyCells[i].Item1, nonEmptyCells[i].Item2] = 0;
            holes--;

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

    List<(int, int)> GetNonEmptyCells()
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

    public bool FindAmountOfSolutions()
    {
        soulutuons = 0;

        Solve(unsolvedGrid, 0, 0);

        if (soulutuons >= 2)
            return false;
        return true;
    }

    public bool Solve(int[,] grid, int row, int col)
    {
        if (row == 9)
        {
            soulutuons++;
            return false;
        }
        else if (col == 9)
        {
            //Console.WriteLine("Col " + row);
            Solve(grid, row + 1, 0);
        }
        else if (grid[row, col] != 0)
        {
            //Console.WriteLine("Prefilled");
            Solve(grid, row, col + 1);
        }
        else
        {
            // for (int i = 0; i < 9; i++)
            // {
            //     for (int j = 0; j < 9; j++)
            //     {
            //         Console.Write(grid[i, j] + " ");
            //     }
            //     Console.WriteLine();
            // }
            // Console.WriteLine("Ready?");
            // Console.ReadKey();
            for (int i = 1; i < 10; i++)
            {
                if (IsValid(grid, row, col, i))
                {
                    grid[row, col] = i;
                    if (Solve(grid, row, col + 1))
                        return false;
                    else
                        grid[row, col] = 0;
                }
            }
            return false;
        }

        //soulutuons++;
        return false;
    }

    bool IsValid(int[,] grid, int row, int col, int value)
    {
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

    void UpdateCorrsponingCells(int row, int col, int value, Cell[,] cells)
    {
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

    void RecalculateAllCells(Cell[,] cells)
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
    bool CheckGrid()
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
    public void DrawSudoku(int[,] grid)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Console.Write(grid[i, j] + " ");
            }
            Console.WriteLine();
        }
    }

    public void SetSudoku(int[,] soduko, bool unsolved)
    {
        if (unsolved)
            unsolvedGrid = soduko;
        else
            grid = soduko;
    }

    #endregion




}

struct Cell
{
    public int value;
    public List<int> possibleValues;
    public bool colapsed;

    public (int, int) CellCameFrom;


    public Cell()
    {
        value = 0;
        possibleValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        colapsed = false;
    }
}

struct OrderOperations
{
    int row, col;
    List<int> options;
    List<int> notValidOptions;
}

