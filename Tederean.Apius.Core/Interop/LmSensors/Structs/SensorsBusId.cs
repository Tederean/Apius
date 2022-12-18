#if LINUX
using System.Runtime.InteropServices;

namespace Tederean.Apius.Interop.LmSensors
{

  [StructLayout(LayoutKind.Sequential)]
  public struct SensorsBusId
  {

    public short Type;

    public short Nr;
  }
}
#endif