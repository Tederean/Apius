using System;
using System.Threading;
using System.Windows;
using Tederean.Apius.Hardware;
using Tederean.Apius.Interop;
using Tederean.Apius.Extensions;
using WpfScreenHelper;
using System.Linq;

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
        using (var cancellationTokenSource = new CancellationTokenSource())
        {
          var viewModel = new MainWindowViewModel(hardwareService);
          var window = new MainWindow(viewModel);

     
          var suitableScreen = Screen.AllScreens.FirstOrDefault(screen => !screen.Primary && screen.WpfBounds.Height < 700 && screen.WpfBounds.Width < 1000);

          if (suitableScreen != null)
          {
            window.Left = suitableScreen.WpfBounds.Left;
            window.Top = suitableScreen.WpfBounds.Top;
            window.Width = suitableScreen.WpfBounds.Width;
            window.Height = suitableScreen.WpfBounds.Height;
          }


          MainWindow = window;
          MainWindow.Show();


          void OnAppSessionEndingInternal(object sender, SessionEndingCancelEventArgs args)
          {
            cancellationTokenSource.Cancel();
          }

          void OnWindowClosedInternal(object? sender, EventArgs args)
          {
            cancellationTokenSource.Cancel();
          }


          SessionEnding += OnAppSessionEndingInternal;
          window.Closed += OnWindowClosedInternal;

          try
          {
            await window.RunAsync(cancellationTokenSource);
          }
          finally
          {
            SessionEnding -= OnAppSessionEndingInternal;
            window.Closed -= OnWindowClosedInternal;
          }
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }
  }
}