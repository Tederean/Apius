#if LINUX
using System.Runtime.InteropServices;

namespace Tederean.Apius.Interop.LibC
{

  public static class LibC
  {

    [DllImport(NativeLibraryResolver.LibCLibrary, EntryPoint = "getuid")]
    public static extern uint GetUid();


    public static bool IsRoot() => GetUid() == 0;
  }
}
#endif