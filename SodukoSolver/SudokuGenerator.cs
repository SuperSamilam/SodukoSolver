using System.Numerics;
using System.Security.Cryptography.X509Certificates;

public class SodokuGenerator
{
    static int[,] grid;
    static int[,] unsolvedGrid;

    public SodokuGenerator()
    {
        grid = new int[9, 9];
        unsolvedGrid = new int[9, 9];
    }

    #region generate
    public void GenerateSudoku()
    {
        Random rand = new Random();
        Cell[,] cells = new Cell[9, 9];
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                cells[i, j] = new Cell();
            }
        }

        for (int i = 0; i < 81; i++)
        {
            Vector2 pos = FindLeastEntropyCell(cells, rand);
            int row = (int)pos.X;
            int col = (int)pos.Y;
            int value = cells[row, col].possibleValues[rand.Next(0, cells[row, col].possibleValues.Count)];
            cells[row, col].value = value;
            cells[row, col].possibleValues.Clear();
            ReduceOtherCellPossiebiltys(row, col, value, cells);
        }

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                grid[i, j] = cells[i, j].value;
            }
        }


        Console.WriteLine("Sudoku generated succsesfully");
    }
    Vector2 FindLeastEntropyCell(Cell[,] cells, Random rand)
    {
        List<Vector2> leastEntropyCells = new List<Vector2>();
        int threshold = 9;

        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                if (cells[i, j].value != -1)
                    continue;

                if (cells[i, j].possibleValues.Count < threshold)
                {
                    leastEntropyCells.Clear();
                    leastEntropyCells.Add(new Vector2(i, j));
                    threshold = cells[i, j].possibleValues.Count;
                }
                else if (cells[i, j].possibleValues.Count == threshold)
                {
                    leastEntropyCells.Add(new Vector2(i, j));
                }
            }
        }
        return leastEntropyCells[rand.Next(0, leastEntropyCells.Count)];
    }
    void ReduceOtherCellPossiebiltys(int row, int col, int value, Cell[,] cells)
    {
        for (int i = 0; i < 9; i++)
        {
            if (cells[row, i].value == -1)
                if (cells[row, i].possibleValues.Contains(value))
                    cells[row, i].possibleValues.Remove(value);

            if (cells[i, col].value == -1)
                if (cells[i, col].possibleValues.Contains(value))
                    cells[i, col].possibleValues.Remove(value);
        }

        if (row <= 2)
        {
            if (col <= 2)
            {
                RemoveNumbersInSquere(0, 0, value, cells);
            }
            else if (col <= 5)
            {
                RemoveNumbersInSquere(0, 3, value, cells);
            }
            else
            {
                RemoveNumbersInSquere(0, 6, value, cells);
            }
        }
        else if (row <= 5)
        {
            if (col <= 2)
            {
                RemoveNumbersInSquere(3, 0, value, cells);
            }
            else if (col <= 5)
            {
                RemoveNumbersInSquere(3, 3, value, cells);
            }
            else
            {
                RemoveNumbersInSquere(3, 6, value, cells);
            }
        }
        else
        {
            if (col <= 2)
            {
                RemoveNumbersInSquere(6, 0, value, cells);
            }
            else if (col <= 5)
            {
                RemoveNumbersInSquere(6, 3, value, cells);
            }
            else
            {
                RemoveNumbersInSquere(6, 6, value, cells);
            }
        }

    }
    void RemoveNumbersInSquere(int rowMin, int colMin, int value, Cell[,] cells)
    {
        for (int i = rowMin; i < rowMin + 3; i++)
        {
            for (int j = colMin; j < colMin + 3; j++)
            {
                if (cells[i, j].value == -1)
                    if (cells[i, j].possibleValues.Contains(value))
                        cells[i, j].possibleValues.Remove(value);
            }
        }
    }

    #endregion

    public void GenerateUnsolvedGrid(int clues)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                unsolvedGrid[i, j] = grid[i, j];
            }
        }

        int holes = 81 - clues;
        List<(int, int)> removedValues = new List<(int, int)>();

        List<(int, int)> values = new List<(int, int)>();
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                values.Add((i, j));
            }
        }
        Random random = new Random();
        int n = values.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            (int, int) value = values[k];
            values[k] = values[n];
            values[n] = value;
        }

        while (removedValues.Count < holes)
        {
            int row = values[values.Count].Item1;
            int col = values[values.Count].Item2;
            removedValues.Add(values[values.Count]);
            values.RemoveAt(values.Count);

            unsolvedGrid[row, col] = 0;

            int[,] copy = new int[9, 9];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    copy[i, j] = unsolvedGrid[i, j];
                }
            }
        }
    }

    void CheckForMultpleSoulutions()
    {
        
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
}

struct Cell
{
    public int value;
    public List<int> possibleValues;

    public Cell()
    {
        value = -1;
        possibleValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    }
}