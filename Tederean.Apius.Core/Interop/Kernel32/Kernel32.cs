#if WINDOWS
using System.Runtime.InteropServices;

namespace Tederean.Apius.Interop.Kernel32
{

  public static class Kernel32
  {

    public static GlobalMemoryStatusEx GetGlobalMemoryStatusEx()
    {
      var globalMemoryStatusEx = new GlobalMemoryStatusEx() { Length = (uint)Marshal.SizeOf<GlobalMemoryStatusEx>() };

      if (GlobalMemoryStatusEx(ref globalMemoryStatusEx))
      {
        return globalMemoryStatusEx;
      }

      throw new InvalidOperationException("GlobalMemoryStatusEx call failed.");
    }


    [DllImport(NativeLibraryResolver.Kernel32Library, CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx(ref GlobalMemoryStatusEx globalMemoryStatusEx);
  }
}
#endif