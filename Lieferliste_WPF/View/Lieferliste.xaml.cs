using Lieferliste_WPF.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Lieferliste_WPF.View
{
    /// <summary>
    /// Interaktionslogik für Lieferliste.xaml
    /// </summary>
    public partial class Lieferliste : Page
    {
      private LieferViewModel _viewModel = new();

        public Lieferliste()
        {         
            InitializeComponent();
            Loaded += Lieferliste_Loaded;
            DataContext = _viewModel;
        }

        private async void Lieferliste_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadDataAsync();
        }
    }
}
