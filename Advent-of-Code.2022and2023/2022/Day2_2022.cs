namespace Advent_of_Code._2022;

public static class Day2_2022
{
    private enum Shapes
    {
        ROCK = 1,
        PAPER,
        SCISSORS
    }

    private static Dictionary<char, Shapes> OpponentMap = new()
    {
        ['A'] = Shapes.ROCK,
        ['B'] = Shapes.PAPER,
        ['C'] = Shapes.SCISSORS
    };

    private static Dictionary<char, Shapes> MyMap = new()
    {
        ['X'] = Shapes.ROCK,
        ['Y'] = Shapes.PAPER,
        ['Z'] = Shapes.SCISSORS
    };
    
    public static void Run(string testInputPath, string challengeInputPath)
    {
        Solve(File.ReadAllLines(testInputPath));
        Solve(File.ReadAllLines(challengeInputPath));
    }

    private static void Solve(string[] lines)
    {
        Console.WriteLine("--- Part 1 ---");
        var sum = 0;
        var roundCnt = 1; 
        foreach (string round in lines)
        {
            (string opponentHandString, string myHandString, _) = round.Split(' ');
            Shapes opponentHand = OpponentMap.GetValueOrDefault(opponentHandString[0]); 
            Shapes myHand = MyMap.GetValueOrDefault(myHandString[0]);

            var pointsForShape = (int) myHand;
            
            int outcome = CalculateOutcomePoints(opponentHand, myHand);

            int roundSum = pointsForShape + outcome; 
            sum += roundSum;
            Console.WriteLine($"Round {roundCnt++}: {pointsForShape} + {outcome} = {roundSum}");
        }

        Console.WriteLine($"Total Points: {sum}");
        
        Console.WriteLine("--- Part 2 ---");
        sum = 0;
        roundCnt = 1; 
        foreach (string round in lines)
        {
            (string opponentHandString, string outcomeString, _) = round.Split(' ');
            Shapes opponentHand = OpponentMap.GetValueOrDefault(opponentHandString[0]);
            Shapes myHand = outcomeString[0] switch
            {
                'X' => opponentHand switch // lose
                {
                    Shapes.ROCK => Shapes.SCISSORS,
                    Shapes.PAPER => Shapes.ROCK, 
                    _ => Shapes.PAPER // scissors
                },
                'Y' => opponentHand, // draw
                _ => opponentHand switch // 'Z', win
                {
                    Shapes.ROCK => Shapes.PAPER,
                    Shapes.PAPER => Shapes.SCISSORS, 
                    _ => Shapes.ROCK // scissors
                }
            };

            var pointsForShape = (int) myHand;
            int outcome = CalculateOutcomePoints(opponentHand, myHand);

            int roundSum = pointsForShape + outcome; 
            sum += roundSum;
            Console.WriteLine($"Round {roundCnt++}: {pointsForShape} + {outcome} = {roundSum}");
        }

        Console.WriteLine($"Total Points: {sum}");
    }

    private static int CalculateOutcomePoints(Shapes opponentHand, Shapes myHand)
    {
        return opponentHand switch
        {
            Shapes.ROCK => myHand switch
            {
                Shapes.ROCK => 3,
                Shapes.PAPER => 6,
                _ => 0 // scissors
            },
            Shapes.PAPER => myHand switch
            {
                Shapes.ROCK => 0,
                Shapes.PAPER => 3,
                _ => 6 // scissors
            },
            _ => myHand switch // scissors
            {
                Shapes.ROCK => 6,
                Shapes.PAPER => 0,
                _ => 3 // scissors
            },
        };
    }
}
