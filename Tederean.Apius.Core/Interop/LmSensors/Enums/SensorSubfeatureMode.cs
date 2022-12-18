#if LINUX
namespace Tederean.Apius.Interop.LmSensors
{

  public enum SensorSubfeatureMode : uint
  {
    Read = 1,
    Write = 2,
    ComputeMapping = 4,
  }
}
#endif