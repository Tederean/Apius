using System;
using System.Threading;
using System.Windows;
using Tederean.Apius.Interop;
using Tederean.Apius.Extensions;
using WpfScreenHelper;
using System.Linq;
using Microsoft.Win32;

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

        using (var cancellationTokenSource = new CancellationTokenSource())
        {
          var viewModel = new MainWindowViewModel();
          var window = new MainWindow(viewModel);

          ApplyWindowDimensions(window);

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

          void OnDisplaySettingsChanged(object? sender, EventArgs args)
          {
            ApplyWindowDimensions(window);
          }


          SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
          SessionEnding += OnAppSessionEndingInternal;
          window.Closed += OnWindowClosedInternal;

          try
          {
            await window.RunAsync(cancellationTokenSource);
          }
          finally
          {
            SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
            SessionEnding -= OnAppSessionEndingInternal;
            window.Closed -= OnWindowClosedInternal;
          }
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
      finally
      {
        Shutdown();
      }
    }

    private void ApplyWindowDimensions(Window window)
    {
      var screen = Screen.AllScreens.FirstOrDefault(screen => !screen.Primary && screen.WpfBounds.Height < 700 && screen.WpfBounds.Width < 1000);

      if (screen != null)
      {
        window.Topmost = true;
        window.Left = screen.WpfBounds.Left;
        window.Top = screen.WpfBounds.Top;
        window.Width = screen.WpfBounds.Width;
        window.Height = screen.WpfBounds.Height;
        window.Show();
      }
      else
      {
        window.Hide();
      }
    }
  }
}