namespace Tederean.Apius.Hardware
{

  public class HardwareService : IHardwareService
  {

#if LINUX || WINDOWS
    private Interop.Nvml.Nvml? _nvml;
#endif

#if LINUX
    private Interop.LmSensors.LmSensors? _lmSensors;
#endif


    public IMainboardService? MainboardService { get; private set; }

    public IGraphicsCardService? GraphicsCardService { get; private set; }


    public HardwareService()
    {
#if LINUX
      Interop.LmSensors.LmSensors.TryCreateInstance(out _lmSensors);

      MainboardService = new LinuxMainboardService(_lmSensors);
#endif


#if WINDOWS
      MainboardService = new WindowsMainboardService();
#endif


#if LINUX || WINDOWS
      if (Interop.Nvml.Nvml.TryCreateInstance(out _nvml))
      {
        var firstGraphicsCard = _nvml.DeviceGetHandles().FirstOrDefault();

        if (firstGraphicsCard != null)
        {
          GraphicsCardService = new ProprietaryNvidiaGraphicsCardService(_nvml, firstGraphicsCard);
        }
      }
#endif
    }

    public void Dispose()
    {
#if LINUX || WINDOWS
      _nvml?.Dispose();
#endif

#if LINUX
      _lmSensors?.Dispose();
#endif

      MainboardService?.Dispose();
      GraphicsCardService?.Dispose();
    }


    public HardwareSensors GetHardwareInfo()
    {
      return new HardwareSensors()
      {
        MainboardValues = MainboardService?.GetMainboardSensors(),
        GraphicsCardValues = GraphicsCardService?.GetGraphicsCardSensors(),
      };
    }
  }
}
