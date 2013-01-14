using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using FolderPickerLib;
using SeriesSelector.Data;
using SeriesSelector.Frame;
using SeriesSelector.Properties;

namespace SeriesSelector.ViewModels
{
    [Export("Series", typeof(object))]
    public class SeriesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        public SeriesViewModel()
        {
            _sourcePath = Settings.Default.SourcePath;
            _destinationPath = Settings.Default.DestinationSeriesPath;

            _currentMappings = new Dictionary<string, string>();

            InitializeCommands();
            InitializeServices();
            
            UpdateFileLists();
        }

        private void InitializeServices()
        {
            _episodeService = BootStrapper.Resolve<IEpisodeService>();
        }

        private void InitializeCommands()
        {
            SelectSourceFolder = new AdHocCommand(ExecuteSelectSourceFolder);
            SelectDestinationFolder = new AdHocCommand(ExecuteSelectDestinationFolder);
            AddMapping = new AdHocCommand(ExecuteAddMapping);
            RemoveMapping = new AdHocCommand(ExecuteRemoveMapping);
            MoveAllFiles = new AdHocCommand(ExecuteMoveAllFiles);
            PlayThumbnail = new AdHocCommand(ExecutePlayThumbnail);
            SelectAll = new AdHocCommand(ExecuteSelectAll);
            UnselectAll = new AdHocCommand(ExecuteUnselectAll);
        }

        private void UpdateFileLists()
        {
            _fileList = new ObservableCollection<EpisodeType>(_episodeService.GetSourceEpisode(SourcePath));
            _newFileList = new ObservableCollection<EpisodeType>(_episodeService.GetNewFileList(_fileList));

            if(PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("FileList"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("NewFileList"));
            }
            
        }

        private string _selectedFileType;
        public string SelectedFileType
        {
            get { return _selectedFileType; }
            set
            {
                _selectedFileType = value;
                PropertyChanged(this, new PropertyChangedEventArgs("FileList"));
                PropertyChanged(this, new PropertyChangedEventArgs("NewFileList"));
            }
        }

        public ObservableCollection<string> FileTypes { get; set; }

        private void ExecuteRemoveMapping(object obj)
        {
            _currentMappings.Remove(_selectedMapping.Key);
            _episodeService.WriteMappingValue(_currentMappings);
            PropertyChanged(this, new PropertyChangedEventArgs("CurrentMappings"));
            UpdateFileLists();
        }

        public ICommand RemoveMapping { get; set; }

        private KeyValuePair<string, string> _selectedMapping;
        public KeyValuePair<string, string> SelectedMapping
        {
            get { return _selectedMapping; }
            set
            {
                _selectedMapping = value;
                _newName = _selectedMapping.Value;
                PropertyChanged(this, new PropertyChangedEventArgs("NewName"));
            }
        }

        private void ExecuteAddMapping(object obj)
        {
            _selectedFile = null;
            if (!CurrentMappings.ContainsKey(Name))
            {
                CurrentMappings.Add(Name, NewName);
                _episodeService.WriteMappingValue(_currentMappings);
            }
            PropertyChanged(this, new PropertyChangedEventArgs("CurrentMappings"));
            UpdateFileLists();
            
        }

        private string _newName;

        public string NewName
        {
            get { return _newName; }
            set { _newName = value; }
        }

        private IEpisodeService _episodeService;

        private Dictionary<string, string> _currentMappings;

        public ICommand AddMapping { get; set; }

        private string _sourcePath;

        public string SourcePath
        {
            get { return _sourcePath; }
            set
            {
                _sourcePath = value;

            }
        }

        private string _destinationPath;

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

                                     if(episodeType.NewName == null)
                                     {
                                         episodeType.IsMoving = false;
                                         continue;
                                     }

                                     var oldPath = episodeType.FullPath;
                                     var newPath = Path.Combine(_destinationPath, episodeType.SeriesName,
                                                                episodeType.Season.ToUpper());


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

        private ObservableCollection<EpisodeType> _newFileList;
        public ObservableCollection<EpisodeType> NewFileList
        {
            get { return _newFileList; }
            set { _newFileList = value; }
        }

        //private void GetNewFileList()
        //{
        //    var list = new List<EpisodeType>();

        //    _currentMappings = _episodeService.GetMappingValues();

        //    foreach (var episodeType in _fileList)
        //    {
        //        string newName = null;
        //        var oldName = episodeType.FileName;
        //        var matcher = BootStrapper.ResolveAll<ISeriesMatcher>();
        //        foreach (var seriesMatcher in matcher)
        //        {
        //            newName = seriesMatcher.Match(_currentMappings, oldName);
        //            if (!string.IsNullOrEmpty(newName)) break;
        //        }

        //        episodeType.SeriesName = newName;

        //        episodeType.NewName = newName + " " + episodeType.Season.ToUpper() +
        //                              episodeType.Episode.ToUpper();
        //        episodeType.FileType = Path.GetExtension(episodeType.FileName);

        //        episodeType.IsSelected = true;

        //        list.Add(episodeType);
        //    }

        //    _newFileList = new CollectionView(list);
        //}

        public ICommand PlayThumbnail { get; set; }
        private void ExecutePlayThumbnail(object obj)
        {
            var mediaElement = (MediaElement)obj;
            mediaElement.Play();
        }

        private ObservableCollection<EpisodeType> _fileList;
        public ObservableCollection<EpisodeType> FileList
        {
            get
            {
                return _fileList;
            }

        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _season;
        public string Season
        {
            get { return _season; }
            set { _season = value; }
        }

        private string _episode;
        public string Episode
        {
            get { return _episode; }
            set { _episode = value; }
        }

        private string _releaseGroup;
        public string ReleaseGroup
        {
            get { return _releaseGroup; }
            set { _releaseGroup = value; }
        }

        private string _fileSize;
        public string FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; }
        }

        private string _fileType;
        public string FileType
        {
            get { return _fileType; }
            set { _fileType = value; }
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
                    _name = SelectedFile.FileName;
                    _season = SelectedFile.Season;
                    _episode = SelectedFile.Episode;
                    _releaseGroup = SelectedFile.ReleaseGroup;
                    _fileSize = SelectedFile.FileSize;
                    _fileType = SelectedFile.FileType;
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                    PropertyChanged(this, new PropertyChangedEventArgs("Season"));
                    PropertyChanged(this, new PropertyChangedEventArgs("Episode"));
                    PropertyChanged(this, new PropertyChangedEventArgs("ReleaseGroup"));
                    PropertyChanged(this, new PropertyChangedEventArgs("FileSize"));
                    PropertyChanged(this, new PropertyChangedEventArgs("FileType"));
                }
            }
        }

        public Dictionary<string, string> CurrentMappings
        {
            get
            {
                _currentMappings = _episodeService.GetMappingValues();
                return _currentMappings;
            }

        }
    }
}