using System;
using System.Threading.Tasks;
using System.Windows;

namespace Tederean.Apius
{

  public partial class MainWindow : Window
  {

    private readonly MainWindowViewModel _mainWindowViewModel;

    private readonly TaskCompletionSource _taskCompletionSource;


    public MainWindow(MainWindowViewModel viewModel)
    {
      _taskCompletionSource = new TaskCompletionSource();

      DataContext = _mainWindowViewModel = viewModel;

      InitializeComponent();
    }


    public async Task ShowAsync()
    {
      Show();

      await _taskCompletionSource.Task;
    }


    protected override void OnClosed(EventArgs args)
    {
      base.OnClosed(args);

      _mainWindowViewModel.OnClosed();

      _taskCompletionSource.SetResult();
    }
  }
}
