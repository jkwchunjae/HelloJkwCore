<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

public static class Utils
{
    public static IEnumerable<int[]> Combinations(this int[] numbers, int choose)
    {
        if (choose <= 0 || choose > numbers.Length)
            yield break;

        var buffer = new int[choose];

        foreach (var combo in CombinationsInternal(numbers, choose, 0, 0, buffer))
            yield return combo;
    }

    private static IEnumerable<int[]> CombinationsInternal(
        int[] numbers,
        int choose,
        int start,
        int depth,
        int[] buffer)
    {
        if (depth == choose)
        {
            // 복사해서 반환 (buffer 재사용되기 때문)
            yield return (int[])buffer.Clone();
            yield break;
        }

        for (int i = start; i <= numbers.Length - (choose - depth); i++)
        {
            buffer[depth] = numbers[i];

            foreach (var combo in CombinationsInternal(numbers, choose, i + 1, depth + 1, buffer))
                yield return combo;
        }
    }
}