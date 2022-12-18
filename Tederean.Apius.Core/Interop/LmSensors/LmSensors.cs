#if LINUX
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Tederean.Apius.Interop.LmSensors
{

  public class LmSensors : IDisposable
  {

    private bool _disposed;


    public static bool TryCreateInstance([NotNullWhen(true)] out LmSensors? lmSensors)
    {
      try
      {
        if (Initialize(IntPtr.Zero) == 0)
        {
          lmSensors = new LmSensors();
          return true;
        }
      }
      catch (DllNotFoundException) { }

      lmSensors = null;
      return false;
    }


    private LmSensors()
    {
      _disposed = false;
    }

    ~LmSensors()
    {
      _disposed = true;

      Cleanup();
    }

    public void Dispose()
    {
      if (!_disposed)
      {
        _disposed = true;

        Cleanup();
        GC.SuppressFinalize(this);
      }
    }

    public SensorsChipName[] GetDetectedChipNames()
    {
      ThrowIfDisposed();

      var list = new List<SensorsChipName>();

      SensorsChipName? chipName;
      int counter = 0;

      while ((chipName = GetStruct<SensorsChipName>(GetChipName(IntPtr.Zero, ref counter))).HasValue)
      {
        list.Add(chipName.Value);
      }

      return list.ToArray();
    }

    public SensorsFeature[] GetSensorFeatures(SensorsChipName sensorsChipName)
    {
      ThrowIfDisposed();

      var list = new List<SensorsFeature>();

      var sensorsChipNamePointer = Marshal.AllocHGlobal(Marshal.SizeOf(sensorsChipName));

      try
      {
        Marshal.StructureToPtr(sensorsChipName, sensorsChipNamePointer, false);

        SensorsFeature? feature;
        int counter = 0;

        while ((feature = GetStruct<SensorsFeature>(GetSensorFeature(sensorsChipNamePointer, ref counter))).HasValue)
        {
          list.Add(feature.Value);
        }

        return list.ToArray();
      }
      finally
      {
        Marshal.FreeHGlobal(sensorsChipNamePointer);
      }
    }

    public SensorsSubfeature[] GetSensorSubfeatures(SensorsChipName sensorsChipName, SensorsFeature sensorsFeature)
    {
      ThrowIfDisposed();

      var list = new List<SensorsSubfeature>();

      var sensorsChipNamePointer = Marshal.AllocHGlobal(Marshal.SizeOf(sensorsChipName));
      var sensorsFeaturePointer = Marshal.AllocHGlobal(Marshal.SizeOf(sensorsFeature));

      try
      {
        Marshal.StructureToPtr(sensorsChipName, sensorsChipNamePointer, false);
        Marshal.StructureToPtr(sensorsFeature, sensorsFeaturePointer, false);

        SensorsSubfeature? subfeature;
        int counter = 0;

        while ((subfeature = GetStruct<SensorsSubfeature>(GetSubfeature(sensorsChipNamePointer, sensorsFeaturePointer, ref counter))).HasValue)
        {
          list.Add(subfeature.Value);
        }

        return list.ToArray();
      }
      finally
      {
        Marshal.FreeHGlobal(sensorsChipNamePointer);
        Marshal.FreeHGlobal(sensorsFeaturePointer);
      }
    }

    public double GetValue(SensorsChipName sensorsChipName, SensorsFeature sensorFeature)
    {
      ThrowIfDisposed();

      var sensorsChipNamePointer = Marshal.AllocHGlobal(Marshal.SizeOf(sensorsChipName));

      try
      {
        Marshal.StructureToPtr(sensorsChipName, sensorsChipNamePointer, false);

        var value = 0.0;
        var result = GetValue(sensorsChipNamePointer, sensorFeature.Number, ref value);

        if (result < 0)
        {
          throw new Exception($"sensors_get_value returned code {result}.");
        }

        return value;
      }
      finally
      {
        Marshal.FreeHGlobal(sensorsChipNamePointer);
      }
    }


    private TStruct? GetStruct<TStruct>(IntPtr pointer) where TStruct : struct
    {
      if (pointer != IntPtr.Zero)
      {
        return Marshal.PtrToStructure<TStruct>(pointer);
      }

      return null;
    }


    [DllImport(NativeLibraryResolver.LmSensorsLibrary, EntryPoint = "sensors_init")]
    private static extern int Initialize(IntPtr fileHandle);

    [DllImport(NativeLibraryResolver.LmSensorsLibrary, EntryPoint = "sensors_cleanup")]
    private static extern void Cleanup();

    [DllImport(NativeLibraryResolver.LmSensorsLibrary, EntryPoint = "sensors_get_detected_chips")]
    private static extern IntPtr GetChipName(IntPtr match, ref int nr);

    [DllImport(NativeLibraryResolver.LmSensorsLibrary, EntryPoint = "sensors_get_features")]
    private static extern IntPtr GetSensorFeature(IntPtr name, ref int nr);

    [DllImport(NativeLibraryResolver.LmSensorsLibrary, EntryPoint = "sensors_get_all_subfeatures")]
    private static extern IntPtr GetSubfeature(IntPtr name, IntPtr feature, ref int nr);

    [DllImport(NativeLibraryResolver.LmSensorsLibrary, EntryPoint = "sensors_get_value")]
    private static extern int GetValue(IntPtr name, int subfeat_nr, ref double value);


    // https://github.com/paroj/sensors.py/blob/master/sensors.py
    // code /usr/include/sensors/sensors.h


    private void ThrowIfDisposed()
    {
      if (_disposed)
      {
        throw new ObjectDisposedException(nameof(LmSensors));
      }
    }
  }
}
#endif