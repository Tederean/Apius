namespace Tederean.Apius.Formating
{

  public enum BinaryPrefix
  {
    None = 0,
    Kibi = 1,
    Mebi = 2,
    Gibi = 3,
    Tebi = 4,
    Pebi = 5,
    Exbi = 6,
    Zebi = 7,
    Yobi = 8,
  }


  public static class BinaryPrefixExtensions
  {

    public static string ToShortString(this BinaryPrefix binaryPrefix)
    {
      return binaryPrefix switch
      {
        BinaryPrefix.None => string.Empty,
        BinaryPrefix.Kibi => "Ki",
        BinaryPrefix.Mebi => "Mi",
        BinaryPrefix.Gibi => "Gi",
        BinaryPrefix.Tebi => "Ti",
        BinaryPrefix.Pebi => "Pi",
        BinaryPrefix.Exbi => "Ei",
        BinaryPrefix.Zebi => "Zi",
        BinaryPrefix.Yobi => "Yi",
        _ => throw new NotImplementedException(nameof(ToShortString)),
      };
    }
  }
}
