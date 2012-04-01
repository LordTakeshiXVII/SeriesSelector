using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;
using SeriesSelector.Frame;
using SeriesSelector.SeriesManagement;
using SeriesSelector.Windows;

namespace SeriesSelector.ViewModels
{
    [Export("DashBoard", typeof(object))]
    public class DashBoardViewModel : INotifyPropertyChanged
    {
        public DashBoardViewModel()
        {
            CurrentSeriesViewModel = (SeriesViewModel) BootStrapper.Resolve<object>("Series");
            UnmovedFiles = CurrentSeriesViewModel.FileList.Count;
            OpenSeries = new AdHocCommand(ExecuteOpenSeries);
            NavigateToHome = new AdHocCommand(ExecuteNavigateToHome);
        }

        public SeriesViewModel CurrentSeriesViewModel { get; set; }

        public int UnmovedFiles { get; set; }

        public ICommand OpenSeries { get; set; }
        public ICommand NavigateToHome { get; set; }
        
        private void ExecuteOpenSeries(object obj)
        {
            CurrentViewModel = CurrentSeriesViewModel;
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentViewModel"));
        }

        private void ExecuteNavigateToHome(object obj)
        {
            CurrentViewModel = BootStrapper.Resolve<object>("DashBoard");
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentViewModel"));
        }

        public object CurrentViewModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}