using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tederean.Apius.Formating;
using Tederean.Apius.Hardware;
using Tederean.Apius.Localisation;

namespace Tederean.Apius
{
  
  public partial class MainWindowViewModel : ObservableObject
  {

    private readonly CancellationTokenSource _cancellationTokenSource;


    [ObservableProperty]
    private string _cpuName;

    [ObservableProperty]
    private string _gpuName;

    [ObservableProperty]
    private HardwareSensors? _sensors;

    [ObservableProperty]
    private Func<double?, string> _loadFormatter;

    [ObservableProperty]
    private Func<double?, string> _wattageFormatter;

    [ObservableProperty]
    private Func<double?, string> _temperatureFormatter;

    [ObservableProperty]
    private Func<double?, string> _memoryFormatter;


    public MainWindowViewModel(IHardwareService hardwareService)
    {
      _cancellationTokenSource = new CancellationTokenSource();

      _cpuName = hardwareService.MainboardService?.CpuName ?? StringsPanel.FallbackCpuName;
      _gpuName = hardwareService.GraphicsCardService?.GraphicsCardName ?? StringsPanel.FallbackGpuName;

      _loadFormatter = value => value.HasValue ? (value.Value.ToString("0") + " %") : "?";
      _wattageFormatter = value => value.HasValue ? (value.Value.ToString("0") + " W") : "?";
      _temperatureFormatter = value => value.HasValue ? (value.Value.ToString("0") + " °C") : "?";
      _memoryFormatter = value => BinaryFormatter.Format(value, "B");

      _ = LoopAsync(hardwareService);
    }


    private async Task LoopAsync(IHardwareService hardwareService)
    {
      while (!_cancellationTokenSource.IsCancellationRequested)
      {
        try
        {
          await Task.Delay(1000, _cancellationTokenSource.Token);
        }
        catch
        {
          continue;
        }

        Sensors = hardwareService.GetHardwareInfo();
      }
    }


    public void OnClosed()
    {
      _cancellationTokenSource.Cancel();
    }
  }
}