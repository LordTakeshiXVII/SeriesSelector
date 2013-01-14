using System;
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
            CurrentMoviesViewModel = (MoviesViewModel) BootStrapper.Resolve<object>("Movies");
            UnmovedSeries = CurrentSeriesViewModel.FileList.Count;
            UnmovedMovies = CurrentMoviesViewModel.FileList.Count;
            OpenSeries = new AdHocCommand(ExecuteOpenSeries);
            OpenMovies = new AdHocCommand(ExecuteOpenMovies);
            NavigateToHome = new AdHocCommand(ExecuteNavigateToHome);
        }

        public SeriesViewModel CurrentSeriesViewModel { get; set; }
        public MoviesViewModel CurrentMoviesViewModel { get; set; }

        public int UnmovedSeries { get; set; }
        public int UnmovedMovies { get; set; }

        public ICommand OpenSeries { get; set; }
        public ICommand OpenMovies { get; set; }

        public ICommand NavigateToHome { get; set; }

        private void ExecuteOpenSeries(object obj)
        {
            CurrentViewModel = CurrentSeriesViewModel;
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentViewModel"));
        }

        private void ExecuteOpenMovies(object obj)
        {
            CurrentViewModel = CurrentMoviesViewModel;
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