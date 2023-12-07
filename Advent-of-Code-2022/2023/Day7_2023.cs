using System.Diagnostics;

namespace Advent_of_Code_2022._2023;

public static class Day7_2023
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        // part 1 & 2
        var sw = new Stopwatch();
        sw.Start();
        
        //Solve(File.ReadAllLines(testInputPath));
        Solve(File.ReadAllLines(challengeInputPath));
        
        sw.Stop();
        Console.WriteLine($"Solution took: {sw.ElapsedMilliseconds} ms");
    }

    record Hand(string Cards, HandType Type, int bid);

    class RankComparer : IComparer<string>
    {
        private readonly Func<char, int> _rankGetter;

        public RankComparer(Func<char, int> rankGetter)
        {
            _rankGetter = rankGetter;
        }
        
        public int Compare(string? a, string? b)
        {
            if (a is null || b is null) return 0;
            
            for (var i = 0; i < a.Length; i++)
            {
                int aRank = _rankGetter(a[i]);
                int bRank = _rankGetter(b[i]);
                if (aRank < bRank) return -1;
                if (aRank > bRank) return 1;
            }

            return 0;
        }
    }
    private static void Solve(string[] lines)
    {
        Part1(lines);
        Part2(lines);
    }

    private static void Part1(string[] lines)
    {
        var hands = lines.Select(line =>
        {
            string hand = line[..5];
            int bid = int.Parse(line[6..]);
            HandType type = DetermineType(hand);
            return new Hand(hand, type, bid);
        });

        long sum = 0;
        IOrderedEnumerable<Hand> ordered =
            hands.OrderByDescending(x => x.Type).ThenByDescending(x => x.Cards, new RankComparer(GetRank));
        var cnt = 0;
        foreach (var hand in ordered)
        {
            int index = lines.Length - cnt;
            sum += hand.bid * index;
            cnt++;
        }

        Console.WriteLine($"Part 1: {sum}");
    }

    private static void Part2(string[] lines)
    {
        var hands = lines.Select(line =>
        {
            string hand = line[..5];
            int bid = int.Parse(line[6..]);
            HandType type = DetermineType2(hand);
            return new Hand(hand, type, bid);
        });

        long sum = 0;
        IOrderedEnumerable<Hand> ordered =
            hands.OrderByDescending(x => x.Type).ThenByDescending(x => x.Cards, new RankComparer(GetRank2));
        var cnt = 0;
        foreach (var hand in ordered)
        {
            int index = lines.Length - cnt;
            sum += hand.bid * index;
            cnt++;
        }

        Console.WriteLine($"Part 2: {sum}");
    }

    enum HandType { HIGH_CARD, ONE_PAIR, TWO_PAIR, THREE_OF_KIND, FULL_HOUSE, FOUR_OF_KIND, FIVE_OF_KIND }
    private static HandType DetermineType(string hand)
    {
        IGrouping<char, char>[] groups = hand.GroupBy(x => x).ToArray();
        if (groups.Length == 1 && groups[0].Count() == 5)
        {
            return HandType.FIVE_OF_KIND;
        }
        if (groups.Length == 2)
        {
            int max = groups.Max(g => g.Count());
            return max == 4 
                ? HandType.FOUR_OF_KIND 
                : HandType.FULL_HOUSE;
        }
        if (groups.Length == 3)
        {
            int max = groups.Max(g => g.Count());
            return max == 3
                ? HandType.THREE_OF_KIND
                : HandType.TWO_PAIR;
        }

        return groups.Length == 4
            ? HandType.ONE_PAIR
            : HandType.HIGH_CARD;
    }
    
    private static HandType DetermineType2(string hand)
    {
        IGrouping<char, char>[] groups = hand.GroupBy(x => x).ToArray();
        if (groups.Length == 1) return HandType.FIVE_OF_KIND;
        IGrouping<char, char>? jGroup = groups.FirstOrDefault(x => x.Key == 'J');
        if (jGroup is null) return DetermineType(hand);

        var maxType = HandType.HIGH_CARD;
        foreach (var group in groups.Where(x => x.Key != 'J'))
        {
            var type = DetermineType(hand.Replace('J', group.Key));
            if (type > maxType) maxType = type;
        }

        return maxType;
    }

    private static int GetRank(char c) => c switch
    {
        'A' => 14,
        'K' => 13,
        'Q' => 12,
        'J' => 11,
        'T' => 10,
        '9' => 9,
        '8' => 8,
        '7' => 7,
        '6' => 6,
        '5' => 5,
        '4' => 4,
        '3' => 3,
        '2' => 2,
        _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
    };

    private static int GetRank2(char c) => c switch
    {
        'A' => 14,
        'K' => 13,
        'Q' => 12,
        'T' => 10,
        '9' => 9,
        '8' => 8,
        '7' => 7,
        '6' => 6,
        '5' => 5,
        '4' => 4,
        '3' => 3,
        '2' => 2,
        'J' => 1,
        _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
    };
}