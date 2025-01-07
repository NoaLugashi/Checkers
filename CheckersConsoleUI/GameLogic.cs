using System;
using System.Collections.Generic;
using Ex02.ConsoleUtils;

namespace CheckersConsoleUI
{
    internal class GameLogic
    {
        private Board m_Board;
        private Player m_Player1;
        private Player m_Player2;
        private Player m_CurrentPlayer;
        private GameUI m_GameUI;

        public GameLogic(BoardSize i_BoardSize, string i_Player1Name, string i_Player2Name, bool i_IsPlayer2Computer)
        {
            m_Board = new Board(i_BoardSize);
            m_Player1 = new Player(i_Player1Name, (char)PieceType.RegularPlayer1, false);
            m_Player2 = new Player(i_Player2Name, (char)PieceType.RegularPlayer2, i_IsPlayer2Computer);
            m_CurrentPlayer = m_Player1;
            m_GameUI = new GameUI();
        }

        public void StartGame()
        {
            bool playAgain = true;

            while (playAgain)
            {
                Move lastMove = null;
                int? lastRow = null;
                int? lastCol = null;

                while (true)
                {
                    Screen.Clear();
                    DisplayBoard(m_Board);

                    if (lastMove != null)
                    {
                        Console.WriteLine($"{lastMove.GetMovePlayer()}'s last move ({GetOpponent().GetSymbol()}): {lastMove}");
                    }

                    Console.WriteLine($"{m_CurrentPlayer.GetName()}'s turn ({m_CurrentPlayer.GetSymbol()}):");
                    Move currentMove = m_CurrentPlayer.GetMove(m_Board, GetOpponent(), lastRow, lastCol);

                    if (currentMove == null)
                    {
                        IsGameOver(out string gameResultMessageQ);
                        break;
                    }
                    else if (!MakeMove(currentMove))
                    {
                        lastMove = currentMove;
                        lastRow = currentMove.ToRow();
                        lastCol = currentMove.ToCol();

                        Console.WriteLine($"{m_CurrentPlayer.GetName()}'s last move ({m_CurrentPlayer.GetSymbol()}): {lastMove}");
                        Console.WriteLine("Another jump is possible. You must jump again.");

                        continue;
                    }
                    else if (IsGameOver(out string gameResultMessage))
                    {
                        Screen.Clear();
                        DisplayBoard(m_Board);
                        Console.WriteLine(gameResultMessage);

                        break;
                    }

                    lastMove = currentMove;
                    lastRow = null;
                    lastCol = null;

                    SwitchTurn();
                }

                Console.WriteLine("Game Over. Would you like to play another round? (y/n)");
                string response = Console.ReadLine();
                playAgain = response.ToLower() == "y";

                if (playAgain)
                {
                    m_Board.InitializeBoard();
                }
                else
                {
                    Console.WriteLine("Thank you for playing! Goodbye.");
                }
            }
        }

        private void DisplayBoard(Board i_Board)
        {
            int boardSize = i_Board.GetSize();
            char columnLabel = 'a';

            Console.Write("  ");
            for (int col = 0; col < boardSize; col++)
            {
                Console.Write($"  {columnLabel++} ");
            }

            Console.WriteLine();
            Console.WriteLine("  " + new string('=', boardSize * 4));

            for (int row = 0; row < boardSize; row++)
            {
                Console.Write($"{(char)('A' + row)} ");
                for (int col = 0; col < boardSize; col++)
                {
                    Console.Write($"| {i_Board.GetCell(row, col)} ");
                }
                Console.WriteLine("|");
                Console.WriteLine("  " + new string('=', boardSize * 4));
            }
        }

        private Player GetOpponent()
        {
            return m_CurrentPlayer == m_Player1 ? m_Player2 : m_Player1;
        }

        private bool MakeMove(Move i_Move)
        {
            bool moveCompleted = true;
            char updatedPiece = UpdateBoard(i_Move);
            int rowDiff = Math.Abs(i_Move.ToRow() - i_Move.FromRow());
            int colDiff = Math.Abs(i_Move.ToCol() - i_Move.FromCol());

            if (rowDiff == 2 && colDiff == 2)
            {
                if (CanJumpOver(m_Board, i_Move.ToRow(), i_Move.ToCol(), updatedPiece))
                {
                    Console.WriteLine("Another jump is possible. You must jump again.");
                    moveCompleted = false;
                }
            }

            return moveCompleted;
        }

        public static bool HasMandatoryJump(char i_PlayerSymbol, Board i_Board)
        {
            bool hasJump = false;
            for (int row = 0; row < i_Board.GetSize(); row++)
            {
                for (int col = 0; col < i_Board.GetSize(); col++)
                {
                    if (i_Board.GetCell(row, col) == i_PlayerSymbol)
                    {
                        if (CanJumpOver(i_Board, row, col, i_PlayerSymbol))
                        {
                            hasJump = true;
                        }
                    }
                }
            }

            return hasJump;
        }

        private static bool CanJumpOver(Board i_Board, int i_Row, int i_Col, char i_PlayerSymbol)
        {
            int[] rowOffsets = { -2, 2 };
            int[] colOffsets = { -2, 2 };
            bool canJump = false;
            bool isKing = (i_PlayerSymbol == (char)PieceType.KingPlayer1 || i_PlayerSymbol == (char)PieceType.KingPlayer2);

            foreach (int rowOffset in rowOffsets)
            {
                foreach (int colOffset in colOffsets)
                {
                    int jumpRow = i_Row + rowOffset;
                    int jumpCol = i_Col + colOffset;

                    if (jumpRow >= 0 && jumpRow < i_Board.GetSize() &&
                        jumpCol >= 0 && jumpCol < i_Board.GetSize())
                    {
                        int middleRow = (i_Row + jumpRow) / 2;
                        int middleCol = (i_Col + jumpCol) / 2;
                        char middlePiece = i_Board.GetCell(middleRow, middleCol);
                        char targetCell = i_Board.GetCell(jumpRow, jumpCol);

                        if (middlePiece != (char)PieceType.None &&
                            middlePiece != i_PlayerSymbol &&
                            middlePiece != Move.GetKingSymbol(i_PlayerSymbol) &&
                            targetCell == (char)PieceType.None)
                        {
                            if (isKing ||
                                (i_PlayerSymbol == (char)PieceType.RegularPlayer1 && rowOffset == -2) ||
                                (i_PlayerSymbol == (char)PieceType.RegularPlayer2 && rowOffset == 2))
                            {
                                canJump = true;
                            }
                        }
                    }
                }
            }

            return canJump;
        }

        private char UpdateBoard(Move i_Move)
        {
            char movingPiece = m_Board.GetCell(i_Move.FromRow(), i_Move.FromCol());
            int rowDiff = i_Move.FromRow() - i_Move.ToRow();
            m_Board.SetCell(i_Move.FromRow(), i_Move.FromCol(), (char)PieceType.None);

            if (Math.Abs(rowDiff) == 2)
            {
                int middleRow = (i_Move.FromRow() + i_Move.ToRow()) / 2;
                int middleCol = (i_Move.FromCol() + i_Move.ToCol()) / 2;
                m_Board.SetCell(middleRow, middleCol, (char)PieceType.None);
            }
            if ((movingPiece == (char)PieceType.RegularPlayer1 && i_Move.ToRow() == 0) ||
                (movingPiece == (char)PieceType.RegularPlayer2 && i_Move.ToRow() == m_Board.GetSize() - 1))
            {
                movingPiece = movingPiece == (char)PieceType.RegularPlayer1 ? (char)PieceType.KingPlayer1 : (char)PieceType.KingPlayer2;
            }

            m_Board.SetCell(i_Move.ToRow(), i_Move.ToCol(), movingPiece);
            return movingPiece;
        }

        private void SwitchTurn()
        {
            m_CurrentPlayer = m_CurrentPlayer == m_Player1 ? m_Player2 : m_Player1;
        }

        private int CountPieces(char i_Symbol)
        {
            int count = 0;

            for (int row = 0; row < m_Board.GetSize(); row++)
            {
                for (int col = 0; col < m_Board.GetSize(); col++)
                {
                    char piece = m_Board.GetCell(row, col);

                    if (piece == i_Symbol)
                    {
                        count += 1;
                    }
                    else if (IsKingOfPlayer(i_Symbol, piece))
                    {
                        count += 4;
                    }
                }
            }

            return count;
        }

        private bool IsKingOfPlayer(char i_Symbol, char i_Piece)
        {
            return (i_Symbol == (char)PieceType.RegularPlayer1 && i_Piece == (char)PieceType.KingPlayer1) ||
                   (i_Symbol == (char)PieceType.RegularPlayer2 && i_Piece == (char)PieceType.KingPlayer2);
        }

        private bool HasLegalMoves(Player i_Player)
        {
            bool hasMoves = false;

            for (int row = 0; row < m_Board.GetSize(); row++)
            {
                for (int col = 0; col < m_Board.GetSize(); col++)
                {
                    char piece = m_Board.GetCell(row, col);

                    if (IsPieceOfPlayer(piece, i_Player.GetSymbol()))
                    {
                        List<Move> validMoves = m_Board.GetValidMovesForPiece(row, col, piece, i_Player.GetName());
                        if (validMoves.Count > 0)
                        {
                            hasMoves = true;
                            break;
                        }
                    }
                }

                if (hasMoves)
                {
                    break;
                }
            }

            return hasMoves;
        }

        private bool IsPieceOfPlayer(char i_Piece, char i_PlayerSymbol)
        {
            return (i_PlayerSymbol == (char)PieceType.RegularPlayer1 &&
                    (i_Piece == (char)PieceType.RegularPlayer1 || i_Piece == (char)PieceType.KingPlayer1)) ||
                   (i_PlayerSymbol == (char)PieceType.RegularPlayer2 &&
                    (i_Piece == (char)PieceType.RegularPlayer2 || i_Piece == (char)PieceType.KingPlayer2));
        }

        private bool IsGameOver(out string o_GameResultMessage)
        {
            int player1PiecesScore = CountPieces(m_Player1.GetSymbol());
            int player2PiecesScore = CountPieces(m_Player2.GetSymbol());
            int scoreDifference = Math.Abs(player1PiecesScore - player2PiecesScore);
            bool isGameOver = false;

            if (player1PiecesScore == 0)
            {
                m_Player2.AddToScore(scoreDifference);
                o_GameResultMessage = $"{m_Player2.GetName()} wins! Score: {m_Player2.GetName()}: {m_Player2.GetTotalScore()}" +
                                      $"\n{m_Player1.GetName()}: {m_Player1.GetTotalScore()}";
                isGameOver = true;
            }
            else if (player2PiecesScore == 0)
            {
                m_Player1.AddToScore(scoreDifference);
                o_GameResultMessage = $"{m_Player1.GetName()} wins! Score: {m_Player1.GetName()}: {m_Player1.GetTotalScore()}" +
                                      $"\n{m_Player2.GetName()}: {m_Player2.GetTotalScore()}";
                isGameOver = true;
            }
            else if (!HasLegalMoves(m_Player1) && !HasLegalMoves(m_Player2))
            {
                if (m_Player1.GetTotalScore() > m_Player2.GetTotalScore())
                {
                    m_Player1.AddToScore(scoreDifference);
                    o_GameResultMessage = $"{m_Player1.GetName()} wins by score! Score: {m_Player1.GetName()}: {m_Player1.GetTotalScore()}" +
                                          $"\n{m_Player2.GetName()}: {m_Player2.GetTotalScore()}";
                }
                else if (m_Player2.GetTotalScore() > m_Player1.GetTotalScore())
                {
                    m_Player2.AddToScore(scoreDifference);
                    o_GameResultMessage = $"{m_Player2.GetName()} wins by score! Score: {m_Player2.GetName()}: {m_Player2.GetTotalScore()}" +
                                          $"\n{m_Player1.GetName()}: {m_Player1.GetTotalScore()}";
                }
                else
                {
                    o_GameResultMessage = $"It's a draw! Both players have the same score: {m_Player2.GetTotalScore()}";
                }

                isGameOver = true;
            }
            else if (!HasLegalMoves(m_CurrentPlayer))
            {
                o_GameResultMessage = $"{GetOpponent().GetName()} wins! Score: {GetOpponent().GetName()}: {GetOpponent().GetTotalScore()}" +
                                      $"\n{m_CurrentPlayer.GetName()}: {m_CurrentPlayer.GetTotalScore()}";
                isGameOver = true;
            }
            else
            {
                o_GameResultMessage = string.Empty;
            }

            return isGameOver;
        }
    }
}