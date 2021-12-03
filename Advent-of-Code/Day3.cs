public static partial class AoCSolution
{
    public static Func<Task> Day3 => async () =>
      {
          Console.WriteLine("===== Day 3 =====");
          var testInput = await File.ReadAllLinesAsync("TestInputs/day3_test.txt");
          var mainInput = await File.ReadAllLinesAsync("Inputs/day3.txt");

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
              var bitCnt = report[0].Length;
              var bits = new int[bitCnt];
              for (int i = 0; i < report.Length; i++)
              {
                  for(int b = 0; b < bitCnt; b++)
                  {
                      bits[b] += report[i][b] == '1' ? 1 : -1;
                  }
              }
              var gamma = string.Join("", bits.Select(x => x > 0 ? "1" : "0"));
              var gammaInt = Convert.ToInt32(gamma, 2);
              var epsilon = string.Join("", bits.Select(x => x < 0 ? "1" : "0"));
              var epsilonInt = Convert.ToInt32(epsilon, 2);
              Console.WriteLine($"\tgamma: {gamma} ({gammaInt}), epsilon: {epsilon} ({epsilonInt})");
              Console.WriteLine($"\tAnswer: {gammaInt * epsilonInt}");
          }

          void SolvePart2ForInputs(string[] report)
          {
              var bitCnt = report[0].Length;
              var oxygenCandidates = new List<string>(report);
              var co2Candidates = new List<string>(report);

              for (int b = 0; b < bitCnt; b++)
              {
                  var mostCommonBit = 0;
                  foreach (var oxygenCandidate in oxygenCandidates)
                  {
                      mostCommonBit += oxygenCandidate[b] == '1' ? 1 : -1;
                  }
                  oxygenCandidates = oxygenCandidates.Where(o => o[b] == (mostCommonBit >= 0 ? '1' : '0')).ToList();
                  if (oxygenCandidates.Count == 1) break;
              }

              for (int b = 0; b < bitCnt; b++)
              {
                  var mostCommonBit = 0;
                  foreach (var co2Candidate in co2Candidates)
                  {
                      mostCommonBit += co2Candidate[b] == '1' ? 1 : -1;
                  }
                  co2Candidates = co2Candidates.Where(c => c[b] == (mostCommonBit < 0 ? '1' : '0')).ToList();
                  if (co2Candidates.Count == 1) break;
              }

              var oxygenReport = oxygenCandidates.Single();
              var oxygenInt = Convert.ToInt32(oxygenReport, 2);
              var co2Report = co2Candidates.Single();
              var co2Int = Convert.ToInt32(co2Report, 2);
              Console.WriteLine($"\toxygen: {oxygenReport} ({oxygenInt}), co2: {co2Report} ({co2Int})");
              Console.WriteLine($"\tAnswer: {oxygenInt * co2Int}");
          }
      };
}