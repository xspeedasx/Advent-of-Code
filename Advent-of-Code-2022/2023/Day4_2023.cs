using System.Text.RegularExpressions;

namespace Advent_of_Code_2022._2023;

public static class Day4_2023
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        // part 1 & 2
        Solve(File.ReadAllLines(testInputPath));
        Solve(File.ReadAllLines(challengeInputPath));
    }
    
    private static void Solve(string[] cardLines)
    {
        var sum = 0;
        var cards = new Dictionary<int, Card>();
        var winQueue = new Queue<Card>();

        foreach (string cardLine in cardLines)
        {
            Match idMatch = Regex.Match(cardLine, @"Card +(\d+): ");
            int id = int.Parse(idMatch.Groups[1].Value);

            string[] splits = cardLine[idMatch.Length..].Split("|", StringSplitOptions.TrimEntries);
            int[] winningNums = splits[0].Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            int[] cardNums = splits[1].Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            int[] myWins = cardNums.Intersect(winningNums).ToArray();
            var card = new Card(id, myWins.Length);
            cards[id] = card;
            winQueue.Enqueue(card);
            var pts = (int)Math.Pow(2, myWins.Length - 1);
            sum += pts;
            Console.WriteLine($"Card {id}: {pts} pts. ({string.Join(" ", myWins)})");
        }
        Console.WriteLine($"part 1 answer: {sum}");

        var cardCount = 0;
        while (winQueue.Any())
        {
            Card card = winQueue.Dequeue();
            for (int i = 0; i < card.winCount; i++)
            {
                var nextId = card.id + i + 1; 
                winQueue.Enqueue(cards[nextId]);
            }
            
            cardCount++;
        }
        Console.WriteLine($"part 2 answer: {cardCount}");

    }

    private record Card(int id, int winCount);
}