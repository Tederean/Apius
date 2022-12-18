#if LINUX || WINDOWS
namespace Tederean.Apius.Interop.Nvml
{

  public class NvmlDevice
  {

    public IntPtr Handle { get; }


    public NvmlDevice(IntPtr handle)
    {
      if (handle == IntPtr.Zero)
      {
        throw new ArgumentException("Cannot create NvmlDevice from null pointer handle.");
      }

      Handle = handle;
    }
  }
}
#endif