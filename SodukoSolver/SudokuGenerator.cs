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

        while (!CheckGrid())
        {
            // DrawSudoku();
            //DrawPossibliyes(cells);
            // Console.WriteLine("Ready");
            // Console.ReadKey();

            (int, int) pos = FindCell(cells);
            CollapseCell(pos.Item1, pos.Item2, cells);
        }
    }

    (int, int) FindCell(Cell[,] cells)
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

        return values[rand.Next(0, values.Count)];
    }
    bool CollapseCell(int row, int col, Cell[,] cells)
    {
        cells[row, col].value = cells[row, col].possibleValues[rand.Next(0, cells[row, col].possibleValues.Count)];
        cells[row, col].colapsed = true;
        cells[row, col].possibleValues.Clear();
        grid[row, col] = cells[row, col].value;
        UpdateCorrsponingCells(row, col, grid[row, col], cells);

        return true;
    }
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
                // if (cells[i, j].colapsed)
                //     Console.Write(cells[i, j].possibleValues.Count + " ");
                // else
                //     Console.Write("  ");
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

