#if LINUX
using System.Runtime.InteropServices;

namespace Tederean.Apius.Interop.LmSensors
{

  [StructLayout(LayoutKind.Sequential)]
  public struct SensorsChipName
  {

    [MarshalAs(UnmanagedType.LPStr)]
    public string? Name;

    public SensorsBusId Bus;

    public int Addr;

    [MarshalAs(UnmanagedType.LPStr)]
    public string? Path;
  }
}
#endif