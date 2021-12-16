using System.Collections;
using System.Collections.Concurrent;

public static partial class AoCSolution
{
    public static Func<Task> Day16 => async () =>
    {
        Console.WriteLine("===== Day 16 =====");
        var testInput = File.ReadAllLines("TestInputs/day16_test.txt");
        var mainInput = File.ReadAllLines("Inputs/day16.txt");

        Console.WriteLine("Part 1 solution for test inputs:");
        SolvePart1ForInputs(testInput);
        Console.WriteLine("Part 1 solution for puzzle inputs:");
        SolvePart1ForInputs(mainInput);

        //Console.WriteLine("Part 2 solution for test inputs:");
        //SolvePart2ForInputs(testInput);
        //Console.WriteLine("Part 2 solution for puzzle inputs:");
        //SolvePart2ForInputs(mainInput);

        void SolvePart1ForInputs(string[] entries)
        {
            foreach (var entry in entries)
            {
                //var bits = entry.Select(x => Convert.ToString(x < 'A' ? x - '0' : x - 'A' + 10, 2)).ToArray();
                var bits = Hex2Bits(entry);

                Console.WriteLine($"Packet: {entry}");
                Console.Write("Bits: ");
                for(int i = 0; i < bits.Length; i++)
                {
                    Console.Write(bits[i] ? "1" : "0");
                }
                Console.WriteLine();

                var p = new Packet(bits, 1);

                Console.WriteLine($"\tAnswer is: {CalcVersionNumber(p)}");
            }

            int CalcVersionNumber(Packet packet)
            {
                return packet.Version + packet.SubPackets.Select(x => CalcVersionNumber(x)).Sum();
            }
        }

        void SolvePart2ForInputs(string[] entries)
        {
            Console.WriteLine($"\tAnswer is: {0}");
        }

        
    };

    static bool[] Hex2Bits(string hex)
    {
        var bits = new List<bool>();
        var bytes = hex.Chunk(2).Select(chunk => Convert.ToByte(new String(chunk), 16)).ToArray();
        foreach (var b in bytes)
        {
            //Console.WriteLine($"{b:X2}: {b} : {Convert.ToString(b,2).PadLeft(8,'0')}");
            bits.AddRange(Convert.ToString(b, 2).PadLeft(8, '0').Select(x => x == '1'));
        }
        return bits.ToArray();
    }

    static int BitsToInt(bool[] bits)
    {
        return Convert.ToInt32(String.Join("", bits.Select(x => x ? '1' : '0')).PadLeft(32, '0'), 2);
    }

    class Packet
    {
        public int Length { get; set; }
        public int Version { get; set; }
        public int TypeId { get; set; }
        public int Value { get; set; }
        public List<Packet> SubPackets { get; set; } = new();

        public Packet(bool[] input, int indent)
        {
            Version = BitsToInt(input[0..3]);
            TypeId = BitsToInt(input[3..6]);

            Console.Write(new String(' ', indent * 2)); Console.WriteLine($"Packet version: {Version}");
            Console.Write(new String(' ', indent * 2)); Console.WriteLine($"Packet TypeId: {TypeId}");
            if (TypeId == 4)
            {
                var vals = new Queue<int>();
                var idx = 6;
                var more = true;
                while (more)
                {
                    more = input[idx];
                    var val = BitsToInt(input[(idx+1)..(idx + 5)]);
                    Console.Write(new String(' ', indent * 2)); Console.WriteLine($"LiteralValue: {val}");
                    vals.Enqueue(val);
                    idx += 5;
                }
                Length = 6 + vals.Count * 5;
                while (vals.Count > 0)
                {
                    Value *= 16;
                    Value += vals.Dequeue();
                }
                Console.Write(new String(' ', indent * 2)); Console.WriteLine($"Value: {Value}");
            }
            else
            {
                var lengthId = input[6] ? 1 : 0;
                Console.Write(new String(' ', indent * 2)); Console.WriteLine($"LengthId: {lengthId}");

                if(lengthId == 0)
                {
                    var subLen = BitsToInt(input[7..22]);
                    Length = 7 + 15 + subLen;
                    Console.Write(new String(' ', indent * 2)); Console.WriteLine($"Parsing next {subLen} bits as subpackets");
                    var offset = 22;
                    while(subLen > 0)
                    {
                        var subP = new Packet(input[offset..], indent + 1);
                        subLen -= subP.Length;
                        offset += subP.Length;
                        SubPackets.Add(subP);
                        Console.Write(new String(' ', indent * 2)); Console.WriteLine($"Subpacket of length:{subP.Length}, {subLen} remain");
                    }
                }
                else
                {
                    var subLen = 0;
                    var pCount = BitsToInt(input[7..18]);
                    Console.Write(new String(' ', indent * 2)); Console.WriteLine($"Parsing next {pCount} subpackets");

                    var offset = 18;
                    while(pCount > 0)
                    {
                        var subP = new Packet(input[offset..], indent + 1);
                        pCount--;
                        subLen += subP.Length;
                        offset += subP.Length;
                        SubPackets.Add(subP);
                        Console.Write(new String(' ', indent * 2)); Console.WriteLine($"Subpacket of length: {subP.Length}, {pCount} packets remain");
                    }
                    Length = 7 + 11 + subLen;
                }
            }
            Console.Write(new String(' ', indent * 2)); Console.WriteLine($"Packet length: {Length}");
        }
    }
}