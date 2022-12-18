namespace Tederean.Apius.Extensions
{

  public static class MathExtensions
  {

    public static double Map(this double inputValue, double inputMinimum, double inputMaximum, double outputMinimum, double outputMaximum)
    {
      return (inputValue - inputMinimum) * (outputMaximum - outputMinimum) / (inputMaximum - inputMinimum) + outputMinimum;
    }
  }
}
