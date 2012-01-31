using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SeriesSelector.Frame;
using SeriesSelector.ViewModels;

namespace SeriesSelector.Dashboard
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        private DashBoardViewModel _currentDashBoardViewModel;

        public DashboardView()
        {
            InitializeComponent();
            SetDataContext();
        }

        private void SetDataContext()
        {
            _currentDashBoardViewModel = (DashBoardViewModel)BootStrapper.Resolve<object>("DashBoard");
            DataContext = _currentDashBoardViewModel;
        }


    }
}
