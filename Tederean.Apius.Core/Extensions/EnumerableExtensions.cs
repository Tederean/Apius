namespace Tederean.Apius.Extensions
{

  public static class EnumerableExtensions
  {

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable) where T : class
    {
      foreach (var entry in enumerable)
      {
        if (entry is not null)
        {
          yield return entry;
        }
      }
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable) where T : struct
    {
      foreach (var entry in enumerable)
      {
        if (entry.HasValue)
        {
          yield return entry.Value;
        }
      }
    }
  }
}
