using System.Collections.Concurrent;
using System.Text.RegularExpressions;

public static partial class AoCSolution
{
    public static Func<Task> Day18 => async () =>
    {
        Console.WriteLine("===== Day 18 =====");
        var testInput = File.ReadAllLines("TestInputs/day18_test.txt");
        var mainInput = File.ReadAllLines("Inputs/day18.txt");

        Console.WriteLine("Part 1 solution for test inputs:");
        SolvePart1ForInputs(testInput);
        //Console.WriteLine("Part 1 solution for puzzle inputs:");
        //SolvePart1ForInputs(mainInput);

        //Console.WriteLine("Part 2 solution for test inputs:");
        //SolvePart2ForInputs(testInput);
        //Console.WriteLine("Part 2 solution for puzzle inputs:");
        //SolvePart2ForInputs(mainInput);
        
        void SolvePart1ForInputs(string[] entries)
        {
            //new SnailNumberNode("[[1,2],3]",0);
            //new SnailNumberNode("[9,[8,7]]", 0);
            //new SnailNumberNode("[[1,9],[8,5]]", 0);
            //new SnailNumberNode("[[[[1,2],[3,4]],[[5,6],[7,8]]],9]", 0);
            //new SnailNumberNode("[[[[1,3],[5,3]],[[1,3],[8,7]]],[[[4,9],[6,9]],[[8,2],[7,3]]]]", 0);

            //var node = Add(new SnailNumberNode("[1,2]", 0), new SnailNumberNode("[[3,4],5]", 0));
            //Console.WriteLine("Added node:");
            //Console.WriteLine(node.ToString());

            //var toReduce = new SnailNumberNode("[[[[[9,8],1],2],3],4]");
            //var toReduce = new SnailNumberNode("[7,[6,[5,[4,[3,2]]]]]");
            //var toReduce = new SnailNumberNode("[[6,[5,[4,[3,2]]]],1]");
            //var toReduce = new SnailNumberNode("[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]");
            
            //var toReduce = new SnailNumberNode("[[[[[4,3],4],4],[7,[[8,4],9]]],[1,1]]");

            //Console.WriteLine($"{Reduce(toReduce)}");
            //return;

            var entry = new SnailNumberNode(entries[0]);
            var idx = 1;
            while(idx < entries.Length)
            {
                var next = new SnailNumberNode(entries[idx++]);
                entry = Add(entry, next);
                Console.WriteLine($"added: {entry}");
                entry = Reduce(entry);
            }

            //var reduced = Reduce(toReduce);

            Console.WriteLine($"\tAnswer is: {entry}");
        }

        void SolvePart2ForInputs(string[] entries)
        {            
            Console.WriteLine($"\tAnswer is: {0}");
        }

        SnailNumberNode Reduce(SnailNumberNode node)
        {
            var actionTaken = true;

            while (actionTaken)
            {
                actionTaken = false;
                SnailNumberNode explosionPair = FindExplosion(node);
                if (explosionPair != null)
                {
                    Console.WriteLine($"Explosion pair: {explosionPair}, depth: {explosionPair.Depth}");
                    var regularLeft = FindRegularNumber(explosionPair, explosionPair.Parent, true);
                    //Console.WriteLine($"regular pair with left: {regularLeft} ({regularLeft?.Parent})");

                    var regularRight = FindRegularNumber(explosionPair, explosionPair.Parent, false);
                    //Console.WriteLine($"regular pair with right: {regularRight} ({regularRight?.Parent})");
                    if (regularLeft != null)
                    {
                        regularLeft.Value += explosionPair.Left.Value;
                    }
                    if (regularRight != null)
                    {
                        regularRight.Value += explosionPair.Right.Value;
                    }
                    explosionPair.Left = null;
                    explosionPair.Right = null;
                    explosionPair.Value = 0;
                    actionTaken = true;
                }
                else
                {
                    var ge10 = FindRegularNumberInChildrenGE10(node);
                    if(ge10 != null)
                    {
                        Console.WriteLine($"Big number: {ge10}, parent: {ge10.Parent}");
                        var left = (int)Math.Floor(ge10.Value!.Value / 2.0);
                        var right = (int)Math.Ceiling(ge10.Value!.Value / 2.0);
                        ge10.Value = null;
                        ge10.Left = new SnailNumberNode { Value = left };
                        ge10.Right = new SnailNumberNode { Value = right };
                        actionTaken = true;
                    }
                }

                Console.WriteLine("reduced:");
                Console.WriteLine($"{node}");
            }

            return node;

            SnailNumberNode FindRegularNumberInChildrenGE10(SnailNumberNode node)
            {
                if (node == null) return null;
                if (node.Value >= 10) return node;
                return FindRegularNumberInChildrenGE10(node.Left) ?? FindRegularNumberInChildrenGE10(node.Right);
            }

            SnailNumberNode FindAnyRegularNumberInChildren(SnailNumberNode node, bool left)
            {
                if (node == null) return null;
                if (node.Value != null) return node;
                if (left)
                    return FindAnyRegularNumberInChildren(node.Right, left) ?? FindAnyRegularNumberInChildren(node.Left, left);
                else
                    return FindAnyRegularNumberInChildren(node.Left, left) ?? FindAnyRegularNumberInChildren(node.Right, left);
            }

            SnailNumberNode FindRegularNumber(SnailNumberNode previous, SnailNumberNode node, bool left)
            {
                if(node == null) return null;
                if (left)
                {
                    if (node.Left.Value != null) 
                        return node.Left;
                    if (node.Left != previous)
                    {
                        var deepFind = FindAnyRegularNumberInChildren(node.Left, left);
                        if (deepFind != null) return deepFind;
                    }
                    if (node.Parent != null && node.Parent.Left != node)
                    {
                        var deepFind = FindAnyRegularNumberInChildren(node.Parent.Left, left);
                        if (deepFind != null) return deepFind;
                    }
                }
                else 
                { 
                    if (node.Right.Value != null) 
                        return node.Right;
                    if (node.Right != previous)
                    {
                        var deepFind = FindAnyRegularNumberInChildren(node.Right, left);
                        if (deepFind != null) return deepFind;
                    }
                    if (node.Parent != null && node.Parent.Right != node)
                    {
                        var deepFind = FindAnyRegularNumberInChildren(node.Parent.Right, left);
                        if (deepFind != null) return deepFind;
                    }
                }
                
                return FindRegularNumber(node, node.Parent, left);
            }

            SnailNumberNode FindExplosion(SnailNumberNode node)
            {
                if (node == null || node.Value != null) return null;
                //is pair?
                if(node.Left?.Value != null && node.Right?.Value != null && node.Depth >= 4)
                {
                    return node;
                }
                else
                {
                    return FindExplosion(node.Left) ?? FindExplosion(node.Right);
                }
            }
        }

        SnailNumberNode Add(SnailNumberNode left, SnailNumberNode right)
        {
            var newNode = new SnailNumberNode { Left = left, Right = right, Depth = -1 };
            left.Parent = newNode;
            right.Parent = newNode;
            IncreaseDepth(newNode);

            void IncreaseDepth(SnailNumberNode node)
            {
                if (node == null) return;
                node.Depth++;
                IncreaseDepth(node.Left);
                IncreaseDepth(node.Right);
            }
            return newNode;
        }
    };

    public class SnailNumberNode
    {
        public int Depth { get; set; }
        public int? Value { get; set; }
        public SnailNumberNode Left { get; set; }
        public SnailNumberNode Right { get; set; }
        public SnailNumberNode Parent { get; set; }

        public SnailNumberNode() { }
        public SnailNumberNode(string input, SnailNumberNode parent = null, int layer = 0)
        {
            Depth = layer;
            Parent = parent;
            if (!input.StartsWith("["))
            {
                Value = int.Parse(input);
                //Console.WriteLine($"{new string(' ', layer * 2)}Value {Value}");
            }
            else
            {
                var endIdx = 0;
                var commaIdx = 0;
                var depth = 0;
                for (int i = 1; i < input.Length - 1; i++)
                {
                    if (input[i] == '[') depth++;
                    else if (input[i] == ']') { if(depth == 0) { endIdx = i; } else { depth--; } }
                    else if (input[i] == ',' && depth == 0) commaIdx = i;
                }

                var leftText = input[1..commaIdx];
                //Console.WriteLine($"{new string(' ', layer * 2)}Left part: {leftText}");
                Left = new SnailNumberNode(leftText, this, layer + 1);

                var rightText = input[(1 + commaIdx)..^1];
                //Console.WriteLine($"{new string(' ', layer * 2)}Right part: {rightText}");
                Right = new SnailNumberNode(rightText, this, layer + 1);
            }
        }

        public override string ToString()
        {
            if(Value != null)
            {
                return Value.ToString();
            }
            else
            {
                return $"[{Left},{Right}]";
            }
        }
    }
}