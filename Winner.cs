using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Winner
{
    public static class Winner
    {
        private static string input = string.Empty;
        private static string output = string.Empty;
        private static string inputFile = string.Empty;
        private static string outputFile = string.Empty;
        public static void Main(string[] args)
        {
            try
            {
                ValidateInputInfo(args);

                var players = GetPlayers();

                //Get the best score
                var highestScore = players.OrderByDescending(p => p.Score).FirstOrDefault();

                //Check if there is tie
                var winners = players.Where(p => p.Score == highestScore.Score);

                var names = new List<string>();
                foreach (var w in winners)
                {
                    names.Add($"{w.Name}");
                }

                Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss} {string.Join(",", names)}:{highestScore.Score}");
                WriteToFile($"{string.Join(",", names)}:{highestScore.Score}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss} {ex.Message}");
                WriteToFile("ERROR");
            }
        }

        private static List<Player> GetPlayers()
        {
            string[] lines = File.ReadAllLines(inputFile);

            if (lines.Length < 5)
            {
                throw new Exception("Define at least 5 players");
            }

            var players = new List<Player>();

            foreach (var line in lines)
            {
                string[] splitLine = line.Split(':');

                if (splitLine.Length < 2)
                {
                    throw new Exception($"Invalid line: '{line}'");
                }
                var playerName = splitLine[0];
                var playerHand = splitLine[1];

                string[] cards = playerHand.Split(',');

                var cardList = new List<Card>();
                foreach (var card in cards)
                {
                    ValidateCard(card);
                    cardList.Add(new Card() { Name = card.ToUpper(), Score = CalcScore(card) });
                }

                players.Add(new Player() { Name = playerName, Cards = cardList });
            }

            return players;
        }

        private static void WriteToFile(string Message)
        {
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            using (StreamWriter sw = File.AppendText(outputFile))
            {
                sw.WriteLine(Message);
            }
        }

        private static void ValidateInputInfo(string[] args)
        {
            if (args == null || args.Length < 4)
            {
                throw new Exception("INPUT/OUT parameters are required");
            }

            if (args[0].ToLower() == "--in")
            {
                input = args[1];
                output = args[3];
            }

            if (args[0].ToLower() == "--out")
            {
                input = args[3];
                output = args[1];
            }

            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(output))
            {
                throw new Exception("Invalid parameters");
            }

            inputFile = Path.IsPathRooted(input) ? input : AppDomain.CurrentDomain.BaseDirectory + $"\\{input}";
            outputFile = Path.IsPathRooted(output) ? output : AppDomain.CurrentDomain.BaseDirectory + $"\\{output}";

        }

        private static void ValidateCard(string card)
        {
            var cards = new List<string>
            {
                "AS","AH","AD","AC",
                "1S","1H","1D","1C",
                "2S","2H","2D","2C",
                "3S","3H","3D","3C",
                "4S","4H","4D","4C",
                "5S","5H","5D","5C",
                "6S","6H","6D","6C",
                "7S","7H","7D","7C",
                "8S","8H","8D","8C",
                "9S","9H","9D","9C",
                "10S","10H","10D","10C",
                "JS","JH","JD","JC",
                "QS","QH","QD","QC",
                "KS","KH","KD","KC"
            };

            if (!cards.Any(c => c.Equals(card.ToUpper())))
            {
                throw new Exception($"Invalid card '{card}'");
            }
        }

        private static int CalcScore(string card)
        {
            string faceValue;
            string suit;
            int score = 0;

            if (card.Length == 2)
            {
                faceValue = card.Substring(0, 1);
                suit = card.Substring(1, 1);
            }
            else
            {
                faceValue = card.Substring(0, 2);
                suit = card.Substring(2, 1);
            }

            switch (faceValue.ToUpper())
            {
                case "A":
                    score = 1;
                    break;
                case "2":
                    score = 2;
                    break;
                case "3":
                    score = 3;
                    break;
                case "4":
                    score = 4;
                    break;
                case "5":
                    score = 5;
                    break;
                case "6":
                    score = 6;
                    break;
                case "7":
                    score = 7;
                    break;
                case "8":
                    score = 8;
                    break;
                case "9":
                    score = 9;
                    break;
                case "10":
                    score = 10;
                    break;
                case "J":
                    score = 11;
                    break;
                case "Q":
                    score = 12;
                    break;
                case "K":
                    score = 13;
                    break;
            }

            switch (suit.ToUpper()) // S = Spades, H = Hearts, D = Diamonds and C = Clubs
            {
                case "S":
                    score += 4;
                    break;
                case "H":
                    score += 3;
                    break;
                case "D":
                    score += 2;
                    break;
                case "C":
                    score += 1;
                    break;
            }
            return score;
        }
    }

    public class Card
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }

    public class Player
    {
        public string Name { get; set; }
        public List<Card> Cards { get; set; }
        public virtual int Score
        {
            get
            {
                return Cards.Sum(x => x.Score);
            }
        }
    }
}
