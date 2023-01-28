namespace SudokuSolver;

public class Solver
{
    public void SolveSudoku(char[][] board)
    {
        var result = SolveSudokuInner(board);

        CloneBoard(result, board);
        
        CW("Solved");
        PrintBoard(board);
    }
    
    public char[][] SolveSudokuInner(char[][] board)
    {
        var allAvailable = new List<bool[][]>();

        for (int i = 0; i < 9; i++)
        {
            allAvailable.Add(new[]
            {
                new[] {true, true, true, true, true, true, true, true, true},
                new[] {true, true, true, true, true, true, true, true, true},
                new[] {true, true, true, true, true, true, true, true, true},
                new[] {true, true, true, true, true, true, true, true, true},
                new[] {true, true, true, true, true, true, true, true, true},
                new[] {true, true, true, true, true, true, true, true, true},
                new[] {true, true, true, true, true, true, true, true, true},
                new[] {true, true, true, true, true, true, true, true, true},
                new[] {true, true, true, true, true, true, true, true, true}
            });
        }

        while (!IsFinish(board))
        {
            var anyChanged = false;
            var availChanged = false;
            
            for (int num = 1; num < 10; num++)
            {
                var available = allAvailable[num - 1];
                var changed = true;

                while (changed)
                {
                    changed = false;

                    // fill available
                    availChanged |= FindAvailableNum(board, available, num);

                    // fill changes if available
                    for (int row = 0; row < 9; row++)
                    {
                        if (FillRow(board, available, num, row))
                        {
                            changed = true;
                            anyChanged = true;
                            break;
                        }
                    }

                    if (changed)
                        continue;

                    for (int col = 0; col < 9; col++)
                    {
                        if (FillCol(board, available, num, col))
                        {
                            changed = true;
                            anyChanged = true;
                            break;
                        }
                    }

                    if (changed)
                        continue;

                    for (int col = 0; col < 3; col++)
                    {
                        for (int row = 0; row < 3; row++)
                        {
                            if (FillSquare(board, available, num, col, row))
                            {
                                changed = true;
                                anyChanged = true;
                                break;
                            }
                        }

                        if (changed)
                            break;
                    }
                }
            }
            
            if (availChanged || anyChanged)
                continue;

            if (!IsValid(board))
            {
                // BROKEN BOARD
                return null;
            }
            
            // At this point we need to make an assumption and go to recursive call
            // Find cell with two options and try any of them
            var result = MakeAsumption(board, allAvailable);

            return result;
        }

        return IsValid(board) ? board : null;
    }

    private bool FindAvailableNum(char[][] board, bool[][] available, int num)
    {
        var result = false;
        
        for (int row = 0; row < 9; row++)
        {
            result |= AvailableRow(board, available, num, row);
        }

        for (int col = 0; col < 9; col++)
        {
            result |= AvailableCol(board, available, num, col);
        }

        for (int col = 0; col < 3; col++)
        {
            for (int row = 0; row < 3; row++)
            {
                result |= AvailableSquare(board, available, num, col, row);
            }
        }

        return result;
    }

    private bool AvailableRow(char[][] board, bool[][] available, int num, int rowNum)
    {
        var result = false;
        var found = false;
        var numCh = num.ToString()[0];
        for (var i = 0; i < 9; i++)
        {
            if (board[rowNum][i] != '.' && available[rowNum][i])
            {
                result = true;
                available[rowNum][i] = false;
            }

            if (board[rowNum][i] != numCh)
                continue;

            found = true;
            break;
        }

        if (!found)
            return result;

        for (var i = 0; i < 9; i++)
        {
            if (!available[rowNum][i])
                continue;
            
            result = true;
            available[rowNum][i] = false;
        }

        return result;
    }

    private bool AvailableCol(char[][] board, bool[][] available, int num, int colNum)
    {
        var result = false;
        var found = false;
        var numCh = num.ToString()[0];
        for (var i = 0; i < 9; i++)
        {
            if (board[i][colNum] != '.' && available[i][colNum])
            {
                result = true;
                available[i][colNum] = false;
            }

            if (board[i][colNum] != numCh)
                continue;

            found = true;
            break;
        }

        if (!found)
            return result;

        for (var i = 0; i < 9; i++)
        {
            if (!available[i][colNum])
                continue;
            
            result = true;
            available[i][colNum] = false;
        }

        return result;
    }

    private bool AvailableSquare(char[][] board, bool[][] available, int num, int sqColNum, int sqRowNum)
    {
        var result = false;
        var found = false;
        var numCh = num.ToString()[0];
        for (var i = sqRowNum * 3; i < (sqRowNum * 3 + 3); i++)
        {
            for (int j = sqColNum * 3; j < (sqColNum * 3 + 3); j++)
            {
                if (board[i][j] != '.' && available[i][j])
                {
                    result = true;
                    available[i][j] = false;
                }

                if (board[i][j] != numCh)
                    continue;

                found = true;
                break;
            }

            if (found)
                break;

        }

        if (!found)
            return result;

        for (var i = sqRowNum * 3; i < (sqRowNum * 3 + 3); i++)
        {
            for (int j = sqColNum * 3; j < (sqColNum * 3 + 3); j++)
            {
                if (!available[i][j])
                    continue;
                
                result = true;
                available[i][j] = false;
            }
        }

        return result;
    }

    private bool FillRow(char[][] board, bool[][] available, int num, int rowNum)
    {
        var fillId = -1;
        for (var i = 0; i < 9; i++)
        {
            if (available[rowNum][i])
            {
                if (fillId < 0)
                    fillId = i;
                else
                {
                    fillId = -1;
                    break;
                }
            }
        }

        if (fillId < 0)
            return false;
        
        board[rowNum][fillId] = num.ToString()[0];
        
        return true;
    }
    
    private bool FillCol(char[][] board, bool[][] available, int num, int colNum)
    {
        var fillId = -1;
        for (var i = 0; i < 9; i++)
        {
            if (available[i][colNum])
            {
                if (fillId < 0)
                    fillId = i;
                else
                {
                    fillId = -1;
                    break;
                }
            }
        }

        if (fillId < 0)
            return false;
        
        board[fillId][colNum] = num.ToString()[0];
        
        return true;
    }
    
    private bool FillSquare(char[][] board, bool[][] available, int num, int sqColNum, int sqRowNum)
    {
        var fillCol = -1;
        var fillRow = -1;
        var multiHit = false;
        for (var i = sqRowNum*3; i < (sqRowNum * 3 + 3); i++)
        {
            for (int j = sqColNum*3; j < (sqColNum * 3 + 3); j++)
            {
                if (available[i][j])
                {
                    if (fillCol < 0 && fillRow < 0)
                    {
                        fillCol = j;
                        fillRow = i;
                    }
                    else
                    {
                        fillCol = -1;
                        fillRow = -1;
                        multiHit = true;
                        break;
                    }
                }
            }

            if (multiHit)
                break;
        }

        
        if (fillCol < 0 || fillRow < 0)
            return false;
        
        board[fillRow][fillCol] = num.ToString()[0];
        
        return true;
    }

    private bool IsFinish(char[][] board)
    {
        return !board.Any(c => c.Any(x => x == '.'));
    }

    private bool IsValid(char[][] board)
    {
        for (var rowNum = 0; rowNum < 9; rowNum++)
        {
            if (board[rowNum].GroupBy(x => x).Any(x => x.Key != '.' && x.Count() > 1))
                return false;
        }
        
        for (var colNum = 0; colNum < 9; colNum++)
        {
            if (board.Select(x => x[colNum]).GroupBy(x => x).Any(x => x.Key != '.' && x.Count() > 1))
                return false;
        }


        for (var sqRowNum = 0; sqRowNum < 3; sqRowNum++)
        {
            for (var sqColNum = 0; sqColNum < 3; sqColNum++)
            {
                var vals = new List<int>();
                
                for (var i = sqRowNum * 3; i < (sqRowNum * 3 + 3); i++)
                {
                    for (var j = sqColNum * 3; j < (sqColNum * 3 + 3); j++)
                    {
                        vals.Add(board[i][j]);
                    }
                }

                if (vals.GroupBy(x => x).Any(x => x.Key != '.' && x.Count() > 1))
                    return false;
            }
        }

        return true;
    }

    private void PrintBoard(char[][] board)
    {
        Console.WriteLine($"{board[0][0]}{board[0][1]}{board[0][2]}|{board[0][3]}{board[0][4]}{board[0][5]}|{board[0][6]}{board[0][7]}{board[0][8]}");
        Console.WriteLine($"{board[1][0]}{board[1][1]}{board[1][2]}|{board[1][3]}{board[1][4]}{board[1][5]}|{board[1][6]}{board[1][7]}{board[1][8]}");
        Console.WriteLine($"{board[2][0]}{board[2][1]}{board[2][2]}|{board[2][3]}{board[2][4]}{board[2][5]}|{board[2][6]}{board[2][7]}{board[2][8]}");
        Console.WriteLine("---+---+---");
        Console.WriteLine($"{board[3][0]}{board[3][1]}{board[3][2]}|{board[3][3]}{board[3][4]}{board[3][5]}|{board[3][6]}{board[3][7]}{board[3][8]}");
        Console.WriteLine($"{board[4][0]}{board[4][1]}{board[4][2]}|{board[4][3]}{board[4][4]}{board[4][5]}|{board[4][6]}{board[4][7]}{board[4][8]}");
        Console.WriteLine($"{board[5][0]}{board[5][1]}{board[5][2]}|{board[5][3]}{board[5][4]}{board[5][5]}|{board[5][6]}{board[5][7]}{board[5][8]}");
        Console.WriteLine("---+---+---");
        Console.WriteLine($"{board[6][0]}{board[6][1]}{board[6][2]}|{board[6][3]}{board[6][4]}{board[6][5]}|{board[6][6]}{board[6][7]}{board[6][8]}");
        Console.WriteLine($"{board[7][0]}{board[7][1]}{board[7][2]}|{board[7][3]}{board[7][4]}{board[7][5]}|{board[7][6]}{board[7][7]}{board[7][8]}");
        Console.WriteLine($"{board[8][0]}{board[8][1]}{board[8][2]}|{board[8][3]}{board[8][4]}{board[8][5]}|{board[8][6]}{board[8][7]}{board[8][8]}");
    }

    private void CW(string str) => Console.WriteLine(str);

    private char[][] CloneBoard(char[][] board)
    {
        var clone = new char[9][];

        CloneBoard(board, clone);

        return clone;
    }
    
    private void CloneBoard(char[][] from, char[][] to)
    {
        for (int rowNum = 0; rowNum < 9; rowNum++)
        {
            var row = new char[9];

            for (int colNum = 0; colNum < 9; colNum++)
            {
                row[colNum] = from[rowNum][colNum];
            }
            
            to[rowNum] = row;
        }
    }

    private char[][] MakeAsumption(char[][] board, List<bool[][]> allAvailable)
    {
        for (var rowNum = 0; rowNum < 9; rowNum++)
        {
            for (var colNum = 0; colNum < 9; colNum++)
            {
                var options = new List<char>();
                for (int i = 0; i < 9; i++)
                {
                    if(allAvailable[i][rowNum][colNum])
                        options.Add((i + 1).ToString()[0]);
                }
                
                if (options.Count != 2)
                    continue;

                foreach (var option in options)
                {
                    var clone = CloneBoard(board);

                    clone[rowNum][colNum] = option;

                    CW($"Trying asumtion with {option} in {rowNum}{colNum}");
                    PrintBoard(clone);
                    
                    var result = SolveSudokuInner(clone);

                    if (result != null)
                        return result;
                }
            }
        }

        return null;
    }
}