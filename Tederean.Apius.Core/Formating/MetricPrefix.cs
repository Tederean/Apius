namespace Tederean.Apius.Formating
{

  public enum MetricPrefix
  {
    Yocto = -8,
    Zepto = -7,
    Atto = -6,
    Femto = -5,
    Pico = -4,
    Nano = -3,
    Mikro = -2,
    Milli = -1,
    None = 0,
    Kilo = 1,
    Mega = 2,
    Giga = 3,
    Teta = 4,
    Peta = 5,
    Exa = 6,
    Zetta = 7,
    Yotta = 8,
  }


  public static class MetricPrefixExtensions
  {

    public static string ToShortString(this MetricPrefix metricPrefix)
    {
      return metricPrefix switch
      {
        MetricPrefix.Yocto => "y",
        MetricPrefix.Zepto => "z",
        MetricPrefix.Atto => "a",
        MetricPrefix.Femto => "f",
        MetricPrefix.Pico => "p",
        MetricPrefix.Nano => "n",
        MetricPrefix.Mikro => "μ",
        MetricPrefix.Milli => "m",
        MetricPrefix.None => string.Empty,
        MetricPrefix.Kilo => "k",
        MetricPrefix.Mega => "M",
        MetricPrefix.Giga => "G",
        MetricPrefix.Teta => "T",
        MetricPrefix.Peta => "P",
        MetricPrefix.Exa => "E",
        MetricPrefix.Zetta => "Z",
        MetricPrefix.Yotta => "Y",
        _ => throw new NotImplementedException(nameof(ToShortString)),
      };
    }
  }
}