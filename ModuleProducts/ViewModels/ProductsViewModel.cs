using CompositeCommands.Core;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Windows.Data;

namespace ModuleProducts.ViewModels
{
    internal class ProductsViewModel : ViewModelBase
    {
        public ProductsViewModel(IContainerExtension container, IApplicationCommands applicationCommands)
        {
            _container = container;
            _applicationCommands = applicationCommands;
            var loggerFactory = _container.Resolve<ILoggerFactory>();
            _Logger = loggerFactory.CreateLogger<ProductsViewModel>();
            firstPartInfo = new MeasureFirstPartInfo(_container);
            MaterialTask = new NotifyTaskCompletion<ICollectionView>(OnLoadMaterialsAsync());
        }
        public string Title { get; } = "Produkt Übersicht";
        IContainerExtension _container;
        ILogger _Logger;
        public ICollectionView ProductsView { get; private set; }
        private MeasureFirstPartInfo firstPartInfo;
        private List<TblMaterial> _Materials;
        private string? _SearchText;
        private RelayCommand? _SearchCommand;
        public RelayCommand SearchCommand => _SearchCommand ??= new RelayCommand(OnTextSearch);

        private IApplicationCommands _applicationCommands;
        public IApplicationCommands ApplicationCommands
        {
            get { return _applicationCommands; }
            set
            {
                if (_applicationCommands != null)
                {
                    _applicationCommands = value;
                    NotifyPropertyChanged(() => ApplicationCommands);
                }
            }
        }
        private NotifyTaskCompletion<ICollectionView>? _materialTask;

        public NotifyTaskCompletion<ICollectionView>? MaterialTask
        {
            get { return _materialTask; }
            set
            {
                if (_materialTask != value)
                {
                    _materialTask = value;
                    NotifyPropertyChanged(() => MaterialTask);
                }
            }
        }
        private async Task<ICollectionView> OnLoadMaterialsAsync()
        {
            try
            {
                using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();

                var mat = await db.TblMaterials.AsNoTracking()
                    .Include(x => x.OrderRbs)
                    .Where(x => string.IsNullOrWhiteSpace(x.Ttnr) == false)
                    .ToListAsync();

                _Materials = mat;
                ProductsView = CollectionViewSource.GetDefaultView(_Materials);
                ProductsView.Filter += OnFilterPredicate;
            }
            catch (Exception e)
            {
                _Logger.LogError("{message}", e.ToString());
            }
            return ProductsView;
        }

        private bool OnFilterPredicate(object obj)
        {
            bool accept = true;
            if (obj is TblMaterial mat && _SearchText != null)
            {
                accept = mat.Ttnr.Contains(_SearchText, StringComparison.CurrentCultureIgnoreCase);
                if (!accept)
                    accept = (mat.Bezeichng != null) && mat.Bezeichng.Contains(_SearchText, StringComparison.CurrentCultureIgnoreCase);
                if (!accept)
                    accept = mat.OrderRbs.Any(x => x.Aid.Contains(_SearchText, StringComparison.CurrentCultureIgnoreCase));
            }
            return accept;
        }
        private void OnTextSearch(object obj)
        {
            if(obj is string search)
            {
                _SearchText = search;
                ProductsView.Refresh();
            }
        }
    }
}
