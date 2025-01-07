using System.Collections.Generic;

namespace CheckersConsoleUI
{
    public class Board
    {
        private char[,] m_Board;
        private int m_Size;

        public Board(BoardSize i_BoardSize)
        {
            m_Size = (int)i_BoardSize;
            m_Board = new char[m_Size, m_Size];
            InitializeBoard();
        }

        public void InitializeBoard()
        {
            int rowsToFill = m_Size == (int)BoardSize.Large ? 4 : (m_Size == (int)BoardSize.Small ? 2 : 3);

            for (int row = 0; row < m_Size; ++row)
            {
                for (int col = 0; col < m_Size; ++col)
                {
                    if ((row + col) % 2 != 0)
                    {
                        if (row < rowsToFill)
                        {
                            m_Board[row, col] = (char)PieceType.RegularPlayer2;
                        }
                        else if (row >= m_Size - rowsToFill)
                        {
                            m_Board[row, col] = (char)PieceType.RegularPlayer1;
                        }
                        else
                        {
                            m_Board[row, col] = (char)PieceType.None;
                        }
                    }
                    else
                    {
                        m_Board[row, col] = (char)PieceType.None;
                    }
                }
            }
        }

        public void SetCell(int i_Row, int i_Col, char i_Value)
        {
            if (i_Row >= 0 && i_Row < m_Size && i_Col >= 0 && i_Col < m_Size)
            {
                m_Board[i_Row, i_Col] = i_Value;
            }
        }

        public char GetCell(int i_Row, int i_Col)
        {
            return m_Board[i_Row, i_Col];
        }

        public int GetSize()
        {
            return this.m_Size;
        }

        public List<Move> GetValidMovesForPiece(int i_FromRow, int i_FromCol, char i_Piece, string i_PlayerName = "Computer", bool i_OnlyJumps = false)
        {
            List<Move> validMoves = new List<Move>();

            if (m_Board[i_FromRow, i_FromCol] == i_Piece)
            {
                int[] rowOffsets = { -1, 1 };
                int[] colOffsets = { -1, 1 };

                foreach (int rowOffset in rowOffsets)
                {
                    foreach (int colOffset in colOffsets)
                    {
                        int toRow = i_FromRow + rowOffset;
                        int toCol = i_FromCol + colOffset;

                        if (!i_OnlyJumps && isWithinBounds(toRow, toCol))
                        {
                            Move move = new Move(i_FromRow, i_FromCol, toRow, toCol, i_PlayerName);
                            if (move.IsValid(this, i_Piece))
                            {
                                validMoves.Add(move);
                            }
                        }

                        int jumpRow = i_FromRow + 2 * rowOffset;
                        int jumpCol = i_FromCol + 2 * colOffset;

                        if (isWithinBounds(jumpRow, jumpCol))
                        {
                            Move jumpMove = new Move(i_FromRow, i_FromCol, jumpRow, jumpCol, i_PlayerName);
                            if (jumpMove.IsValid(this, i_Piece))
                            {
                                validMoves.Add(jumpMove);
                            }
                        }
                    }
                }
            }

            return validMoves;
        }

        private bool isWithinBounds(int i_Row, int i_Col)
        {
            return i_Row >= 0 && i_Row < m_Board.GetLength(0) &&
                   i_Col >= 0 && i_Col < m_Board.GetLength(1);
        }
    }
}