using System;
using System.Windows;
using Tederean.Apius.Hardware;
using Tederean.Apius.Interop;

namespace Tederean.Apius
{

  public partial class App : Application
  {

    protected override async void OnStartup(StartupEventArgs args)
    {
      try
      {
        base.OnStartup(args);


        NativeLibraryResolver.Initialize();

        using (var hardwareService = new HardwareService())
        {
          var viewModel = new MainWindowViewModel(hardwareService);
          var window = new MainWindow(viewModel);

          MainWindow = window;

          await window.ShowAsync();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }
  }
}