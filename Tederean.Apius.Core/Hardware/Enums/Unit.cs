namespace Tederean.Apius.Hardware
{

  public enum Unit
  {
    Utilization,
    Power,
    Memory,
    Temperature
  }


  public static class UnitExtensions
  {

    public static string ToShortString(this Unit unit)
    {
      return unit switch
      {
        Unit.Utilization => "%",
        Unit.Power => "W",
        Unit.Memory => "B",
        Unit.Temperature => "°C",
        _ => throw new NotImplementedException(nameof(ToShortString)),
      };
    }

    public static bool IsBinary(this Unit unit)
    {
      return unit switch
      {
        Unit.Memory => true,
        _ => false,
      };
    }
  }
}
