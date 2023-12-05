using System.Diagnostics;

namespace Advent_of_Code_2022._2023;

public static class Day5_2023
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
    
    private static List<RangeMap> seedToSoil = new();
    private static List<RangeMap> soilToFertilizer = new();
    private static List<RangeMap> fertilizerToWater = new();
    private static List<RangeMap> waterToLight = new();
    private static List<RangeMap> lightToTemperature = new();
    private static List<RangeMap> temperatureToHumidity = new();
    private static List<RangeMap> humidityToLocation = new();
    
    private static void Solve(string[] lines)
    {
        var lineNum = 0;

        long[] seeds = lines[0][7..].Split(" ", StringSplitOptions.TrimEntries).Select(long.Parse).ToArray();

        var mapName = "";
        foreach (string line in lines[2..])
        {
            if (line == "")
            {
                continue;
            }
            if (!char.IsDigit(line[0]))
            {
                mapName = line;
                continue;
            }

            switch (mapName)
            {
                case "seed-to-soil map:":
                    seedToSoil.Add(RangeMap.Parse(line));
                    break;
                case "soil-to-fertilizer map:":
                    soilToFertilizer.Add(RangeMap.Parse(line));
                    break;
                case "fertilizer-to-water map:":
                    fertilizerToWater.Add(RangeMap.Parse(line));
                    break;
                case "water-to-light map:":
                    waterToLight.Add(RangeMap.Parse(line));
                    break;
                case "light-to-temperature map:":
                    lightToTemperature.Add(RangeMap.Parse(line));
                    break;
                case "temperature-to-humidity map:":
                    temperatureToHumidity.Add(RangeMap.Parse(line));
                    break;
                case "humidity-to-location map:":
                    humidityToLocation.Add(RangeMap.Parse(line));
                    break;
            }
        }

        Part1(seeds);
        Part2(seeds);
        //Part2Stupid(seeds);
    }

    static void Part2Stupid(long[] seeds)
    {
        object minLocO = new();
        var minLoc = long.MaxValue;
        for (int seedIdx = 0; seedIdx < seeds.Length; seedIdx += 2)
        {
            var i = seedIdx;
            Parallel.For(seeds[i], seeds[i] + seeds[i + 1] - 1, seed =>
            {
                var seedOffset = seed - seeds[i];
                if (seedOffset > 0 && seedOffset % 1000000 == 0)
                {
                    Console.WriteLine($"[{i}/{seeds.Length}] seed {seed} in {seeds[i]} - {seeds[i] + seeds[i + 1] - 1} minLoc: {minLoc}");
                }

                long location = MapEntry(humidityToLocation,
                    MapEntry(temperatureToHumidity,
                        MapEntry(lightToTemperature,
                            MapEntry(waterToLight,
                                MapEntry(fertilizerToWater,
                                    MapEntry(soilToFertilizer,
                                        MapEntry(seedToSoil, seed)))))));
                if (location < minLoc)
                {
                    lock (minLocO)
                    {
                        minLoc = location;
                    }
                }
            });
        }

        Console.WriteLine($"Stupid answer: {minLoc}");
    }

    private static long MapEntry(List<RangeMap> maps, long entry)
    {
        RangeMap? mapInRange = maps.FirstOrDefault(x => x.InRange(entry));
        return mapInRange?.GetMapped(entry) ?? entry;
    }

    static void Part2(long[] seeds)
    {
        var seedRanges = new List<LongRange>();
        for (int i = 0; i < seeds.Length; i+=2)
        {
            seedRanges.Add(new LongRange(seeds[i], seeds[i] + seeds[i + 1] - 1));
        }
        
        List<LongRange> soilRanges = MapRanges(seedRanges, seedToSoil);
        List<LongRange> fertilizerRanges = MapRanges(soilRanges, soilToFertilizer);
        List<LongRange> waterRanges = MapRanges(fertilizerRanges, fertilizerToWater);
        List<LongRange> lightRanges = MapRanges(waterRanges, waterToLight);
        List<LongRange> tempRanges = MapRanges(lightRanges, lightToTemperature);
        List<LongRange> humiditieRanges = MapRanges(tempRanges, temperatureToHumidity);
        List<LongRange> locationRanges = MapRanges(humiditieRanges, humidityToLocation);

        Console.WriteLine($"Part 2: {locationRanges.Min(x => x.Start)}");
    }

    private static List<LongRange> MapRanges(List<LongRange> srcRanges, List<RangeMap> rangeMaps)
    {
        var dstRanges = new List<LongRange>();
        foreach (LongRange srcRange in srcRanges)
        {
            MapRange(srcRange);

            void MapRange(LongRange range)
            {
                RangeMap? map =
                    rangeMaps.FirstOrDefault(map => range.Start <= map.Src.End && map.Src.Start <= range.End);
                if (map == null)
                {
                    dstRanges.Add(range);
                    return;
                }

                // includes [  (  )  ] - 1 range
                if (range.Start >= map.Src.Start && range.End <= map.Src.End)
                {
                    var mappedRange = new LongRange(map.GetMapped(range.Start), map.GetMapped(range.End));
                    dstRanges.Add(mappedRange);
                }
                // includes (  [  ]  ) - 3 ranges
                else if (range.Start < map.Src.Start && range.End > map.Src.End)
                {
                    var unmapped1 = new LongRange(range.Start, map.Src.Start - 1);
                    MapRange(unmapped1);
                    var mapped = new LongRange(map.GetMapped(map.Src.Start), map.GetMapped(map.Src.End));
                    dstRanges.Add(mapped);
                    var unmapped2 = new LongRange(map.Src.End + 1, range.End);
                    MapRange(unmapped2);
                }
                // splits (  [ )   ] - 2 ranges
                else if (range.Start < map.Src.Start)
                {
                    var unmapped = new LongRange(range.Start, map.Src.Start - 1);
                    MapRange(unmapped);
                    var mapped = new LongRange(map.GetMapped(map.Src.Start), map.GetMapped(range.End));
                    dstRanges.Add(mapped);
                }
                // splits [   ( ]  )- 2 ranges
                else if (range.End > map.Src.End)
                {
                    var unmapped = new LongRange(map.Src.End + 1, range.End);
                    MapRange(unmapped);
                    var mapped = new LongRange(map.GetMapped(range.Start), map.GetMapped(map.Src.End));
                    dstRanges.Add(mapped);
                }
            }
        }

        return dstRanges;
    }

    static void Part1(long[] seeds) 
    {
        long[] soils = seeds.Select(x => MapEntry(seedToSoil, x)).ToArray();
        long[] fertilizers = soils.Select(x => MapEntry(soilToFertilizer, x)).ToArray();
        long[] waters = fertilizers.Select(x => MapEntry(fertilizerToWater, x)).ToArray();
        long[] lights = waters.Select(x => MapEntry(waterToLight, x)).ToArray();
        long[] temps = lights.Select(x => MapEntry(lightToTemperature, x)).ToArray();
        long[] humidities = temps.Select(x => MapEntry(temperatureToHumidity, x)).ToArray();
        long[] locations = humidities.Select(x => MapEntry(humidityToLocation, x)).ToArray();

        Console.WriteLine($"Part 1: {locations.Min()}");

        long MapEntry(List<RangeMap> maps, long entry)
        {
            RangeMap? mapInRange = maps.FirstOrDefault(x => x.InRange(entry));
            return mapInRange?.GetMapped(entry) ?? entry;
        }
    }

    private record LongRange(long Start, long End);

    private class RangeMap
    {
        public LongRange Src { get; set; }
        public LongRange Dst { get; set; }

        public static RangeMap Parse(string line)
        {
            long[] splits = line.Split(" ", StringSplitOptions.TrimEntries).Select(long.Parse).ToArray();
            var src = new LongRange(splits[1], splits[1] + splits[2] - 1);
            var dst = new LongRange(splits[0], splits[0] + splits[2] - 1);
            return new RangeMap { Src = src, Dst = dst };
        }

        public bool InRange(long a)
        {
            return a >= Src.Start && a <= Src.End;
        }

        public long GetMapped(long a)
        {
            return a + (Dst.Start - Src.Start);
        }
    }
}