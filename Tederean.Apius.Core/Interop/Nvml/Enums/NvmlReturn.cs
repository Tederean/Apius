#if LINUX || WINDOWS
namespace Tederean.Apius.Interop.Nvml
{

  public enum NvmlReturn
  {
    Success = 0,
    Uninitialized,
    InvalidArgument,
    NotSupported,
    NoPermission,
    AlreadInitialized,
    NotFound,
    InsufficientSize,
    InsufficientPower,
    DriverNotLoaded,
    Timeout,
    IrqIssue,
    LibraryNotFound,
    FunctionNotFound,
    CorruptedInforom,
    GpuIsLost,
    ResetRequired,
    OperatingSystem,
    LibRmVersionMismatch,
    InUse,
    Memory,
    NoData,
    VGpuEccNotSupported,
    InsufficientResources,
    Unknown = 999
  }
}
#endif