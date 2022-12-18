#if WINDOWS
using System.Runtime.InteropServices;

namespace Tederean.Apius.Interop.Kernel32
{

  [StructLayout(LayoutKind.Sequential)]
  public struct GlobalMemoryStatusEx
  {

    public uint Length;

    public uint MemoryLoad;

    public ulong TotalPhysicalMemory;

    public ulong AvailablePhysicalMemory;

    public ulong TotalPageFile;

    public ulong AvaileablePageFile;

    public ulong TotalVirtualMemory;

    public ulong AvailableVirtualMemory;

    public ulong AvailableExtendedVirtualMemory;
  }
}
#endif