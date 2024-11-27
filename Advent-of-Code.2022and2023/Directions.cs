using System.Drawing;

namespace Advent_of_Code;

public static class Directions
{
    public static Dictionary<Direction, Size> Offsets = new()
    {
        [Direction.UP] = new Size(0, -1),
        [Direction.DOWN] = new Size(0, 1),
        [Direction.LEFT] = new Size(-1, 0),
        [Direction.RIGHT] = new Size(1, 0)
    };

    public enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    };
}