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
        }

        public SeriesViewModel CurrentSeriesViewModel { get; set; }

        public int UnmovedFiles { get; set; }

        public ICommand OpenSeries { get; set; }

        private void ExecuteOpenSeries(object obj)
        {
            CurrentViewModel = CurrentSeriesViewModel;
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentViewModel"));
        }

        public object CurrentViewModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}