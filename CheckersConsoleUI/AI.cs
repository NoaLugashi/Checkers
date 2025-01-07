using System;
using System.Collections.Generic;

namespace CheckersConsoleUI
{
    public class AI
    {
        public static Move GetMove(Board i_Board)
        {
            List<Move> validMoves = GetValidMoves(i_Board);
            if (validMoves.Count == 0)
            {
                return null;
            }

            List<Move> captureMoves = validMoves.FindAll(IsCaptureMove);

            if (captureMoves.Count > 0)
            {
                return GetBestMove(i_Board, captureMoves);
            }

            return GetBestMove(i_Board, validMoves);
        }

        private static List<Move> GetValidMoves(Board i_Board)
        {
            List<Move> validMoves = new List<Move>();

            for (int row = 0; row < i_Board.GetSize(); row++)
            {
                for (int col = 0; col < i_Board.GetSize(); col++)
                {
                    char piece = i_Board.GetCell(row, col);

                    if (piece == (char)PieceType.RegularPlayer2 || piece == (char)PieceType.KingPlayer2)
                    {
                        List<Move> pieceMoves = i_Board.GetValidMovesForPiece(row, col, piece);

                        foreach (Move move in pieceMoves)
                        {
                            if (piece == (char)PieceType.RegularPlayer2 && move.ToRow() < move.FromRow())
                            {
                                continue;
                            }

                            validMoves.Add(move);
                        }
                    }
                }
            }

            return validMoves;
        }

        private static Move GetBestMove(Board i_Board, List<Move> moves)
        {
            Move bestMove = null;
            int bestScore = int.MinValue;

            foreach (Move move in moves)
            {
                int score = EvaluateMove(i_Board, move);

                if (IsCaptureMove(move))
                {
                    score += 50;
                }
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        private static int EvaluateMove(Board i_Board, Move i_Move)
        {
            int score = 0;

            if (IsCaptureMove(i_Move))
            {
                score += 10;
            }
            char piece = i_Board.GetCell(i_Move.FromRow(), i_Move.FromCol());

            if (IsPromotionMove(i_Board, i_Move, piece))
            {
                score += 20;
            }
            if (IsPieceInDangerAfterMove(i_Board, i_Move))
            {
                score -= 15;
            }

            return score;
        }

        private static bool IsCaptureMove(Move i_Move)
        {
            return Math.Abs(i_Move.ToRow() - i_Move.FromRow()) == 2 &&
                   Math.Abs(i_Move.ToCol() - i_Move.FromCol()) == 2;
        }

        private static bool IsPromotionMove(Board i_Board, Move i_Move, char i_Piece)
        {
            return (i_Piece == (char)PieceType.RegularPlayer2 && i_Move.ToRow() == i_Board.GetSize() - 1) ||
                   (i_Piece == (char)PieceType.KingPlayer2 && i_Move.ToRow() == 0);
        }

        private static bool IsPieceInDangerAfterMove(Board i_Board, Move i_Move)
        {
            bool isInDanger = false;
            Board simulatedBoard = Clone(i_Board);
            simulatedBoard.SetCell(i_Move.ToRow(), i_Move.ToCol(), i_Board.GetCell(i_Move.FromRow(), i_Move.FromCol()));
            simulatedBoard.SetCell(i_Move.FromRow(), i_Move.FromCol(), (char)PieceType.None);

            for (int row = 0; row < simulatedBoard.GetSize(); row++)
            {
                for (int col = 0; col < simulatedBoard.GetSize(); col++)
                {
                    char opponentPiece = simulatedBoard.GetCell(row, col);

                    if (opponentPiece == (char)PieceType.RegularPlayer1 || opponentPiece == (char)PieceType.KingPlayer1)
                    {
                        List<Move> opponentMoves = simulatedBoard.GetValidMovesForPiece(row, col, opponentPiece);
                        foreach (Move move in opponentMoves)
                        {
                            if (move.ToRow() == i_Move.ToRow() && move.ToCol() == i_Move.ToCol())
                            {
                                isInDanger = true;
                                break;
                            }
                        }
                        if (isInDanger)
                        {
                            break;
                        }
                    }
                }
                if (isInDanger)
                {
                    break;
                }
            }

            return isInDanger;
        }

        private static Board Clone(Board i_Board)
        {
            Board clonedBoard = new Board((BoardSize)i_Board.GetSize());

            for (int row = 0; row < i_Board.GetSize(); row++)
            {
                for (int col = 0; col < i_Board.GetSize(); col++)
                {
                    clonedBoard.SetCell(row, col, i_Board.GetCell(row, col));
                }
            }

            return clonedBoard;
        }
    }
}
