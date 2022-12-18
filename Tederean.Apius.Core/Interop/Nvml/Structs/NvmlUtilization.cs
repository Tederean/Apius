#if LINUX || WINDOWS
namespace Tederean.Apius.Interop.Nvml
{

  public struct NvmlUtilization
  {

    public uint Gpu { get; } // Percent of time over the past sample period during which one or more kernels was executing on the GPU.

    public uint Memory { get; } // Percent of time over the past sample period during which global(device) memory was being read or written.
  }
}
#endif