using System.Diagnostics;

namespace Advent_of_Code_2022._2023;

public static class Day20_2023
{
    public enum State
    {
        LOW,
        HIGH
    }

    public static void Run(string testInputPath, string challengeInputPath)
    {
        var sw = new Stopwatch();
        sw.Start();

        var testInputPath2 = testInputPath.Replace(".txt", "_2.txt");

        //Solve(File.ReadAllLines(testInputPath));
        //Solve(File.ReadAllLines(testInputPath2));
        Solve(File.ReadAllLines(challengeInputPath));

        sw.Stop();
        Console.WriteLine($"Solution took: {sw.ElapsedMilliseconds} ms");
    }

    private static void Solve(string[] lines)
    {
        var modules = new Dictionary<string, IModule>();
        List<Conjunction> conjunctions = new();

        foreach (string line in lines)
        {
            string[] splits = line.Split("->", StringSplitOptions.TrimEntries);
            string name = splits[0];
            string[] targets = splits[1].Split(",", StringSplitOptions.TrimEntries);
            
            if (name == "broadcaster")
            {
                modules[name] = new Broadcaster(name, targets);
            }
            else if (name[0] == '%')
            {
                name = name[1..];
                modules[name] = new FlipFlop(name, targets);
            }
            else if (name[0] == '&')
            {
                name = name[1..];
                var conjunction = new Conjunction(name, targets);
                modules[name] = conjunction;
                conjunctions.Add(conjunction);
            }
            else
            {
                modules[name] = new Untyped(name);
            }
        }

        var untypeds = new List<string>();
        foreach ((string _, IModule module) in modules)
        {
            foreach (Conjunction c in conjunctions.Where(x => module.Targets.Contains(x.Name)))
            {
                c.InputStates[module.Name] = State.LOW;
            }

            foreach (string target in module.Targets)
            {
                if (!modules.ContainsKey(target))
                {
                    untypeds.Add(target);
                }
            }
        }

        foreach (string untypedName in untypeds)
        {
            modules[untypedName] = new Untyped(untypedName);
        }

        var lowCnt = 0;
        var hiCnt = 0;
        var pressCnt = 0;
        var gotLowOnRx = false;

        // Part 1
        // for (int i = 0; i < 1000; i++)
        // {
        //     SimulatePress();
        // }
        // Console.WriteLine($"{lowCnt} low pulses, {hiCnt} hi pulses");
        // Console.WriteLine($"answer: {(long)lowCnt * hiCnt}");
        
        // Part 2
        while (!gotLowOnRx)
        {
            pressCnt++;
            SimulatePress();
        }
        Console.WriteLine($"presses: {pressCnt}");
        // end Part 2

        void SimulatePress()
        {
            var queue = new Queue<Pulse>();
            queue.Enqueue(new Pulse("button", "broadcaster", State.LOW));
            while (queue.Count > 0)
            {
                Pulse pulse = queue.Dequeue();
                if (pulse.Dest == "rx" && pulse.State == State.LOW)
                {
                    gotLowOnRx = true;
                }
                if (pulse.State == State.LOW)
                {
                    lowCnt++;
                }
                else
                {
                    hiCnt++;
                }

                IModule module = modules[pulse.Dest];
                Pulse[] response = module.Process(pulse);
                foreach (Pulse rPulse in response)
                {
                    queue.Enqueue(rPulse);
                }
            }
        }
    }

    private class Untyped : IModule
    {
        public Untyped(string name)
        {
            Name = name;
        }
        
        public string Name { get; set; }
        public string[] Targets { get; set; } = null!;
        public Pulse[] Process(Pulse input)
        {
            //Console.WriteLine($"{Name} received {input.State} pulse from {input.Src}");
            return Array.Empty<Pulse>();
        }
    }
    
    private class Conjunction : IModule
    {
        public Conjunction(string name, string[] targets)
        {
            Name = name;
            Targets = targets;
        }

        public Dictionary<string, State> InputStates { get; } = new();

        public string Name { get; set; }
        public string[] Targets { get; set; }

        public Pulse[] Process(Pulse input)
        {
            InputStates[input.Src] = input.State;
            State outState = InputStates.Any(x => x.Value == State.LOW) ? State.HIGH : State.LOW;

            return Targets.Select(x => new Pulse(Name, x, outState)).ToArray();
        }
    }

    private class FlipFlop : IModule
    {
        private bool _isOn;

        public FlipFlop(string name, string[] targets)
        {
            Name = name;
            Targets = targets;
        }

        public string Name { get; set; }
        public string[] Targets { get; set; }

        public Pulse[] Process(Pulse input)
        {
            if (input.State == State.HIGH)
            {
                return Array.Empty<Pulse>();
            }

            _isOn = !_isOn;
            return Targets.Select(x => new Pulse(Name, x, _isOn ? State.HIGH : State.LOW)).ToArray();
        }
    }

    private class Broadcaster : IModule
    {
        public Broadcaster(string name, string[] targets)
        {
            Name = name;
            Targets = targets;
        }

        public string Name { get; set; }
        public string[] Targets { get; set; }

        public Pulse[] Process(Pulse input)
        {
            return Targets.Select(x => new Pulse(Name, x, input.State)).ToArray();
        }
    }

    private record Pulse(string Src, string Dest, State State);

    private interface IModule
    {
        string Name { get; set; }
        string[] Targets { get; set; }
        Pulse[] Process(Pulse input);
    }
}