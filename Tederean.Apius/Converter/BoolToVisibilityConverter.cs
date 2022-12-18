using System.Windows;

namespace Tederean.Apius
{

  public class BoolToVisibilityConverter : BoolToValueConverter<Visibility>
  {

    public BoolToVisibilityConverter() : base(Visibility.Visible, Visibility.Collapsed) { }
  }
}