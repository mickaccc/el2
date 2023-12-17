using Lieferliste_WPF.Dialogs.ViewModels;
using Prism.Mvvm;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lieferliste_WPF.Dialogs
{
    /// <summary>
    /// Interaction logic for AddNewWorkArea.xaml
    /// </summary>
    public partial class AddNewWorkArea : UserControl
    {
        public AddNewWorkArea()
        {
            InitializeComponent();
            //ViewModelLocationProvider.Register(typeof(AddNewWorkArea).ToString(), typeof(AddNewWorkAreaVM));
        }
    }
}
