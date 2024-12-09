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

        private readonly IContainerExtension _container;
        private readonly ILogger _Logger;
        public ICollectionView ProductsView { get; private set; }
        private MeasureFirstPartInfo firstPartInfo;
        private List<ProductMaterial> _Materials =[];
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
                    .ThenInclude(x => x.Vorgangs)
                    .Where(x => string.IsNullOrWhiteSpace(x.Ttnr) == false)
                    .ToListAsync();

                foreach(var m in mat.AsParallel())
                {
                    var p = new ProductMaterial(m.Ttnr, m.Bezeichng, [.. m.OrderRbs]);
                    _Materials.Add(p);
                }
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
            if (obj is ProductMaterial mat && _SearchText != null)
            {
                accept = mat.TTNR.Contains(_SearchText, StringComparison.CurrentCultureIgnoreCase);
                if (!accept)
                    accept = (mat.Description != null) && mat.Description.Contains(_SearchText, StringComparison.CurrentCultureIgnoreCase);
                if (!accept)
                    accept = mat.ProdOrders.Any(x => x.OrderNr.Contains(_SearchText, StringComparison.CurrentCultureIgnoreCase));
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
        public class ProductMaterial
        {
            public string TTNR { get; }
            public string? Description { get; }
 
            public List<ProductOrder> ProdOrders { get; } = [];
            public ProductMaterial(string ttnr, string? description, List<OrderRb> orders)
            {
                TTNR = ttnr;
                Description = description;
                foreach (var order in orders)
                {
                    if (order.Vorgangs.Count > 0)
                    {
                        var d = order.Vorgangs.MaxBy(static x => x.Vnr)?.QuantityYield;
                        var s = order.Vorgangs.Sum(x => x.QuantityScrap);
                        var r = order.Vorgangs.Sum(x => x.QuantityRework);
                        var dic = new Dictionary<string, string>() { ["ttnr"] = ttnr, ["aid"] = order.Aid };
                        ProdOrders.Add(new ProductOrder(dic, order.Aid, order.Quantity, order.Eckstart, order.Eckende, d, s, r, order.Abgeschlossen));  
                    }
                }
            }
            public readonly struct ProductOrder(Dictionary<string, string> Link, string OrderNr, int? Quantity, DateTime? EckStart, DateTime? EckEnd, int? Delivered, int? Scrap, int? Rework, bool closed)
            {
                public string OrderNr { get; } = OrderNr;
                public int Quantity { get; } = Quantity ??= 0;
                public bool Closed { get; } = closed;
                public Dictionary<string, string> Link { get; } = Link;
                public DateTime? Start { get; } = EckStart;
                public DateTime? End { get; } = EckEnd;
                public int Delivered { get; } = Delivered ??= 0;
                public int Scrap { get; } = Scrap ??= 0;
                public int Rework { get; } = Rework ??= 0;
            }
        }
    }
}
