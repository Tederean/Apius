#if WINDOWS
#pragma warning disable CA1416 // Validate platform compatibility
using LibreHardwareMonitor.Hardware;
using System.Diagnostics;
using Tederean.Apius.Extensions;
using Tederean.Apius.Interop.Kernel32;

namespace Tederean.Apius.Hardware
{

  public class WindowsMainboardService : IMainboardService
  {

    private readonly PerformanceCounter _cpuPerformanceCounter;

    private readonly Computer _computer;

    private readonly IHardware _cpu;

    private readonly ISensor[] _cpuTemperatureSensors;

    private readonly ISensor[] _cpuWattageSensors;


    public string CpuName => _cpu.Name;


    public WindowsMainboardService()
    {
      _cpuPerformanceCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

      _computer = new Computer()
      {
        IsBatteryEnabled = false,
        IsControllerEnabled = false,
        IsGpuEnabled = false,
        IsMemoryEnabled = false,
        IsMotherboardEnabled = false,
        IsNetworkEnabled = false,
        IsStorageEnabled = false,
        IsPsuEnabled = false,
        IsCpuEnabled = true,
      };

      _computer.Open();

      _cpu = _computer.Hardware.First(hardware => hardware.HardwareType == HardwareType.Cpu);

      _cpuTemperatureSensors = _cpu.Sensors.Where(sensor => sensor.SensorType == SensorType.Temperature).ToArray();
      _cpuWattageSensors = _cpu.Sensors.Where(sensor => sensor.SensorType == SensorType.Power && sensor.Name.StartsWith("Package")).ToArray();
    }


    public MainboardSensors GetMainboardSensors()
    {
      var mainboardSensors = new MainboardSensors();

      GetRamValues(mainboardSensors);

      GetLoadValues(mainboardSensors);

      GetLibreHardwareMonitorValues(mainboardSensors);

      return mainboardSensors;
    }


    private void GetLibreHardwareMonitorValues(MainboardSensors mainboardSensors)
    {
      _cpu.Update();


      var temperatureSensors = _cpuTemperatureSensors.Select(sensor => sensor.Value).WhereNotNull().ToArray();

      if (temperatureSensors.Any())
      {
        mainboardSensors.Temperature = new Sensor(Unit.Temperature, temperatureSensors.Max(), 0.0, 90.0);
      }


      var wattageSensors = _cpuWattageSensors.Select(sensor => sensor.Value).WhereNotNull().ToArray();

      if (wattageSensors.Any())
      {
        mainboardSensors.Wattage = new Sensor(Unit.Power, wattageSensors.Sum(), 0.0, 105.0);
      }
    }

    private void GetLoadValues(MainboardSensors mainboardSensors)
    {
      var loadPercent = (double)_cpuPerformanceCounter.NextValue();

      mainboardSensors.Load = new Sensor(Unit.Utilization, loadPercent * 100.0, 0.0, 100.0);
    }

    private void GetRamValues(MainboardSensors mainboardSensors)
    {
      var memoryStatus = Kernel32.GetGlobalMemoryStatusEx();

      var totalMemory_B = memoryStatus.TotalPhysicalMemory;
      var freeMemory_B = memoryStatus.AvailablePhysicalMemory;

      var usedMemory_B = totalMemory_B - freeMemory_B;

      mainboardSensors.Memory = new Sensor(Unit.Memory, usedMemory_B, 0.0, totalMemory_B);
    }


    public void Dispose()
    {
      _computer.Close();
    }
  }
}
#pragma warning restore CA1416 // Validate platform compatibility
#endif