using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FolderPickerLib;
using SeriesSelector.Data;
using SeriesSelector.Frame;
using SeriesSelector.Properties;

namespace SeriesSelector.ViewModels
{
    [Export("Movies", typeof(object))]
    public class MoviesViewModel : INotifyPropertyChanged
    {
        private string _sourcePath;
        private string _destinationPath;
        private IMovieService _moviesService;
        private ObservableCollection<EpisodeType> _fileList;
        private ObservableCollection<EpisodeType> _newFileList;

        public MoviesViewModel()
        {
            _sourcePath = Settings.Default.SourcePath;
            _destinationPath = Settings.Default.DestinationMoviesPath;

            InitializeCommands();
            InitializeServices();

            UpdateFileLists();
        }

        private void UpdateFileLists()
        {
            FileList = new ObservableCollection<EpisodeType>(_moviesService.GetMovies(_sourcePath));
            NewFileList = new ObservableCollection<EpisodeType>(_moviesService.TryMapMovies(FileList));
        }

        public ObservableCollection<EpisodeType> FileList
        {
            get { return _fileList; }
            set
            {
                _fileList = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("FileList"));
            }
        }

        public ObservableCollection<EpisodeType> NewFileList
        {
            get { return _newFileList; }
            set
            {
                _newFileList = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("NewFileList"));
            }
        }

        private void InitializeServices()
        {
            _moviesService = BootStrapper.Resolve<IMovieService>();
        }

        private void InitializeCommands()
        {
            SelectSourceFolder = new AdHocCommand(ExecuteSelectSourceFolder);
            SelectDestinationFolder = new AdHocCommand(ExecuteSelectDestinationFolder);
            SearchMapping = new AdHocCommand(ExecuteSearchMapping);
            ApplyMapping = new AdHocCommand(ExecuteApplyMapping);
            MoveAllFiles = new AdHocCommand(ExecuteMoveAllFiles);
            PlayThumbnail = new AdHocCommand(ExecutePlayThumbnail);
            SelectAll = new AdHocCommand(ExecuteSelectAll);
            UnselectAll = new AdHocCommand(ExecuteUnselectAll);
        }

        private List<string> _currentMappings;

        public ICommand SearchMapping { get; set; }
        public ICommand ApplyMapping { get; set; }

        public string SourcePath
        {
            get { return _sourcePath; }
            set
            {
                _sourcePath = value;

            }
        }

        public string DestinationPath
        {
            get { return _destinationPath; }
            set { _destinationPath = value; }
        }

        public ICommand SelectSourceFolder { get; private set; }

        public void ExecuteSelectSourceFolder(object obj)
        {
            var dlg = new FolderPickerDialog();
            if (dlg.ShowDialog() != true) return;
            _sourcePath = dlg.SelectedPath;
            PropertyChanged(this, new PropertyChangedEventArgs("SourcePath"));
            UpdateFileLists();
        }

        public ICommand SelectDestinationFolder { get; set; }

        private void ExecuteSelectDestinationFolder(object obj)
        {
            var dlg = new FolderPickerDialog();
            if (dlg.ShowDialog() != true) return;
            _destinationPath = dlg.SelectedPath;
            PropertyChanged(this, new PropertyChangedEventArgs("DestinationPath"));
        }

        public ICommand SelectAll { get; set; }
        private void ExecuteSelectAll(object obj)
        {
            foreach (EpisodeType episode in NewFileList)
            {
                episode.IsSelected = true;
            }
        }

        public ICommand UnselectAll { get; set; }
        private void ExecuteUnselectAll(object obj)
        {
            foreach (EpisodeType episode in NewFileList)
            {
                episode.IsSelected = false;
            }
        }

        private int _progressBarMax = 1;
        public int ProgressBarMax
        {
            get { return _progressBarMax; }
            set
            {
                _progressBarMax = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ProgressBarMax"));
            }
        }

        private int _progressBarValue = 0;
        public int ProgressBarValue
        {
            get { return _progressBarValue; }
            set
            {
                _progressBarValue = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ProgressBarValue"));
            }
        }

        private Visibility _progressBarVisibility = Visibility.Collapsed;
        public Visibility ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set
            {
                _progressBarVisibility = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ProgressBarVisibility"));
            }
        }

        public ICommand MoveAllFiles { get; set; }
        private void ExecuteMoveAllFiles(object obj)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var bg = new BackgroundWorker();

            bg.DoWork += (sender, e) =>
            {
                if (NewFileList == null)
                    return;

                ProgressBarMax = NewFileList.Count;
                ProgressBarValue = 0;
                var counter = 0;
                ProgressBarVisibility = Visibility.Visible;

                foreach (EpisodeType episodeType in NewFileList)
                {

                    counter++;
                    ProgressBarValue = counter;

                    if (!episodeType.IsSelected)
                        continue;

                    episodeType.IsMoving = true;
                    Thread.Sleep(1000);

                    if (episodeType.NewName == null)
                    {
                        episodeType.IsMoving = false;
                        continue;
                    }

                    var oldPath = episodeType.FullPath;
                    var newPath = _destinationPath;


                    if (Directory.Exists(newPath))
                    {
                        newPath = Path.Combine(newPath, episodeType.NewName + episodeType.FileType);
                        if (File.Exists(newPath))
                        {
                            episodeType.IsAlreadyExisting = true;
                            episodeType.IsMoving = false;
                        }

                        else
                            try
                            {
                                File.Move(oldPath, newPath);
                                episodeType.IsMoving = false;
                                episodeType.IsMoved = true;
                            }
                            catch (Exception exception)
                            {
                                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK,
                                                MessageBoxImage.Error);
                            }
                    }
                    else
                    {
                        Directory.CreateDirectory(newPath);
                        newPath = Path.Combine(newPath, episodeType.NewName + episodeType.FileType);
                        try
                        {
                            File.Move(oldPath, newPath);
                            episodeType.IsMoving = false;
                            episodeType.IsMoved = true;
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK,
                                            MessageBoxImage.Error);
                        }

                    }



                    //UpdateFileLists();
                }
            };

            bg.RunWorkerCompleted += (sender, e) =>
            {
                PropertyChanged(this, new PropertyChangedEventArgs("SourcePath"));
                //UpdateFileLists();
                Mouse.OverrideCursor = null;
                MessageBox.Show("All files have been moved.", "Complete",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                ProgressBarVisibility = Visibility.Collapsed;
                UpdateFileLists();
            };

            bg.RunWorkerAsync();
        }

        private void ExecuteApplyMapping(object obj)
        {
            if(SelectedFile == null)
                return;

            SelectedFile.NewName = SelectedMapping;
            NewName = SelectedMapping;
            SelectedFile.IsSelected = true;
            PropertyChanged(this, new PropertyChangedEventArgs("NewFileList"));
        }

        private void ExecuteSearchMapping(object obj)
        {
            if(string.IsNullOrEmpty(MappingSearch))
                return;

            CurrentMappings = _moviesService.SearchFor(MappingSearch);
        }

        public ICommand PlayThumbnail { get; set; }
        private void ExecutePlayThumbnail(object obj)
        {
            var mediaElement = (MediaElement)obj;
            mediaElement.Play();
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        private string _fileSize;
        public string FileSize
        {
            get { return _fileSize; }
            set
            {
                _fileSize = value;
                PropertyChanged(this, new PropertyChangedEventArgs("FileSize"));
            }
        }

        private string _fileType;
        public string FileType
        {
            get { return _fileType; }
            set
            {
                _fileType = value;
                PropertyChanged(this, new PropertyChangedEventArgs("FileType"));
            }
        }

        private string _newName;
        public string NewName
        {
            get { return _newName; }
            set
            {
                _newName = value;
                PropertyChanged(this, new PropertyChangedEventArgs("NewName"));
            }
        }

        private EpisodeType _selectedFile;
        public EpisodeType SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                _selectedFile = value;
                if (_selectedFile != null)
                {
                    Name = SelectedFile.FileName;
                    FileSize = SelectedFile.FileSize;
                    FileType = SelectedFile.FileType;
                    NewName = SelectedFile.NewName;
                    CurrentMappings = _moviesService.SearchFor(Name);
                }
            }
        }

        public List<string> CurrentMappings
        {
            get { return _currentMappings; }
            set
            {
                _currentMappings = value;
                if(PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentMappings"));
            }
        }

        public string SelectedMapping { get; set; }

        public string MappingSearch { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}