using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

public class SodokuGenerator
{
    static int[,] grid;
    static int[,] unsolvedGrid;
    public static int collapsedCells;
    private Random rand;
    Cell lastCell;

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
        Console.WriteLine(soulutuons);
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

    bool FindAmountOfSolutions()
    {
        Cell[,] gridCopy = new Cell[9, 9];
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                gridCopy[i, j] = new Cell();
                if (unsolvedGrid[i, j] != 0)
                {
                    gridCopy[i, j].value = unsolvedGrid[i, j];
                    gridCopy[i, j].colapsed = true;
                }
            }
        }

        SolveGrid(gridCopy);

        if (soulutuons >= 2)
            return false;
        return true;
    }

    bool SolveGrid(Cell[,] cells)
    {
        List<(int, int)> pos = ShuffleList<(int, int)>(FindCell(cells));
        for (int i = 0; i < pos.Count; i++)
        {
            cells[pos[i].Item1, pos[i].Item2].possibleValues = ShuffleList<int>(cells[pos[i].Item1, pos[i].Item2].possibleValues);
            for (int j = 0; j < cells[pos[i].Item1, pos[i].Item2].possibleValues.Count; j++)
            {
                CollapseCellSolving(pos[i].Item1, pos[i].Item2, cells[pos[i].Item1, pos[i].Item2].possibleValues[j], cells);
                if (CheckGrid())
                {
                    soulutuons++;
                    return false;
                }
                else
                {
                    if (SolveGrid(cells))
                        return true;
                    else
                    {
                        cells[pos[i].Item1, pos[i].Item2].value = 0;
                        cells[pos[i].Item1, pos[i].Item2].colapsed = false;
                        RecalculateAllCells(cells);
                    }

                }
            }
        }
        return false;
    }

    bool CollapseCellSolving(int row, int col, int value, Cell[,] cells)
    {
        cells[row, col].value = value;
        cells[row, col].colapsed = true;
        cells[row, col].possibleValues.Clear();
        UpdateCorrsponingCells(row, col, grid[row, col], cells);

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
    public void DrawSudoku()
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

    void DrawPossibliyes(Cell[,] cells)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Console.Write(cells[i, j].possibleValues.Count + " ");
            }
            Console.WriteLine();
        }
    }

    public void DrawSudokuUnsolved()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Console.Write(unsolvedGrid[i, j] + " ");
            }
            Console.WriteLine();
        }
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
        value = -1;
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

