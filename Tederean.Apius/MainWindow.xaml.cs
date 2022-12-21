using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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

    protected override void OnMouseDown(MouseButtonEventArgs args)
    {
      base.OnMouseDown(args);

      if (args.ChangedButton == MouseButton.Left)
      {
        DragMove();
      }
    }
  }
}
