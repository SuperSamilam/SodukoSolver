namespace Sudoku
{
    public class SudokuGenerator
    {
        int[,] grid;
        int[,] unsolvedGrid;

        int soulutuons;
        int holes;

        public SudokuGenerator()
        {
            grid = new int[9, 9];
            unsolvedGrid = new int[9, 9];
            soulutuons = 0;
        }

        //Loads the sudoku and makes the gameplay
        public void Play(int clues)
        {
            //Jag väljer arrays efteresom det har en set size och jag behöver inte öka strolketen i denna tillfälle
            int hints = 3;
            char[] row = new char[9] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };
            char[] col = new char[9] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            GenerateWholeSudoku(clues);
            Console.WriteLine("You are playing Sudoku");
            Console.WriteLine("Select what location to place at with Row(A-H)Col(1-8) folloed by number");
            Console.WriteLine("Ready? Press any button");
            Console.ReadKey();

            //Transforms the unsolved grid to the player grid so i dont lose the unsolved grid
            int[,] playerGrid = MakePlayerGrid();

            //Gameplay Loop
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

                if (!CheckInput(resp))
                {
                    if (resp[0] == '1')
                    {
                        //Gives the first hint if its possible
                        if (hints == 0)
                        {
                            Console.WriteLine("No hints left");
                            Thread.Sleep(3000);
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
                        //You quit casue ur boring
                        return;
                    }
                }
                else
                {
                    int value = (int)char.GetNumericValue(resp[3]);

                    int rowIndex = Array.IndexOf(row, resp.ToUpper()[0]);
                    int colIndex = Array.IndexOf(col, resp.ToUpper()[1]);

                    playerGrid[colIndex, rowIndex] = value;
                    Console.WriteLine("Value changed");
                    Thread.Sleep(3000);
                }
            }
        }

        int[,] MakePlayerGrid()
        {
            int[,] playerGrid = new int[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    playerGrid[i, j] = unsolvedGrid[i, j];
                }
            }
            return playerGrid;
        }
        bool CheckInput(string input)
        {
            char[] row = new char[9] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };
            char[] col = new char[9] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            if (input.Length < 3)
            {
                Console.WriteLine("Wrong Input");
                return false;
            }

            if (!col.Contains(input[1]))
            {
                Console.WriteLine("Wrong Input");
                return false;
            }
            if (!col.Contains(input[3]))
            {
                Console.WriteLine("Wrong Input");
                return false;
            }
            if (!char.IsDigit(input[3])) //tryparse
            {
                Console.WriteLine("Wrong Input");
                return false;
            }
            int rowIndex = Array.IndexOf(row, input.ToUpper()[0]);
            int colIndex = Array.IndexOf(col, input.ToUpper()[1]);
            if (unsolvedGrid[colIndex, rowIndex] != 0)
            {
                Console.WriteLine("Cant change that");
                Thread.Sleep(3000);
                return false;
            }


            return true;
        }

        //Generates an sudoko and an unsolved version of it
        public void GenerateWholeSudoku(int clues)
        {
            GenerateSudoku();
            UnSolveSudoku(clues);
        }

        #region Generation

        //Generates a solved Sudoku
        public void GenerateSudoku()
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

        //A recursive method to fill the sudoku with values that can be in that position alogirthm based on WFC
        bool FillGrid(Cell[,] cells)
        {
            //Gets the cells with the least amount of possible values left and shuffels is so the placement will be random
            List<(int, int)> pos = ShuffleList<(int, int)>(FindCellsWithLeastEntropy(cells));
            //Looping thought them to be availble to backtrack
            for (int i = 0; i < pos.Count; i++)
            {
                //Shuffels the list of possible values for a cell so the grid will be more random
                cells[pos[i].Item1, pos[i].Item2].possibleValues = ShuffleList<int>(cells[pos[i].Item1, pos[i].Item2].possibleValues);
                for (int j = 0; j < cells[pos[i].Item1, pos[i].Item2].possibleValues.Count; j++)
                {
                    //Sets the value for the cell
                    CollapseCell(pos[i].Item1, pos[i].Item2, cells[pos[i].Item1, pos[i].Item2].possibleValues[j], cells);
                    if (CheckGrid())
                    {
                        //If the grid is complete return true
                        return true;
                    }
                    else
                    {
                        //Call the method again - and try to compate the grid
                        if (FillGrid(cells))
                        {
                            return true;
                        }
                        else
                        {
                            //This vaue didint work so reset it - the loop will contnue, if it dosent work it will return false
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
        List<(int, int)> FindCellsWithLeastEntropy(Cell[,] cells)
        {
            int threshold = 9;
            List<(int, int)> values = new List<(int, int)>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (cells[i, j].colapsed)
                    {
                        continue;
                    }

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
        void CollapseCell(int row, int col, int value, Cell[,] cells)
        {
            cells[row, col].value = value;
            cells[row, col].colapsed = true;
            cells[row, col].possibleValues.Clear();
            grid[row, col] = cells[row, col].value;
            UpdateCorrsponingCells(row, col, grid[row, col], cells);
        }

        //Updates all the cells next to this to only be able to contain the right values
        void UpdateCorrsponingCells(int row, int col, int value, Cell[,] cells)
        {
            //Checks the collum and row
            for (int i = 0; i < 9; i++)
            {
                if (!cells[row, i].colapsed)
                {
                    if (cells[row, i].possibleValues.Contains(value))
                    {
                        cells[row, i].possibleValues.Remove(value);
                    }
                }

                if (!cells[i, col].colapsed)
                {
                    if (cells[i, col].possibleValues.Contains(value))
                    {
                        cells[i, col].possibleValues.Remove(value);
                    }
                }
            }

            int startRow = 0;
            int startCol = 0;

            //Finds teh 3x3 grid the number is in
            if (row < 3)
            {
                if (col < 3)
                {
                    startCol = 0;
                }
                else if (col < 6)
                {
                    startCol = 3;
                }
                else
                {
                    startCol = 6;
                }

                startRow = 0;
            }
            else if (row < 6)
            {
                if (col < 3)
                {
                    startCol = 0;
                }
                else if (col < 6)
                {
                    startCol = 3;
                }
                else
                {
                    startCol = 6;
                }

                startRow = 3;
            }
            else
            {
                if (col < 3)
                {
                    startCol = 0;
                }
                else if (col < 6)
                {
                    startCol = 3;
                }
                else
                {
                    startCol = 6;
                }

                startRow = 6;
            }

            //Checks the grid it is in
            for (int i = startRow; i < startRow + 3; i++)
            {
                for (int j = startCol; j < startCol + 3; j++)
                {
                    if (!cells[i, j].colapsed)
                    {
                        if (cells[i, j].possibleValues.Contains(value))
                        {
                            cells[i, j].possibleValues.Remove(value);
                        }
                    }
                }
            }
        }

        #endregion

        #region UnSolve

        //Adds the holes in the sudoku
        public void UnSolveSudoku(int clues = 25)
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
        bool RemoveFromGrid()
        {
            //shuffeling it for randomness
            List<(int, int)> nonEmptyCells = ShuffleList<(int, int)>(GetNonEmptyCells());

            int backUp;
            //Goes trhough all cells that it can to try set them to 0 if it cant restet
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

                //Soduku is generated and done
                if (holes == 0)
                {
                    return true;
                }
                else
                {
                    //Remove another value from grid
                    if (RemoveFromGrid())
                    {
                        return true;
                    }
                    else
                    {
                        //Reset the value
                        unsolvedGrid[nonEmptyCells[i].Item1, nonEmptyCells[i].Item2] = backUp;
                        holes++;
                    }
                }

            }
            return false;
        }

        //Get all cells that does not have a 0
        List<(int, int)> GetNonEmptyCells()
        {
            List<(int, int)> nonEmptyCells = new List<(int, int)>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (unsolvedGrid[i, j] != 0)
                    {
                        nonEmptyCells.Add((i, j));
                    }
                }
            }
            return nonEmptyCells;
        }

        //Finds the amount of solutions a sudoku have
        public bool FindAmountOfSolutions()
        {
            soulutuons = 0;

            GetSoultionAmount(unsolvedGrid, 0, 0);

            if (soulutuons >= 2)
            {
                return false;
            }

            return true;
        }

        //A recusrive method that solves a sudoku
        bool GetSoultionAmount(int[,] grid, int row, int col)
        {
            //The entire grid have been itterated to
            if (row == 9)
            {
                soulutuons++;
                return false;
            }
            else if (col == 9)
            {
                //It reached the end of this collumn, change row
                GetSoultionAmount(grid, row + 1, 0);
            }
            else if (grid[row, col] != 0)
            {
                //ITs already filled
                GetSoultionAmount(grid, row, col + 1);
            }
            else
            {
                //Lopes thought each value a soduku grid can have
                for (int i = 1; i < 10; i++)
                {
                    if (IsValid(grid, row, col, i))
                    {
                        //It can be placed 
                        grid[row, col] = i;
                        //If the souluton in the futrue is right return false so it will count all soulutons
                        if (GetSoultionAmount(grid, row, col + 1))
                        {
                            return false;
                        }
                        else
                        {
                            grid[row, col] = 0;
                        }
                        //The value can not be the expted value anymore for further tests, so reset it. -- needs to be there because of it might the the last itteration
                    }
                }
                return false;
            }

            return false;
        }

        //Finds if a value can be placed at a cell
        bool IsValid(int[,] grid, int row, int col, int value)
        {
            //Checks the collum and row
            for (int i = 0; i < 9; i++)
            {
                if (i != col)
                {
                    if (grid[row, i] == value)
                    {
                        return false;
                    }
                }

                if (i != row)
                {
                    if (grid[i, col] == value)
                    {
                        return false;
                    }
                }
            }

            int startRow = 0;
            int startCol = 0;

            if (row < 3)
            {
                if (col < 3)
                {
                    startCol = 0;
                }
                else if (col < 6)
                {
                    startCol = 3;
                }
                else
                {
                    startCol = 6;
                }

                startRow = 0;
            }
            else if (row < 6)
            {
                if (col < 3)
                {
                    startCol = 0;
                }
                else if (col < 6)
                {
                    startCol = 3;
                }
                else
                {
                    startCol = 6;
                }

                startRow = 3;
            }
            else
            {
                if (col < 3)
                {
                    startCol = 0;
                }
                else if (col < 6)
                {
                    startCol = 3;
                }
                else
                {
                    startCol = 6;
                }

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
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        #endregion

        #region Generic

        //Shuffels a generic list
        List<T> ShuffleList<T>(List<T> list)
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

        //Checks if there are any zeros in the grid
        bool CheckGrid()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid[i, j] == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //Draws the sudoku with different colors for clarificatoin for the user
        void DrawSudokuGame(int[,] grid, int[,] playergrid, int[,] unsolvedGrid)
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
                        {
                            Console.Write(playergrid[i, j] + " ");
                        }
                    }

                }
                Console.WriteLine();
            }
        }

        //checks if the sudoku is done
        bool IsSudokuEqual(int[,] grid, int[,] check)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid[i, j] != check[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion

    }

    struct Cell
    {
        public int value;
        //I choose a lsit becasue of the lenght of the lsit can be 1-9, an arrey chould be used by filling the reaming cells with -1 but it whould be a lot harder
        public List<int> possibleValues;
        public bool colapsed;

        public Cell()
        {
            value = 0;
            possibleValues = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            colapsed = false;
        }
    }
}