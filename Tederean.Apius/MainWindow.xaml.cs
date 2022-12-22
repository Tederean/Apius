using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Tederean.Apius
{

  public partial class MainWindow : Window
  {

    private readonly MainWindowViewModel _mainWindowViewModel;


    public MainWindow(MainWindowViewModel viewModel)
    {
      DataContext = _mainWindowViewModel = viewModel;

      InitializeComponent();
    }


    public async Task RunAsync(CancellationTokenSource cancellationTokenSource)
    {
      await _mainWindowViewModel.RunAsync(cancellationTokenSource);
    }
  }
}
