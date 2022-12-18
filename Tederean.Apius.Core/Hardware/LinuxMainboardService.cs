#if LINUX
using Tederean.Apius.Interop.LibC;
using Tederean.Apius.Interop.LmSensors;

namespace Tederean.Apius.Hardware
{

  public class LinuxMainboardService : IMainboardService
  {

    private readonly string[] CpuTemperatureDivers = new [] { "k10temp", "coretemp" };


    private readonly IRaplService? _raplService;

    private readonly LmSensors? _lmSensors;


    private CpuLoadSample? _lastCpuLoadSample;

    private CpuTemperatureModule[]? _cpuTemperatureModules;


    public string CpuName { get; }


    public LinuxMainboardService(LmSensors? lmSenors)
    {
      if (lmSenors != null)
      {
        var chips = lmSenors.GetDetectedChipNames().Where(chipName => CpuTemperatureDivers.Any(driverName => driverName.Equals(chipName.Name))).ToArray();

        _cpuTemperatureModules = chips.Select(chip =>
        {
          var features = lmSenors.GetSensorFeatures(chip).Where(feature => feature.Type == SensorFeatureType.Temperature).ToArray();

          return new CpuTemperatureModule(chip, features);

        }).ToArray();

        _lmSensors = lmSenors;
      }


      var cpuinfo = File.ReadAllLines("/proc/cpuinfo");

      var vendorName = GetCpuinfoValue(cpuinfo.First(entry => entry.StartsWith("vendor_id")));
      var cpuBrandName = GetCpuinfoValue(cpuinfo.First(entry => entry.StartsWith("model name")));
      var cpuFlags = GetCpuinfoValue(cpuinfo.First(entry => entry.StartsWith("flags"))).Split(' ');
      var cpuVendor = CpuUtils.GetCpuVendor(vendorName);
      var virtualCores = cpuinfo.Where(entry => entry.StartsWith("processor")).Select(line => int.Parse(GetCpuinfoValue(line))).ToArray();

      CpuName = CpuUtils.GetCpuName(cpuBrandName);

      if (cpuVendor == CpuVendor.AMD && cpuFlags.Contains("rapl") && LibC.IsRoot())
      {
        _raplService = new LinuxAmdRaplService(virtualCores);
      }
    }

    public void Dispose()
    {
      _raplService?.Dispose();
    }


    public MainboardSensors GetMainboardSensors()
    {
      var mainboardSensors = new MainboardSensors();

      GetRamValues(mainboardSensors);

      GetLoadValues(mainboardSensors);

      _raplService?.GetWattageValues(mainboardSensors);

      GetTemperatureValues(mainboardSensors);

      return mainboardSensors;
    }


    private void GetTemperatureValues(MainboardSensors mainboardSensors)
    {
      if (_lmSensors != null && _cpuTemperatureModules != null)
      {
        var temperatures = _cpuTemperatureModules.SelectMany(module => module.SensorsFeatures.Select(feature => _lmSensors.GetValue(module.SensorsChipName, feature))).ToArray();

        if (temperatures.Any())
        {
          mainboardSensors.Temperature_C = new Sensor(temperatures.Max(), 0, 90);
        }
      }
    }

    private void GetLoadValues(MainboardSensors mainboardSensors)
    {
      var procStat = File.ReadAllLines("/proc/stat");
      var jiffieValues = procStat.First().Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(value => long.Parse(value)).ToArray();

      var totalJiffies = jiffieValues.Sum();
      var workJiffies = jiffieValues.Take(3).Sum();

      var lastCpuLoadSample = _lastCpuLoadSample;
      var currentCpuLoadSample = _lastCpuLoadSample = new CpuLoadSample(workJiffies, totalJiffies);


      if (lastCpuLoadSample != null)
      {
        var deltaWorkJiffies = (double)currentCpuLoadSample.WorkJiffies - lastCpuLoadSample.WorkJiffies;
        var deltaTotalJiffies = (double)currentCpuLoadSample.TotalJiffies - lastCpuLoadSample.TotalJiffies;

        if (deltaTotalJiffies != 0)
        {
          var loadPercent = (deltaWorkJiffies / deltaTotalJiffies) * 100.0;

          mainboardSensors.Load_percent = new Sensor(loadPercent, 0.0, 100.0);
        }
      }
    }

    private void GetRamValues(MainboardSensors mainboardSensors)
    {
      var memoryInfo = File.ReadAllLines("/proc/meminfo");

      var totalMemory_kB = GetMemInfoValue(memoryInfo.First(entry => entry.StartsWith("MemTotal:")));
      var freeMemory_kB = GetMemInfoValue(memoryInfo.First(entry => entry.StartsWith("MemFree:")));
      var cachedMemory_kB = GetMemInfoValue(memoryInfo.First(entry => entry.StartsWith("Cached:")));

      var usedMemory_kB = totalMemory_kB - freeMemory_kB - cachedMemory_kB;

      mainboardSensors.Memory_B = new Sensor(usedMemory_kB * 1024.0, 0.0, totalMemory_kB * 1024.0);
    }


    private long GetMemInfoValue(string line)
    {
      // Example: "MemTotal:       32849676 kB"

      var valueWithUnit = line.Split(':').Skip(1).First().Trim();
      var valueAsString = valueWithUnit.Split(' ').First();

      return long.Parse(valueAsString);
    }

    private string GetCpuinfoValue(string line)
    {
      // Example: "TLB size	: 2560 4K pages"

      return line.Split(':').Skip(1).First().Trim();
    }


    private record class CpuLoadSample(long WorkJiffies, long TotalJiffies);

    private record class CpuTemperatureModule(SensorsChipName SensorsChipName, SensorsFeature[] SensorsFeatures);
  }
}
#endif