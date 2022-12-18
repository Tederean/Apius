#if LINUX
namespace Tederean.Apius.Interop.LmSensors
{

  public enum SensorFeatureType
  {
    In = 0x00,
    Fan = 0x01,
    Temperature = 0x02,
    Power = 0x03,
    Energy = 0x04,
    Current = 0x05,
    Humidity = 0x06,
    MaxMain,
    Vid = 0x10,
    Intrusíon = 0x11,
    MaxOther,
    BeepEnable = 0x18,
    Max,
    Unknown = int.MaxValue,
  }
}
#endif