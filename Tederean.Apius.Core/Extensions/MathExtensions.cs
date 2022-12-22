using System.Numerics;

namespace Tederean.Apius.Extensions
{

  public static class MathExtensions
  {

    public static T Map<T>(this T inputValue, T inputMinimum, T inputMaximum, T outputMinimum, T outputMaximum) where T : INumber<T>
    {
      return (inputValue - inputMinimum) * (outputMaximum - outputMinimum) / (inputMaximum - inputMinimum) + outputMinimum;
    }
  }
}
