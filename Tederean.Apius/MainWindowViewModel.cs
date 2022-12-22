using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Tederean.Apius.Formating;
using Tederean.Apius.Hardware;

namespace Tederean.Apius
{
  
  public partial class MainWindowViewModel : ObservableObject
  {

    private readonly IHardwareService _hardwareService;


    [ObservableProperty]
    private string _cpuName;

    [ObservableProperty]
    private string _gpuName;

    [ObservableProperty]
    private Color _gaugeColor;

    [ObservableProperty]
    private Func<double?, string> _loadFormatter;

    [ObservableProperty]
    private Func<double?, string> _wattageFormatter;

    [ObservableProperty]
    private Func<double?, string> _temperatureFormatter;

    [ObservableProperty]
    private Func<double?, string> _memoryFormatter;

    [ObservableProperty]
    private Geometry _loadIcon;

    [ObservableProperty]
    private Geometry _wattageIcon;

    [ObservableProperty]
    private Geometry _temperatureIcon;

    [ObservableProperty]
    private Geometry _memoryIcon;

    [ObservableProperty]
    private HardwareSensors? _sensors;


    public MainWindowViewModel(IHardwareService hardwareService)
    {
      _hardwareService = hardwareService;

      _cpuName = hardwareService.MainboardService?.CpuName ?? "Unknown CPU";
      _gpuName = hardwareService.GraphicsCardService?.GraphicsCardName ?? "Unknown GPU";

      _gaugeColor = Colors.White;

      _loadFormatter = value => value.HasValue ? (value.Value.ToString("0") + " %") : "?";
      _wattageFormatter = value => value.HasValue ? (value.Value.ToString("0") + " W") : "?";
      _temperatureFormatter = value => value.HasValue ? (value.Value.ToString("0") + " °C") : "?";
      _memoryFormatter = value => BinaryFormatter.Format(value, "B");

      _loadIcon = Geometry.Parse("M3.5,18.5L9.5,12.5L13.5,16.5L22,6.92L20.59,5.5L13.5,13.5L9.5,9.5L2,17L3.5,18.5Z");
      _wattageIcon = Geometry.Parse("M11 15H6L13 1V9H18L11 23V15Z");
      _temperatureIcon = Geometry.Parse("M15 13V5A3 3 0 0 0 9 5V13A5 5 0 1 0 15 13M12 4A1 1 0 0 1 13 5V8H11V5A1 1 0 0 1 12 4Z");
      _memoryIcon = Geometry.Parse("M17,17H7V7H17M21,11V9H19V7C19,5.89 18.1,5 17,5H15V3H13V5H11V3H9V5H7C5.89,5 5,5.89 5,7V9H3V11H5V13H3V15H5V17A2,2 0 0,0 7,19H9V21H11V19H13V21H15V19H17A2,2 0 0,0 19,17V15H21V13H19V11M13,13H11V11H13M15,9H9V15H15V9Z");
    }


    public async Task RunAsync(CancellationTokenSource cancellationTokenSource)
    {
      while (!cancellationTokenSource.IsCancellationRequested)
      {
        try
        {
          await Task.Delay(1000, cancellationTokenSource.Token);
        }
        catch (TaskCanceledException)
        {
          continue;
        }

        Sensors = _hardwareService.GetHardwareInfo();
      }
    }
  }
}