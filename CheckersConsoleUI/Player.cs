using System;

namespace CheckersConsoleUI
{
    internal class Player
    {
        private string m_Name;
        private char m_Symbol;
        private bool m_IsComputer;
        private int m_TotalScore;
        
        public Player(string i_Name, char i_Symbol, bool i_IsComputer)
        {
            m_Name = i_Name;
            m_Symbol = i_Symbol;
            m_IsComputer = i_IsComputer;
            m_TotalScore = 0;
        }

        public string GetName()
        {
            return m_Name;
        }

        public char GetSymbol()
        {
            char o_Symbol = m_Symbol;

            if (m_Symbol == (char)PieceType.KingPlayer1)
                o_Symbol = (char)PieceType.RegularPlayer1;
            if (m_Symbol == (char)PieceType.KingPlayer2)
                o_Symbol = (char)PieceType.RegularPlayer2;

            return o_Symbol;
        }

        public int GetTotalScore()
        {
            return m_TotalScore;
        }

        public void AddToScore(int i_Score)
        {
            m_TotalScore += i_Score;
        }

        public bool HasJump(Board i_board)
        {
            bool hasJump = false;

            if (GameLogic.HasMandatoryJump(GetSymbol(), i_board))
            {
                hasJump = true;
            }

            return hasJump;
        }

        public Move GetMove(Board i_Board, Player i_Player, int? i_LastRow = null, int? i_LastCol = null)
        {
            Move finalMove = null;

            if (m_IsComputer)
            {
                Console.WriteLine("(press ‘enter’ to see it’s move)");
                Console.ReadLine();
                finalMove = AI.GetMove(i_Board);
            }
            else
            {
                while (finalMove == null)
                {
                    string input = Console.ReadLine();

                    if (input == "Q")
                    {
                        Console.WriteLine($"{m_Name} has quit the game! {i_Player.GetName()} win!");

                        break;
                    }
                    else if (TryParseMove(input, i_Board, out int fromRow, out int fromCol, out int toRow, out int toCol))
                    {
                        int rowDiff = Math.Abs(toRow - fromRow);
                        int colDiff = Math.Abs(toCol - fromCol);

                        if (HasJump(i_Board) && rowDiff != 2 && colDiff != 2)
                        {
                            Console.WriteLine("You must jump. Please try again.");
                            continue;
                        }
                        if (i_LastRow.HasValue && i_LastCol.HasValue &&
                            (fromRow != i_LastRow || fromCol != i_LastCol))
                        {
                            Console.WriteLine("You must continue with the same piece. Please try again.");
                            continue;
                        }
                        Move move = new Move(fromRow, fromCol, toRow, toCol, m_Name);

                        if (move.IsValid(i_Board, m_Symbol))
                        {
                            finalMove = move;
                        }
                        else
                        {
                            Console.WriteLine("Invalid move. Please try again.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter your move in the format ROWcol>ROWcol (e.g., Fb>Fa), or 'Q' to quit.");
                    }
                }
            }
            return finalMove;
        }

        private bool TryParseMove(string i_Input, Board i_Board, out int o_FromRow, out int o_FromCol, out int o_ToRow, out int o_ToCol)
        {
            o_FromRow = o_FromCol = o_ToRow = o_ToCol = -1;
            bool isValid = false;

            if (!string.IsNullOrEmpty(i_Input) && i_Input.Length == 5 && i_Input[2] == '>')
            {
                if (TryParsePosition(i_Input.Substring(0, 2), i_Board, out o_FromRow, out o_FromCol) &&
                    TryParsePosition(i_Input.Substring(3, 2), i_Board, out o_ToRow, out o_ToCol))
                {
                    isValid = true;
                }
            }

            return isValid;
        }

        private bool TryParsePosition(string i_Position, Board i_Board, out int o_Row, out int o_Col)
        {
            o_Row = o_Col = -1;
            bool isValid = false;

            if (i_Position.Length == 2 && char.IsUpper(i_Position[0]) && char.IsLower(i_Position[1]))
            {
                o_Row = i_Position[0] - 'A';
                o_Col = i_Position[1] - 'a';

                if (o_Row >= 0 && o_Row < i_Board.GetSize() && o_Col >= 0 && o_Col < i_Board.GetSize())
                {
                    isValid = true;
                }
                else
                {
                    Console.WriteLine("Parsed position is out of bounds.");
                }
            }
            else
            {
                Console.WriteLine("Position format invalid. Please use the format 'Aa'.");
            }

            return isValid;
        }
    }
}