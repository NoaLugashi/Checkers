using System;

namespace CheckersConsoleUI
{
    public class Move
    {
        private string m_PlayerName;
        private int m_FromRow;
        private int m_FromCol;
        private int m_ToRow;
        private int m_ToCol;

        public Move(int i_FromRow, int i_FromCol, int i_ToRow, int i_ToCol, string i_MovePlayer)
        {
            m_FromRow = i_FromRow;
            m_FromCol = i_FromCol;
            m_ToRow = i_ToRow;
            m_ToCol = i_ToCol;
            m_PlayerName = i_MovePlayer;
        }

        public int FromRow()
        {
            return m_FromRow;
        }

        public int FromCol()
        {
            return m_FromCol;
        }

        public int ToRow()
        {
            return m_ToRow;
        }

        public int ToCol()
        {
            return m_ToCol;
        }

        public string GetMovePlayer()
        {
            return m_PlayerName;
        }

        public bool IsValid(Board i_Board, char i_PlayerSymbol)
        {
            char piece = i_Board.GetCell(m_FromRow, m_FromCol);
            bool isValid = false;
            int rowDiff = Math.Abs(m_ToRow - m_FromRow);
            int colDiff = Math.Abs(m_ToCol - m_FromCol);

            if (piece != i_PlayerSymbol && piece != GetKingSymbol(i_PlayerSymbol) || i_Board.GetCell(m_ToRow, m_ToCol) != (char)PieceType.None)
            {
                isValid = false;
            }
            else if (piece == GetKingSymbol(i_PlayerSymbol))
            {
                isValid = IsValidForKing(i_Board, i_PlayerSymbol);
            }
            else if ((i_PlayerSymbol == (char)PieceType.RegularPlayer1 && m_ToRow >= m_FromRow) ||
                     (i_PlayerSymbol == (char)PieceType.RegularPlayer2 && m_ToRow <= m_FromRow))
            {
                isValid = false;
            }
            else
            {
                isValid = IsValidForRegularPiece(i_Board, i_PlayerSymbol);
            }

            return isValid;
        }

        private bool IsValidForKing(Board i_Board, char i_PlayerSymbol)
        {
            int rowDiff = Math.Abs(m_ToRow - m_FromRow);
            int colDiff = Math.Abs(m_ToCol - m_FromCol);

            if (rowDiff == 1 && colDiff == 1)
            {
                return i_Board.GetCell(m_ToRow, m_ToCol) == (char)PieceType.None;
            }
            if (rowDiff == 2 && colDiff == 2)
            {
                int middleRow = (m_FromRow + m_ToRow) / 2;
                int middleCol = (m_FromCol + m_ToCol) / 2;
                char middlePiece = i_Board.GetCell(middleRow, middleCol);

                return middlePiece != (char)PieceType.None &&
                       middlePiece != i_PlayerSymbol &&
                       middlePiece != GetKingSymbol(i_PlayerSymbol) &&
                       i_Board.GetCell(m_ToRow, m_ToCol) == (char)PieceType.None;
            }

            return false;
        }

        private bool IsValidForRegularPiece(Board i_Board, char i_PlayerSymbol)
        {
            int rowDiff = m_ToRow - m_FromRow;
            int colDiff = Math.Abs(m_ToCol - m_FromCol);
            bool isValid = false;

            if ((i_PlayerSymbol == (char)PieceType.RegularPlayer1 && rowDiff >= 0) ||
                (i_PlayerSymbol == (char)PieceType.RegularPlayer2 && rowDiff <= 0))
            {
                return false;
            }
            if (Math.Abs(rowDiff) == 2 && colDiff == 2)
            {
                int middleRow = (m_FromRow + m_ToRow) / 2;
                int middleCol = (m_FromCol + m_ToCol) / 2;
                char middlePiece = i_Board.GetCell(middleRow, middleCol);

                if (middlePiece != (char)PieceType.None && middlePiece != i_PlayerSymbol && middlePiece != GetKingSymbol(i_PlayerSymbol))
                {
                    isValid = true;
                }
            }
            else if (Math.Abs(rowDiff) == 1 && colDiff == 1)
            {
                isValid = true;
            }

            return isValid;
        }

        public static char GetKingSymbol(char i_PlayerSymbol)
        {
            return i_PlayerSymbol == (char)PieceType.RegularPlayer1 ? (char)PieceType.KingPlayer1 : (char)PieceType.KingPlayer2;
        }

        public override string ToString()
        {
            return $"{(char)('A' + m_FromRow)}{(char)('a' + m_FromCol)}>{(char)('A' + m_ToRow)}{(char)('a' + m_ToCol)}";
        }
    }
}
