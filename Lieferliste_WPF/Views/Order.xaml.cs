using Prism.Services.Dialogs;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

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
