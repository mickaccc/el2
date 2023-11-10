using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Prism.Services.Dialogs;

namespace Lieferliste_WPF.Views
{
    /// <summary>
    /// Interaction logic for OrderView.xaml
    /// </summary>
    public partial class Order : UserControl, IDialogWindow
    {
        public Order()
        {
            InitializeComponent();
           
        }

        public Window Owner { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IDialogResult Result { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event EventHandler Closed;
        public event CancelEventHandler Closing;

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Show()
        {
            throw new NotImplementedException();
        }

        public bool? ShowDialog()
        {
            throw new NotImplementedException();
        }
    }
}
