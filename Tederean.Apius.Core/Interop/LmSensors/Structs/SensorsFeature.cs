#if LINUX
using System.Runtime.InteropServices;

namespace Tederean.Apius.Interop.LmSensors
{

  [StructLayout(LayoutKind.Sequential)]
  public struct SensorsFeature
  {

    [MarshalAs(UnmanagedType.LPStr)]
    public string? Name;

    public int Number;

    public SensorFeatureType Type;
  }
}
#endif