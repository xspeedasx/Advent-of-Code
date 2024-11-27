namespace Advent_of_Code._2022;

public static class Day7_2022
{
    public static void Run(string testInputPath, string challengeInputPath)
    {
        Solve(File.ReadAllLines(testInputPath));
        Solve(File.ReadAllLines(challengeInputPath));
    }

    private static void Solve(string[] lines)
    {
        (List<FSEntity> entities, FSEntity root) = Traverse(lines);

        Console.WriteLine("The FS tree:");
        Print(root);

        Console.WriteLine("--- Sum of directories up to 100K ---");
        ShowSumOfDirsUpTo100K(entities);

        Console.WriteLine("--- Candidate for deletion ---");
        ShowBestDirToDelete(entities, root);
    }

    private static void ShowBestDirToDelete(List<FSEntity> entities, FSEntity root)
    {
        var rootSize = root.Size;
        var unusedSpace = 70_000_000 - rootSize;
        var targetSize = 30_000_000 - unusedSpace;
        Console.WriteLine($"Currently unused space: {unusedSpace}");
        Console.WriteLine($"Need to delete: {targetSize}");

        FSEntity? targetDir = entities
            .Where(x => x.Type == FSEntityType.DIR && x.Size >= targetSize)
            .MinBy(x => x.Size);
        
        Console.WriteLine($"Target directory: {targetDir?.Name}, size: {targetDir?.Size}");
    }

    private static void ShowSumOfDirsUpTo100K(List<FSEntity> entities)
    {
        IEnumerable<FSEntity> dirs = entities.Where(x => x.Type == FSEntityType.DIR && x.Size <= 100_000);
        var sizeSum = 0L;
        foreach (FSEntity dir in dirs)
        {
            var dirSize = dir.Size;
            Console.WriteLine($"Dir {dir.Name}, size: {dirSize}");
            sizeSum += dirSize;
        }

        Console.WriteLine($"Total sizes: {sizeSum}");
    }

    private static (List<FSEntity> entities, FSEntity root) Traverse(string[] lines)
    {
        List<FSEntity> entities = new();
        FSEntity root = new FSEntity
        {
            Type = FSEntityType.DIR,
            Name = "/",
            Parent = null!,
            Children = new List<FSEntity>()
        };

        FSEntity currentDir = root;
        foreach (var line in lines)
        {
            if (line.StartsWith('$'))
            {
                var cmd = line[2..];
                if (cmd.StartsWith("cd"))
                {
                    var target = cmd[3..];
                    switch (target)
                    {
                        case "/":
                            entities.Add(root);
                            break;
                        case "..":
                            currentDir = currentDir.Parent!;              
                            break;
                        default:
                            currentDir = currentDir.Children.First(x => x.Name == target);
                            break;
                    }
                }
            }
            else if (line.StartsWith("dir"))
            {
                var name = line[4..];
                var dir = new FSEntity
                {
                    Type = FSEntityType.DIR,
                    Name = name,
                    Parent = currentDir,
                    Children = new List<FSEntity>()
                };
                entities.Add(dir);
                currentDir.Children.Add(dir);
            }
            else
            {
                var splits = line.Split(" ");
                var size = long.Parse(splits[0]);
                var name = splits[1];
                var file = new FSEntity
                {
                    Type = FSEntityType.FILE,
                    Name = name,
                    Parent = currentDir,
                    Size = size
                };
                entities.Add(file);
                currentDir.Children.Add(file);
            }
        }

        return (entities, root);
    }
    
    static void Print(FSEntity entity, int depth = 0)
    {
        Console.WriteLine($"{new String(' ',2*depth)}- {entity.Name} ({entity.Type}, size={entity.Size})");
        if (entity.Type == FSEntityType.DIR)
        {
            foreach (FSEntity child in entity.Children)
            {
                Print(child, depth + 1);
            }
        }
    }

    private enum FSEntityType { FILE, DIR }
    private record FSEntity
    {
        private long? _size;
        public FSEntityType Type { get; set; }
        public string Name { get; set; } = null!;
        public FSEntity? Parent { get; set; }
        public List<FSEntity> Children { get; set; } = null!;

        public long Size
        {
            get => Type == FSEntityType.FILE 
                ? _size!.Value
                : _size ??= Children.Sum(x => x.Size);
            set => _size = value;
        }
    }
}
