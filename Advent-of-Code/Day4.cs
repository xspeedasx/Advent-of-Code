public static partial class AoCSolution
{
    public static Func<Task> Day4 => async () =>
          {
              Console.WriteLine("===== Day 4 =====");
              var testInput = await File.ReadAllLinesAsync("TestInputs/day4_test.txt");
              var mainInput = await File.ReadAllLinesAsync("Inputs/day4.txt");

              Console.WriteLine("Part 1 solution for test inputs:");
              SolvePart1ForInputs(testInput);
              Console.WriteLine("Part 1 solution for puzzle inputs:");
              SolvePart1ForInputs(mainInput);

              Console.WriteLine("Part 2 solution for test inputs:");
              SolvePart2ForInputs(testInput);
              Console.WriteLine("Part 2 solution for puzzle inputs:");
              SolvePart2ForInputs(mainInput);

              void SolvePart1ForInputs(string[] report)
              {
                  var results = GenerateBoardResults(report);
                  var (firstWon, winScore) = results.MinBy(x => x.wonAt);
                  Console.WriteLine($"\tAnswer: {winScore}");
              }

              void SolvePart2ForInputs(string[] report)
              {
                  var results = GenerateBoardResults(report);
                  var (firstWon, winScore) = results.MaxBy(x => x.wonAt);
                  Console.WriteLine($"\tAnswer: {winScore}");
              }

              IEnumerable<(int wonAt, int score)> GenerateBoardResults(string[] report)
              {
                  var inputs = report[0].Split(',').Select(x => int.Parse(x)).ToArray();
                  var boards = new List<Board>();
                  for (int b = 2; b < report.Length; b += 6)
                  {
                      var boardNumbers = string.Join(" ", report[b..(b + 5)]).Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();
                      boards.Add(new Board(inputs, boardNumbers));
                  }

                  return boards.Select(x => x.GetScore()).Where(x => x.wonAt != -1);
              }
        };

    private class Board
    {
        readonly int[] inputs;
        readonly int[] board;

        public Board(int[] inputs, int[] board)
        {
            this.inputs = inputs;
            this.board = board;
        }

        public (int wonAt, int score) GetScore()
        {
            var startScore = board.Sum();
            var markedRows = new int[5];
            var markedColumns = new int[5];

            for (int i = 0; i < inputs.Length; i++)
            {
                var input = inputs[i];
                var idx = Array.IndexOf(board, input);
                if (idx != -1)
                {
                    startScore -= input;
                    var col = idx % 5;
                    markedColumns[col]++;
                    var row = idx / 5;
                    markedRows[row]++;
                    if (markedColumns[col] == 5 || markedRows[row] == 5)
                    {
                        return (i, input * startScore);
                    }
                }
            }
            return (-1, -1);
        }
    }
}