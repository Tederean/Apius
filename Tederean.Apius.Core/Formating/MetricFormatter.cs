namespace Tederean.Apius.Formating
{

  public class MetricFormatter
  {

    public static string Format(double? value, string baseUnit)
    {
      if (!value.HasValue || double.IsNaN(value.Value))
      {
        return "?";
      }

      if (double.IsPositiveInfinity(value.Value))
      {
        return "∞";
      }

      if (double.IsNegativeInfinity(value.Value))
      {
        return "-∞";
      }

      (var formattedValue, var binaryPrefix) = Format(value.Value);

      return $"{formattedValue} {binaryPrefix.ToShortString()}{baseUnit}";
    }


    private static (string, MetricPrefix) Format(double value)
    {
      var unitCounter = 0;

      while (value >= 999.5)
      {
        value /= 1000.0;
        unitCounter++;
      }

      while (value < 0.995)
      {
        value *= 1000.0;
        unitCounter--;
      }

      var roundedValue = RoundToNDigits(value, 3);

      var digitsCountAfterDecimalSeparator = 3 - GetDigitsCountBeforeDecimalSeparator(roundedValue);
      var formatter = digitsCountAfterDecimalSeparator > 0 ? "0." + new string('0', digitsCountAfterDecimalSeparator) : "0";
      var roundedStringValue = roundedValue.ToString(formatter);

      return (roundedStringValue, (MetricPrefix)unitCounter);
    }

    private static double RoundToNDigits(double value, int requestedDigits)
    {
      var digitsCountBeforeDecimalSeparator = GetDigitsCountBeforeDecimalSeparator(value);

      if (digitsCountBeforeDecimalSeparator < requestedDigits)
      {
        value *= Math.Pow(10.0, requestedDigits - digitsCountBeforeDecimalSeparator);
      }

      value = Math.Round(value);

      if (digitsCountBeforeDecimalSeparator < requestedDigits)
      {
        value *= Math.Pow(0.1, requestedDigits - digitsCountBeforeDecimalSeparator);
      }

      return value;
    }

    private static int GetDigitsCountBeforeDecimalSeparator(double value)
    {
      var absoluteValue = Math.Abs(value);

      if (absoluteValue <= 1.0)
        return 1;

      return (int)Math.Floor(Math.Log10(absoluteValue) + 1.0);
    }
  }
}
