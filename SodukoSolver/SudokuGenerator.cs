using System.Numerics;
using System.Security.Cryptography.X509Certificates;

public class SodokuGenerator
{
    static int[,] grid;
    static int[,] unsolvedGrid;
    public static int soulutions;

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
            (int, int) pos = FindLeastEntropyCell(cells, rand);
            cells[pos.Item1, pos.Item2].value = cells[pos.Item1, pos.Item2].possibleValues[rand.Next(0, cells[pos.Item1, pos.Item2].possibleValues.Count)];
            cells[pos.Item1, pos.Item2].possibleValues.Clear();
            ReduceOtherCellPossiebiltys(pos.Item1, pos.Item2, cells[pos.Item1, pos.Item2].value, cells);
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
    (int, int) FindLeastEntropyCell(Cell[,] cells, Random rand)
    {
        List<(int, int)> leastEntropyCells = new List<(int, int)>();
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
                    leastEntropyCells.Add((i, j));
                    threshold = cells[i, j].possibleValues.Count;
                }
                else if (cells[i, j].possibleValues.Count == threshold)
                {
                    leastEntropyCells.Add((i, j));
                }
            }
        }
        return leastEntropyCells[rand.Next(0, leastEntropyCells.Count)];
    }
    void ReduceOtherCellPossiebiltys(int row, int col, int value, Cell[,] cellGrid)
    {
        for (int i = 0; i < 9; i++)
        {
            if (cellGrid[row, i].value == -1)
                if (cellGrid[row, i].possibleValues.Contains(value))
                    cellGrid[row, i].possibleValues.Remove(value);

            if (cellGrid[i, col].value == -1)
                if (cellGrid[i, col].possibleValues.Contains(value))
                    cellGrid[i, col].possibleValues.Remove(value);
        }

        RemoveNumbersInSquere(RoundToNearestFromList(row, new List<int>() { 0, 3, 6 }), RoundToNearestFromList(col, new List<int>() { 0, 3, 6 }), value, cellGrid);

    }
    static int RoundToNearestFromList(int inputNum, List<int> integerList)
    {
        int closestNum = integerList.OrderBy(x => Math.Abs(x - inputNum)).First();
        return closestNum;
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

    public void UnsolveSudoku(int attempts = 5)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                unsolvedGrid[i, j] = grid[i, j];
            }
        }

        Random rand = new Random();
        while (attempts > 0)
        {
            int row = rand.Next(0, 9);
            int col = rand.Next(0, 9);
            while (unsolvedGrid[row, col] == 0)
            {
                row = rand.Next(0, 9);
                col = rand.Next(0, 9);
            }
            int backup = unsolvedGrid[row, col];
            unsolvedGrid[row, col] = 0;

            int[,] copyGrid = new int[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    copyGrid[i, j] = unsolvedGrid[i, j];
                }
            }

            Console.WriteLine(soulutions);
            SolveGrid(copyGrid);
            if (soulutions != 1)
            {
                unsolvedGrid[row, col] = backup;
                attempts--;
            }
            else
            {
            }
            Console.WriteLine(soulutions);
        }
    }

    bool SolveGrid(int[,] grid)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (grid[row, col] == 0)
                {
                    for (int value = 1; value < 10; value++)
                    {
                        List<int> possibleValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

                        for (int i = 0; i < 9; i++)
                        {
                            if (grid[row, i] != -1)
                                if (possibleValues.Contains(grid[row, i]))
                                    possibleValues.Remove(grid[row, i]);

                            if (grid[i, col] != -1)
                                if (possibleValues.Contains(grid[i, col]))
                                    possibleValues.Remove(grid[i, col]);
                        }
                        int minRow = RoundToNearestFromList(row, new List<int>() { 0, 3, 6 });
                        int minCol = RoundToNearestFromList(col, new List<int>() { 0, 3, 6 });

                        for (int i = minRow; i < minRow + 3; i++)
                        {
                            for (int j = minCol; j < minCol + 3; j++)
                            {
                                if (grid[i, j] != -1)
                                    if (possibleValues.Contains(grid[i, j]))
                                        possibleValues.Remove(grid[i, j]);
                            }
                        }

                        if (!possibleValues.Contains(value))
                        {
                            //the grid is compleatly filled
                            if (CheckGrid(grid))
                            {
                                soulutions++;
                                if (soulutions == 2)
                                    return false;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("tja");
                                return true;
                                // if (SolveGrid(grid, count))
                                // {
                                //     return true;
                                // }
                            }
                        }
                    }

                }
            }
        }
        return false;
    }

    bool CheckGrid(int[,] grid)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (grid[row, col] == 0)
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

    public Cell()
    {
        value = -1;
        possibleValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    }
}