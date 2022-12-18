#if LINUX || WINDOWS
using System.Runtime.InteropServices;

namespace Tederean.Apius.Interop.Nvml
{

  [StructLayout(LayoutKind.Sequential)]
  public struct NvmlMemory
  {

    public ulong TotalBytes { get; } // Total installed FB memory(in bytes).

    public ulong FreeBytes { get; } //  Unallocated FB memory(in bytes).

    public ulong UsedBytes { get; } // Allocated FB memory(in bytes). Note that the driver/GPU always sets aside a small amount of memory for bookkeeping.
  }
}
#endif