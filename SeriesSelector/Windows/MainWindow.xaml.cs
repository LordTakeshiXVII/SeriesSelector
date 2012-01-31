using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using SeriesSelector.Dashboard;
using SeriesSelector.Frame;
using SeriesSelector.ViewModels;

namespace SeriesSelector.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Export(typeof(Window))]
    public partial class MainWindow : System.Windows.Window
    {
        private readonly DashBoardViewModel _currentViewModel;

        public MainWindow()
        {
            InitializeComponent();
            Open = new AdHocCommand(ExecuteOpen);
            _currentViewModel =(DashBoardViewModel) BootStrapper.Resolve<object>("DashBoard");
            _currentViewModel.CurrentViewModel = _currentViewModel;
            DataContext = _currentViewModel;
        }

        public ICommand Open { get; private set; }

        private void ExecuteOpen(object parameter)
        {
            var viewModelKey = (string)parameter;
            var vm = BootStrapper.Resolve<object>(viewModelKey);
            mainForm.Content = vm;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        
    }
}
