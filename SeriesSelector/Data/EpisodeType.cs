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
        public string NewName { get; set; }
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}