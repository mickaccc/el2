using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static ModuleProducts.Dialogs.ViewModels.ArchivProcessDialogVM;

namespace ModuleProducts.ViewModels
{
    internal class ProductsViewModel : ViewModelBase
    {
        public ProductsViewModel(IContainerExtension container, IApplicationCommands applicationCommands, IDialogService dialogService)
        {
            _container = container;
            _applicationCommands = applicationCommands;
            var loggerFactory = _container.Resolve<ILoggerFactory>();
            _Logger = loggerFactory.CreateLogger<ProductsViewModel>();
            firstPartInfo = new MeasureFirstPartInfo(_container);
            MaterialTask = new NotifyTaskCompletion<ICollectionView>(OnLoadMaterialsAsync());
            _dialogService = dialogService;
        }
        public string Title { get; } = "Produkt Übersicht";

        private readonly IContainerExtension _container;
        private readonly ILogger _Logger;
        private readonly IDialogService _dialogService;
        public ICollectionView ProductsView { get; private set; }
        private MeasureFirstPartInfo firstPartInfo;
        private ObservableCollection<ProductMaterial> _Materials =[];
        private int _ArchivProcessingCount;

        public int ArchivProcessingCount
        {
            get { return _ArchivProcessingCount; }
            set
            {
                _ArchivProcessingCount = value;
                NotifyPropertyChanged(() => ArchivProcessingCount);
            }
        }
        private int _Archivated;

        public int Archivated
        {
            get { return _Archivated; }
            set
            {
                _Archivated = value;
                NotifyPropertyChanged(() => Archivated);
            }
        }
        private int _ArchivState2Count;

        public int ArchivState2Count
        {
            get { return _ArchivState2Count; }
            set
            {
                _ArchivState2Count = value;
                NotifyPropertyChanged(() => ArchivState2Count);
            }
        }
        private int _ArchivState3Count
;

        public int ArchivState3Count

        {
            get { return _ArchivState3Count; }
            set
            {
                _ArchivState3Count = value;
                NotifyPropertyChanged(() => ArchivState3Count);
            }
        }
        private int _ArchivState4Count;

        public int ArchivState4Count
        {
            get { return _ArchivState4Count; }
            set
            {
                _ArchivState4Count = value;
                NotifyPropertyChanged(() => ArchivState4Count);
            }
        }
        private bool _ArchivComplete;

        public bool ArchivComplete
        {
            get { return _ArchivComplete; }
            set
            {
                _ArchivComplete = value;
                NotifyPropertyChanged(() => ArchivComplete);
            }
        }
        private string? _SearchText;
        public string? SearchText
        {
            get { return _SearchText; }
            set
            {
                _SearchText = value;
                OnTextSearch(value);
            }
        }
        private DateTime _StartDateFilter;

        public DateTime StartDateFilter
        {
            get { return _StartDateFilter; }
            set
            {
                _StartDateFilter = value;
                if (value != null && EndDateFilter != null && value >= EndDateFilter)
                    throw new Exception("Startdatum muss kleiner als Enddatum sein");
                
            }
        }
        private IEnumerable<DateTime> _Selected_Dates;

        public IEnumerable<DateTime> Selected_Dates
        {
            get { return _Selected_Dates; }
            set { _Selected_Dates = value; }
        }

        private DateTime? _EndDateFilter;

        public DateTime? EndDateFilter
        {
            get { return _EndDateFilter; }
            set
            {
                _EndDateFilter = value;
                if (value != null && StartDateFilter != null && value < StartDateFilter)
                    throw new Exception("Startdatum muss kleiner als Enddatum sein");
            }
        }

        private RelayCommand? _SearchCommand;
        public RelayCommand SearchCommand => _SearchCommand ??= new RelayCommand(OnTextSearch);
        public ICommand ArchivateCommand => new ActionCommand(OnArchivateExecute, OnCanArchivateExecute);
        public ICommand DateSelectedCommand => new ActionCommand(OnDateSelectedExecute, OnCanDateSelectedExecute);


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

                var onr = await db.Vorgangs
                    .Where(x => x.ArbPlSap != null && x.ArbPlSap.Length >=3)
                    .Select(x => new ValueTuple<string, string> (x.Aid, x.ArbPlSap))
                    .ToListAsync();

                var a = onr
                    .Where(x => UserInfo.User.AccountCostUnits.Any(y => y.CostId.ToString().Equals(x.Item2[..3])))
                    .Select(x => x.Item1)
                    .ToList();

                var mat = await db.TblMaterials
                    .Include(x => x.OrderRbs)
                    .ThenInclude(x => x.Vorgangs)
                    .Where(x => x.OrderRbs.Any(y => a.Contains(y.Aid)))
                    .ToListAsync();

                foreach (var m in mat)
                {
    
                    var p = new ProductMaterial(m.Ttnr, m.Bezeichng, [.. m.OrderRbs]);
                    _Materials.Add(p);
        
                }
                ProductsView = new ListCollectionView(_Materials);
                ProductsView.Filter += OnFilterPredicate;
            }
            catch (Exception e)
            {
                _Logger.LogError("{message}", e.ToString());
            }
            return ProductsView;
        }
        private bool OnCanDateSelectedExecute(object arg)
        {
            return true;
        }

        private void OnDateSelectedExecute(object obj)
        {
            if (obj is DateTime o)
            {
                Selected_Dates = [];
                Selected_Dates = Selected_Dates.Append(o);
            }
            else 
                Selected_Dates = (IEnumerable<DateTime>)obj;
        }
        private bool OnCanArchivateExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.Archivate);
        }

        private void OnArchivateExecute(object obj)
        {
            try
            {
                var par = new DialogParameters();
                par.Add("Archivate", ProductsView);
                par.Add("Container", _container);
    
 
                _dialogService.Show("ArchivProcessDialog", par, ArchivatorCallBack);
            }
            catch (Exception e)
            {
                _Logger.LogError("{message}", e.ToString());
                MessageBox.Show(e.Message, "Error OpenArchivate", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        
        }
        private void ArchivatorCallBack(IDialogResult result)
        {
            if (result.Result == ButtonResult.Yes)
            {

                var res = result.Parameters.GetValue<List<ArchivatorResult>>("Results");

            }
        }
private bool OnFilterPredicate(object obj)
        {
            bool accept = true;
            if (obj is ProductMaterial mat)
            {
                if (string.IsNullOrEmpty(_SearchText) == false)
                {
                    accept = mat.TTNR.Contains(_SearchText, StringComparison.CurrentCultureIgnoreCase);
                    if (!accept)
                        accept = (mat.Description != null) && mat.Description.Contains(_SearchText, StringComparison.CurrentCultureIgnoreCase);
                    if (!accept)
                    {
                        mat.ProdOrders.Filter = item =>
                        {
                            return item is ProductOrder ord && ord.OrderNr.Contains(_SearchText, StringComparison.CurrentCultureIgnoreCase);
                        };
                        accept = !mat.ProdOrders.IsEmpty;
                    }

                }
                if (Selected_Dates != null)
                {
                    if (accept)
                    {
                        mat.ProdOrders.Filter = item =>
                        {
                            return item is ProductOrder ord && ord.Completed != null && Selected_Dates.Contains(ord.Completed.Value.Date);
                        };
                    }
                    accept = !mat.ProdOrders.IsEmpty;            
                }
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

            public ICollectionView ProdOrders { get; private set; }
            public ProductMaterial(string ttnr, string? description, List<OrderRb> orders)
            {
                TTNR = ttnr;
                Description = description;
                List<ProductOrder> products = [];
                foreach (var order in orders)
                {
                    if (order.Vorgangs.Count > 0)
                    {
                        var d = order.Vorgangs.MaxBy(static x => x.Vnr)?.QuantityYield;
                        var s = order.Vorgangs.Sum(x => x.QuantityScrap);
                        var r = order.Vorgangs.Sum(x => x.QuantityRework);
                        var dic = new ValueTuple<string, string, int, string>(ttnr, order.Aid, order.ArchivState, order.ArchivPath);
                        var msf = order.Vorgangs.Where(x => x.Msf != null).Select(x => x.Msf).ToArray();
                        
                        products.Add(new ProductOrder(dic, order.Aid, order.Quantity, order.Eckstart, order.Eckende,
                            d, s, r, order.Abgeschlossen, msf, order.CompleteDate, (Archivator.ArchivState)order.ArchivState));
                    }
                }
                ProdOrders = CollectionViewSource.GetDefaultView(products);
            }
            internal class DateValidationRule : ValidationRule
            {
                public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
                {
                    DateTime? date = value as DateTime?;
                    if (false)
                        return new ValidationResult(false, "Datum ist ungültig");
                    return ValidationResult.ValidResult;
                }
            }
        }
        public struct ProductOrder(ValueTuple<string, string, int, string> OrderLink, string OrderNr, int? Quantity,
            DateTime? EckStart, DateTime? EckEnd, int? Delivered, int? Scrap, int? Rework, bool closed,
            string?[] tags, DateTime? completed, Archivator.ArchivState archivState) : INotifyPropertyChanged
        {
            public string OrderNr { get; } = OrderNr;
            public int Quantity { get; } = Quantity ??= 0;
            public bool Closed { get; } = closed;
            public ValueTuple<string, string, int, string> OrderLink { get; } = OrderLink;
            public DateTime? Start { get; } = EckStart;
            public DateTime? End { get; } = EckEnd;
            public int Delivered { get; } = Delivered ??= 0;
            public int Scrap { get; } = Scrap ??= 0;
            public int Rework { get; } = Rework ??= 0;
            public string?[] Tags { get; } = tags;
            public DateTime? Completed { get; } = completed;
            private string? _archivPath;
            public string? ArchivPath { get { return _archivPath; } set { _archivPath = value; OnPropertyChanged(nameof(ArchivPath)); } }
            private Archivator.ArchivState _archivState = archivState;
            public Archivator.ArchivState ArchivState { get { return _archivState; } set { _archivState = value; OnPropertyChanged(nameof(ArchivState)); } }

            public event PropertyChangedEventHandler? PropertyChanged;
            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
