using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace Advent_of_Code._2024;

public class Day09(ITestOutputHelper output)
{
    private readonly string _input = File.ReadAllText("Inputs/Day09.txt");

    private long CalculateChecksum(string diskMap)
    {
        var fileSystem = new List<int>();

        for (var i = 0; i < diskMap.Length; i++)
        {
            var digit = int.Parse(diskMap[i] + "");
            var isFile = i % 2 == 0;
            for(int j = 0; j < digit; j++)
            {
                fileSystem.Add(isFile ? i / 2 : -1);
            }
        }
        
        //output.WriteLine(string.Join("", fileSystem.Select(x => x == -1 ? "." : $"{x}")));
        output.WriteLine("rearranging...");
        // rearrange
        while (!IsOrdered())
        {
            var spaceIndex = fileSystem.IndexOf(-1);
            var lastFileIndex = fileSystem.FindLastIndex(x => x != -1);
            output.WriteLine($"spaceIndex: {spaceIndex}, lastFileIndex: {lastFileIndex}, total: {fileSystem.Count}");
            
            // swap
            (fileSystem[spaceIndex], fileSystem[lastFileIndex]) = (fileSystem[lastFileIndex], fileSystem[spaceIndex]);
            
            //output.WriteLine(string.Join("", fileSystem.Select(x => x == -1 ? "." : $"{x}")));
        }

        long checksum = 0;
        for (var i = 0; i < fileSystem.Count; i++)
        {
            if (fileSystem[i] == -1)
            {
                break;
            }
            checksum += fileSystem[i] * i;
        }
        
        return checksum;
        
        bool IsOrdered()
        {
            bool isSpace = false;
            foreach (var i in fileSystem)
            {
                if (i == -1)
                {
                    isSpace = true;
                }
                else if (isSpace)
                {
                    return false;
                }
            }
            return true;
        }
    }
    
    [Fact]
    public void Part1Test()
    {
        Assert.Equal(60, CalculateChecksum("12345"));
        Assert.Equal(1928, CalculateChecksum("2333133121414131402"));
    }
    
    [Fact]
    public void Part1()
    {
        Assert.Equal(6421128769094, CalculateChecksum(_input));
    }
    
    [Fact]
    public void Part2Test()
    {
        Assert.Equal(2858, CalculateChecksum2("2333133121414131402"));
    }
    
    [Fact]
    public void Part2()
    {
        Assert.Equal(6448168620520, CalculateChecksum2(_input));
    }

    private long CalculateChecksum2(string diskMap)
    {
        var fileSystem = new List<int>();
        var files = new List<(int num, int length, int index)>();

        for (var i = 0; i < diskMap.Length; i++)
        {
            var digit = int.Parse(diskMap[i] + "");
            var isFile = i % 2 == 0;
            for(int j = 0; j < digit; j++)
            {
                fileSystem.Add(isFile ? i / 2 : -1);
            }
            if(isFile)
            {
                files.Add((i / 2, digit, fileSystem.Count - digit));
            }
        }

        // rearrange
        var settledFiles = new HashSet<int>{ 0 };
        bool repeat;
        do
        {
            repeat = false;
            var spaceIndex = fileSystem.IndexOf(-1);

            foreach (var (num, length, index) in files.OrderByDescending(x => x.num))
            {
                if(settledFiles.Contains(num))
                {
                    continue;
                }

                var swapped = false;
                var spaceLength = 1;
                var spaceStart = spaceIndex;
                for(int i = spaceIndex + 1; i < index; i++)
                {
                    if (fileSystem[i] == -1)
                    {
                        spaceLength++;
                        if (spaceLength == length)
                        {
                            // swap
                            for (int j = 0; j < length; j++)
                            {
                                (fileSystem[spaceStart + j], fileSystem[index + j]) = (fileSystem[index + j], fileSystem[spaceStart + j]);
                            }
                            settledFiles.Add(num);
                            files.RemoveAll(x => x.num == num);
                            files.Add((num, length, spaceStart));
                   
                            break;
                        }
                    }
                    else
                    {
                        spaceStart = i+1;
                        spaceLength = 0;
                    }
                }
                if(swapped)
                {
                    repeat = true;
                    break;
                }
            }
        }
        while (repeat);
        
        long checksum = 0;
        for (var i = 0; i < fileSystem.Count; i++)
        {
            if (fileSystem[i] == -1)
            {
                continue;
            }
            checksum += fileSystem[i] * i;
        }
        
        return checksum;
    }
}