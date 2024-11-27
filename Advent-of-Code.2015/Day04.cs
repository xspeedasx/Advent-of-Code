using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Advent_of_Code._2015;

public class Day04
{
    private readonly string _input = File.ReadAllText(@"Inputs\Day04.txt");
    
    private int FindHashAnyZeros(string key, int numOfZeros)
    {
        var i = 1;
        while (i < 10_000_000)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes($"{key}{i}");
            var hashBytes = md5.ComputeHash(inputBytes);
            var hash = ByteArrayToHexViaLookup32(hashBytes);
            if (hash[..numOfZeros] == new String('0', numOfZeros))
            {
                return i;
            }
            i++;
        }

        return -1;
    }

    [Fact]
    public void Part1Test()
    {
        Assert.Equal(609043, FindHashAnyZeros("abcdef", 5));
        Assert.Equal(1048970, FindHashAnyZeros("pqrstuv", 5));
    }

    [Fact]
    public void Part1()
    {
        Assert.Equal(117946, FindHashAnyZeros(_input, 5));
    }

    [Fact]
    public void Part2()
    {
        Assert.Equal(3938038, FindHashAnyZeros(_input, 6));
    }
    
    
    // nice piece of code, said to be fastest way to make hex string from byte array
    private static readonly uint[] Lookup32 = CreateLookup32();

    private static uint[] CreateLookup32()
    {
        var result = new uint[256];
        for (int i = 0; i < 256; i++)
        {
            string s=i.ToString("X2");
            result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
        }
        return result;
    }

    private static string ByteArrayToHexViaLookup32(byte[] bytes)
    {
        var lookup32 = Lookup32;
        var result = new char[bytes.Length * 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            var val = lookup32[bytes[i]];
            result[2*i] = (char)val;
            result[2*i + 1] = (char) (val >> 16);
        }
        return new string(result);
    }
}