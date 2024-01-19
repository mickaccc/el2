using System.Windows.Controls;

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
