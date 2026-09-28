namespace ConnectFour;

public sealed record GameMove(int Number, int Player, int Column);

public sealed class GameState
{
    public enum WinState
    {
        No_Winner = 0,
        Player1_Wins = 1,
        Player2_Wins = 2,
        Tie = 3
    }

        public const int RowCount = 6;
        public const int ColumnCount = 7;

    public int PlayerTurn => TheBoard.Count(piece => piece != 0) % 2 + 1;
    public int CurrentTurn => TheBoard.Count(piece => piece != 0);
    public List<int> TheBoard { get; private set; } = new(new int[RowCount * ColumnCount]);
    public List<GameMove> MoveHistory { get; } = [];

    public byte PlayPiece(int column)
    {
        if (column < 0 || column >= ColumnCount)
        {
            throw new ArgumentException("Choose a valid column.", nameof(column));
        }

        if (CheckForWin() != WinState.No_Winner)
        {
            throw new ArgumentException("The game is over.");
        }

        var landingIndex = column;
        for (var index = column; index < RowCount * ColumnCount; index += ColumnCount)
        {
            if (TheBoard[index] != 0)
            {
                break;
            }

            landingIndex = index;
        }

        if (TheBoard[landingIndex] != 0)
        {
            throw new ArgumentException("That column is full.");
        }

        var player = PlayerTurn;
        TheBoard[landingIndex] = player;
        MoveHistory.Add(new GameMove(CurrentTurn, player, column + 1));
        return (byte)(landingIndex / ColumnCount + 1);
    }

    public WinState CheckForWin()
    {
        for (var row = 0; row < RowCount; row++)
        {
            for (var column = 0; column < ColumnCount; column++)
            {
                var player = TheBoard[row * ColumnCount + column];
                if (player == 0)
                {
                    continue;
                }

                if (HasFour(row, column, player, 0, 1)
                    || HasFour(row, column, player, 1, 0)
                    || HasFour(row, column, player, 1, 1)
                    || HasFour(row, column, player, 1, -1))
                {
                    return (WinState)player;
                }
            }
        }

        return CurrentTurn == RowCount * ColumnCount ? WinState.Tie : WinState.No_Winner;
    }

    public bool IsColumnFull(int column) => TheBoard[column] != 0;

    public void ResetBoard()
    {
        TheBoard = new List<int>(new int[RowCount * ColumnCount]);
        MoveHistory.Clear();
    }

    private bool HasFour(int row, int column, int player, int rowStep, int columnStep)
    {
        var endRow = row + 3 * rowStep;
        var endColumn = column + 3 * columnStep;

        if (endRow < 0 || endRow >= RowCount || endColumn < 0 || endColumn >= ColumnCount)
        {
            return false;
        }

        for (var offset = 1; offset < 4; offset++)
        {
            if (TheBoard[(row + offset * rowStep) * ColumnCount + column + offset * columnStep] != player)
            {
                return false;
            }
        }

        return true;
    }
}