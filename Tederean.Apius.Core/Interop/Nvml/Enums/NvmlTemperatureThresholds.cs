#if LINUX || WINDOWS
namespace Tederean.Apius.Interop.Nvml
{

	public enum NvmlTemperatureThresholds
	{
		Shutdown = 0,
		Slowdown = 1,
		MemMax = 2,
		GpuMax = 3,
		AcousticMin = 4,
		AcousticCurrent = 5,
		ThresholdAcousticMax = 6,
		Count,
	}
}
#endif