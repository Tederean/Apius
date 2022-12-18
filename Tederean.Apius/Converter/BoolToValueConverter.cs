using System;
using System.Windows.Data;

namespace Tederean.Apius
{
  public abstract class BoolToValueConverter<T> : IValueConverter where T : notnull
  {

    private readonly T _trueValue;

    private readonly T _falseValue;


    protected BoolToValueConverter(T trueValue, T falseValue)
    {
      _trueValue = trueValue;
      _falseValue = falseValue;
    }


    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is bool boolValue)
      {
        return boolValue ? _trueValue : _falseValue;
      }

      throw new NotImplementedException();
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return _trueValue.Equals(value);
    }
  }
}