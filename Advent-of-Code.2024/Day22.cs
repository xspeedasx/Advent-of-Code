using Xunit;

namespace Advent_of_Code._2024;

public class Day22
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day22.txt");
    private readonly string[] _testInput = File.ReadAllLines("Inputs/TestInputs/Test22.txt");
    private readonly string[] _testInput2 = File.ReadAllLines("Inputs/TestInputs/Test22_2.txt");

    private static long Evolve(long input)
    {
        // mul 64
        long step1 = MixPrune(input << 6, input);
        // div 32
        long step2 = MixPrune(step1 >> 5, step1);
        // mul 2048
        long step3 = MixPrune(step2 << 11, step2);
        return step3;
    }
    
    private static long MixPrune(long a, long b)
    {
        return (a ^ b) & 0b11111111_11111111_11111111;
    }
    
    [Fact]
    public void Part1Test1()
    {
        var secret = 123;
        long result = Evolve(secret);
        Assert.Equal(15887950, result);
        result = Evolve(result);
        Assert.Equal(16495136, result);
        result = Evolve(result);
        Assert.Equal(527345, result);
        result = Evolve(result);
        Assert.Equal(704524, result);
        result = Evolve(result);
        Assert.Equal(1553684, result);
        result = Evolve(result);
        Assert.Equal(12683156, result);
        result = Evolve(result);
        Assert.Equal(11100544, result);
        result = Evolve(result);
        Assert.Equal(12249484, result);
        result = Evolve(result);
        Assert.Equal(7753432, result);
        result = Evolve(result);
        Assert.Equal(5908254, result);
    }
    
    long EvolveMultiple(long input, int times)
    {
        for (int i = 0; i < times; i++)
        {
            input = Evolve(input);
        }

        return input;
    }
    
    [Fact]
    public void Part1Test2()
    {
        Assert.Equal(8685429, EvolveMultiple(long.Parse(_testInput[0]), 2000));
        Assert.Equal(4700978, EvolveMultiple(long.Parse(_testInput[1]), 2000));
        Assert.Equal(15273692, EvolveMultiple(long.Parse(_testInput[2]), 2000));
        Assert.Equal(8667524, EvolveMultiple(long.Parse(_testInput[3]), 2000));
    }

    long CalculateEvolutionSum(string[] input)
    {
        return input
            .Select(long.Parse)
            .Select(x => EvolveMultiple(x, 2000))
            .Sum();
    }
    
    [Fact]
    public void Part1Test3()
    {
        Assert.Equal(37327623, CalculateEvolutionSum(_testInput));
    }
    
    [Fact]
    public void Part1()
    {
        Assert.Equal(14691757043, CalculateEvolutionSum(_input));
    }
    
    private static ulong EncodeBuffer(sbyte[] buffer)
    {
        ulong result = 0;
        for (int i = 0; i < 4; i++)
        {
            result = (result << 8) | (byte)(buffer[i] + 128);
        }
        return result;
    }

    [Fact]
    public void Part2Test1()
    {
        var initial = 123L;
        Dictionary<ulong, byte> sequencePrices = SequencePrices(initial);
        Assert.Equal(6, sequencePrices[EncodeBuffer([-1, -1, 0, 2])]);
    }

    [Fact]
    public void Part2Test2()
    {
        var dicts = _testInput2
            .Select(long.Parse)
            .Select(SequencePrices)
            .ToArray();

        var enc = EncodeBuffer([-2, 1, -1, 3]);
        Assert.Equal(7, dicts[0].GetValueOrDefault(enc));
        Assert.Equal(7, dicts[1].GetValueOrDefault(enc));
        Assert.Equal(0, dicts[2].GetValueOrDefault(enc));
        Assert.Equal(9, dicts[3].GetValueOrDefault(enc));        
    }

    private Dictionary<ulong, byte> SequencePrices(long initial)
    {
        var sequencePrices = new Dictionary<ulong, byte>();
        
        long current = initial;
        var oldPrice = (byte)((byte)current % 10);
        var changesBuffer = new sbyte[4];
        for (int i = 0; i < 2000; i++)
        {
            var price = (byte)(current % 10);
            var change = (sbyte)(price - oldPrice);
            changesBuffer[i % 4] = change;
            
            if(i > 3)
            {
                sbyte[] shifted = ShiftedBuffer(i, changesBuffer);
                ulong encoded = EncodeBuffer(shifted);
                sequencePrices.TryAdd(encoded, price);
            }
            
            current = Evolve(current);
            oldPrice = price;
        }

        return sequencePrices;

        sbyte[] ShiftedBuffer(int i, sbyte[] buffer)
        {
            var newBuffer = new sbyte[4];
            for (int j = 0; j < 4; j++)
            {
                newBuffer[j] = buffer[(i + 1 + j) % 4];
            }
            return newBuffer;
        }
    }

    private long MostBananas(string[] input)
    {
        var dicts = input
            .Select(long.Parse)
            .Select(SequencePrices)
            .ToArray();
        
        var possibleEncodings = new HashSet<ulong>(dicts[0].Keys);
        for (int i = 1; i < dicts.Length; i++)
        {
            possibleEncodings.UnionWith(dicts[i].Keys);
        }
        
        var maxBananas = 0;
        foreach (var encoding in possibleEncodings)
        {
            var bananas = dicts.Sum(dict => dict.GetValueOrDefault(encoding));
            maxBananas = Math.Max(maxBananas, bananas);
        }
        return maxBananas;
    }
    
    [Fact]
    public void Part2Test3()
    {
        Assert.Equal(23, MostBananas(_testInput2));
    }
    
    [Fact]
    public void Part2()
    {
        Assert.Equal(1831, MostBananas(_input));
    }
}