using System.ComponentModel;

namespace SeriesSelector.Data
{
    public class EpisodeType : INotifyPropertyChanged
    {
        public string FileName { get; set; }
        public string Season { get; set; }
        public string Episode { get; set; }
        public string ReleaseGroup { get; set; }
        public string FileSize { get; set; }
        public string FileType { get; set; }
        public string FullPath { get; set; }
        public string SeriesName { get; set; }
        
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsSelected"));
                }

            }
        }

        private bool _isMoving;
        public bool IsMoving
        {
            get { return _isMoving; }
            set
            {
                _isMoving = value;
                if (PropertyChanged != null)
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsMoving"));
            }
        }

        private bool _isMoved;
        public bool IsMoved
        {
            get { return _isMoved; }
            set
            {
                _isMoved = value;
                if (PropertyChanged != null)
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsMoved"));
            }
        }

        private bool _isAlreadyExisting;
        public bool IsAlreadyExisting
        {
            get { return _isAlreadyExisting; }
            set
            {
                _isAlreadyExisting = value;
                if (PropertyChanged != null)
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsAlreadyExisting"));
            }
        }

        private string _newName;
        public string NewName
        {
            get { return _newName; }
            set
            {
                _newName = value;
                if (PropertyChanged != null)
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("NewName"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}