using System;

namespace CheckersConsoleUI
{
    internal class GameUI
    {
        public static void PlayCheckers()
        {
            Console.WriteLine("Select Board Size (6, 8, or 10):");
            BoardSize boardSize = GetBoardSize();

            Console.WriteLine("Enter Player X name:");
            string player1Name = Console.ReadLine();

            Console.WriteLine("Enter Player O name (or type 'Computer' to play against AI):");
            string player2Name = Console.ReadLine();
            bool isPlayer2Computer = player2Name.Equals("Computer", StringComparison.OrdinalIgnoreCase);

            GameLogic gameLogic = new GameLogic(boardSize, player1Name, player2Name, isPlayer2Computer);
            gameLogic.StartGame();
        }

        private static BoardSize GetBoardSize()
        {
            while (true)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out int size) && (size == 6 || size == 8 || size == 10))
                {
                    return (BoardSize)size;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 6, 8, or 10:");
                }
            }
        }
    }
}