using System.Drawing;

namespace Advent_of_Code._2024;

public static class DirUtil
{
    public static readonly Size[] Directions =
    [
        new(0, -1), // N
        new(1, 0), // E
        new(0, 1), // S
        new(-1, 0) // W
    ]; 
}