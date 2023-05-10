using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFClient.ViewModels;

namespace WPFClient.Views
{
    /// <summary>
    /// Interaction logic for EmulatorSetupWindow.xaml
    /// </summary>
    public partial class EmulatorSetupWindow 
    {
        public EmulatorSetupWindow(EmulatorSetupViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
