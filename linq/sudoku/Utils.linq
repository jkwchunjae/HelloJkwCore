<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

public static class Utils
{
    /// <summary> elements중 choose개를 선택할 수 있는 경우의 수 </summary>
    public static IEnumerable<T[]> Combinations<T>(this T[] elements, int choose)
    {
        if (choose <= 0 || choose > elements.Length)
            yield break;

        var buffer = new T[choose];

        foreach (var combo in CombinationsInternal(elements, choose, 0, 0, buffer))
            yield return combo;
    }

    private static IEnumerable<T[]> CombinationsInternal<T>(
        T[] elements,
        int choose,
        int start,
        int depth,
        T[] buffer)
    {
        if (depth == choose)
        {
            // 복사해서 반환 (buffer 재사용되기 때문)
            yield return (T[])buffer.Clone();
            yield break;
        }

        for (int i = start; i <= elements.Length - (choose - depth); i++)
        {
            buffer[depth] = elements[i];

            foreach (var combo in CombinationsInternal(elements, choose, i + 1, depth + 1, buffer))
                yield return combo;
        }
    }
}