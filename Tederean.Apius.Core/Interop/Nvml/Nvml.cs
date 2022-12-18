#if LINUX || WINDOWS
using CommunityToolkit.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace Tederean.Apius.Interop.Nvml
{

  public class Nvml : IDisposable
  {

    private const int NvmlDeviceNameV2BufferSize = 96;


    private bool _disposed;


    public static bool TryCreateInstance([NotNullWhen(true)] out Nvml? nvml)
    {
      try
      {
        if (NvmlInitV2() == NvmlReturn.Success)
        {
          nvml = new Nvml();
          return true;
        }
      }
      catch (DllNotFoundException) { }

      nvml = null;
      return false;
    }


    private Nvml()
    {
      _disposed = false;
    }

    ~Nvml()
    {
      _disposed = true;

      NvmlShutdown();
    }

    public void Dispose()
    {
      if (!_disposed)
      {
        _disposed = true;

        NvmlShutdown();
        GC.SuppressFinalize(this);
      }
    }



    public string DeviceGetName(NvmlDevice nvmlDevice)
    {
      Guard.IsNotNull(nvmlDevice);

      ThrowIfDisposed();

      var name = new byte[NvmlDeviceNameV2BufferSize];

      var response = NvmlDeviceGetName(nvmlDevice.Handle, name, (uint)name.Length);

      ThrowIfNoSuccess(response);

      return FromCString(name);
    }

    public int DeviceGetCount()
    {
      ThrowIfDisposed();

      var response = NvmlDeviceGetCountV2(out uint deviceCount);

      ThrowIfNoSuccess(response);

      return (int)deviceCount;
    }

    public IEnumerable<NvmlDevice> DeviceGetHandles()
    {
      var deviceCount = DeviceGetCount();

      foreach (var index in Enumerable.Range(0, deviceCount))
      {
        var response = NvmlDeviceGetHandleByIndex((uint)index, out IntPtr device);

        ThrowIfNoSuccess(response);

        yield return new NvmlDevice(device);
      }
    }

    public NvmlUtilization DeviceGetUtilizationRates(NvmlDevice nvmlDevice)
    {
      Guard.IsNotNull(nvmlDevice);

      ThrowIfDisposed();

      var response = NvmlDeviceGetUtilizationRates(nvmlDevice.Handle, out NvmlUtilization utilization);

      ThrowIfNoSuccess(response);

      return utilization;
    }

    public NvmlMemory DeviceGetMemoryInfo(NvmlDevice nvmlDevice)
    {
      Guard.IsNotNull(nvmlDevice);

      ThrowIfDisposed();

      var response = NvmlDeviceGetMemoryInfo(nvmlDevice.Handle, out NvmlMemory memory);

      ThrowIfNoSuccess(response);

      return memory;
    }

    public uint DeviceGetPowerUsage_mW(NvmlDevice nvmlDevice)
    {
      Guard.IsNotNull(nvmlDevice);

      ThrowIfDisposed();

      var response = NvmlDeviceGetPowerUsage(nvmlDevice.Handle, out uint power_mW);

      ThrowIfNoSuccess(response);

      return power_mW;
    }

    public uint DeviceGetTemperatureThreshold(NvmlDevice nvmlDevice, NvmlTemperatureThresholds thresholdType)
    {
      Guard.IsNotNull(nvmlDevice);

      ThrowIfDisposed();

      var response = NvmlDeviceGetTemperatureThreshold(nvmlDevice.Handle, thresholdType, out uint temperature_C);

      ThrowIfNoSuccess(response);

      return temperature_C;
    }

    public uint DeviceGetPowerManagementLimit_mW(NvmlDevice nvmlDevice)
    {
      Guard.IsNotNull(nvmlDevice);

      ThrowIfDisposed();

      var response = NvmlDeviceGetPowerManagementLimit_mW(nvmlDevice.Handle, out uint limit_mW);

      ThrowIfNoSuccess(response);

      return limit_mW;
    }

    public uint DeviceGetTemperature(NvmlDevice nvmlDevice, NvmlTemperatureSensor sensorType)
    {
      Guard.IsNotNull(nvmlDevice);

      ThrowIfDisposed();

      var response = NvmlDeviceGetTemperature(nvmlDevice.Handle, sensorType, out uint temperature);

      ThrowIfNoSuccess(response);

      return temperature;
    }



    [DllImport(NativeLibraryResolver.NvmlLibrary, EntryPoint = "nvmlInit_v2")]
    private static extern NvmlReturn NvmlInitV2();


    [DllImport(NativeLibraryResolver.NvmlLibrary, EntryPoint = "nvmlShutdown")]
    private static extern NvmlReturn NvmlShutdown();


    [DllImport(NativeLibraryResolver.NvmlLibrary, CharSet = CharSet.Ansi, EntryPoint = "nvmlDeviceGetCount_v2")]
    private static extern NvmlReturn NvmlDeviceGetCountV2(out uint deviceCount);


    [DllImport(NativeLibraryResolver.NvmlLibrary, EntryPoint = "nvmlDeviceGetHandleByIndex_v2")]
    private static extern NvmlReturn NvmlDeviceGetHandleByIndex(uint index, out IntPtr device);


    [DllImport(NativeLibraryResolver.NvmlLibrary, CharSet = CharSet.Ansi, EntryPoint = "nvmlDeviceGetName")]
    private static extern NvmlReturn NvmlDeviceGetName(IntPtr device, [Out, MarshalAs(UnmanagedType.LPArray)] byte[] name, uint length);


    [DllImport(NativeLibraryResolver.NvmlLibrary, CharSet = CharSet.Ansi, EntryPoint = "nvmlDeviceGetUtilizationRates")]
    private static extern NvmlReturn NvmlDeviceGetUtilizationRates(IntPtr device, out NvmlUtilization utilization);


    [DllImport(NativeLibraryResolver.NvmlLibrary, CharSet = CharSet.Ansi, EntryPoint = "nvmlDeviceGetMemoryInfo")]
    private static extern NvmlReturn NvmlDeviceGetMemoryInfo(IntPtr device, out NvmlMemory memory);


    [DllImport(NativeLibraryResolver.NvmlLibrary, CharSet = CharSet.Ansi, EntryPoint = "nvmlDeviceGetPowerUsage")]
    private static extern NvmlReturn NvmlDeviceGetPowerUsage(IntPtr device, out uint power_mW);


    [DllImport(NativeLibraryResolver.NvmlLibrary, CharSet = CharSet.Ansi, EntryPoint = "nvmlDeviceGetPowerManagementLimit")]
    private static extern NvmlReturn NvmlDeviceGetPowerManagementLimit_mW(IntPtr device, out uint limit_mW);


    [DllImport(NativeLibraryResolver.NvmlLibrary, CharSet = CharSet.Ansi, EntryPoint = "nvmlDeviceGetTemperatureThreshold")]
    private static extern NvmlReturn NvmlDeviceGetTemperatureThreshold(IntPtr device, NvmlTemperatureThresholds thresholdType, out uint temperature_C);


    [DllImport(NativeLibraryResolver.NvmlLibrary, EntryPoint = "nvmlDeviceGetTemperature")]
    private static extern NvmlReturn NvmlDeviceGetTemperature(IntPtr device, NvmlTemperatureSensor sensorType, out uint temperature_C);



    private string FromCString(byte[] buffer)
    {
      return Encoding.Default.GetString(buffer).Replace("\0", "");
    }

    private void ThrowIfNoSuccess(NvmlReturn response)
    {
      if (response != NvmlReturn.Success)
      {
        throw new SystemException(response.ToString());
      }
    }

    private void ThrowIfDisposed()
    {
      if (_disposed)
      {
        throw new ObjectDisposedException(nameof(Nvml));
      }
    }
  }
}
#endif