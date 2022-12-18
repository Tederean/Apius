#if LINUX
using System.Runtime.InteropServices;

namespace Tederean.Apius.Interop.LmSensors
{

  [StructLayout(LayoutKind.Sequential)]
  public struct SensorsSubfeature
  {

    [MarshalAs(UnmanagedType.LPStr)]
    public string? Name;

    public int Number;

    public SensorSubfeatureType Type;

    public int Mapping;

    public SensorSubfeatureMode Flags;
  }
}
#endif