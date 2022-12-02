namespace Advent_of_Code_2022;

public static class Extensions {
    public static void Deconstruct<T>(this IList<T?> list, out T first, out IList<T?> rest) {
        first = list.Count > 0 ? list[0]! : throw new IndexOutOfRangeException();
        rest = list.Skip(1).ToList();
    }

    public static void Deconstruct<T>(this IList<T?> list, out T first, out T second, out IList<T?> rest) {
        first = list.Count > 0 ? list[0]! : throw new IndexOutOfRangeException(); 
        second = list.Count > 1 ? list[1]! : throw new IndexOutOfRangeException();
        rest = list.Skip(2).ToList();
    }
}