using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using static ModuleProducts.ViewModels.ProductsViewModel;

namespace ModuleProducts.Dialogs.ViewModels
{
    public class ArchivProcessDialogVM : ViewModelBase, IDialogAware
    {
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
        private int  _ArchivState3Count
;

        public int ArchivState3Count

        {
            get { return  _ArchivState3Count; }
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
        public List<ArchivatorResult> _ArchivatorResults = [];
        private ListCollectionView _ProductMaterials;
        private IContainerExtension _container;
        public DialogCloseListener RequestClose { get; }
        //public event Action<IDialogResult>? RequestClose;
        private DelegateCommand<Vorgang?>? _closeDialogCommand;
        public DelegateCommand<Vorgang?> CloseDialogCommand =>
            _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand<Vorgang?>(CloseDialog));
        public bool CanCloseDialog()
        {
            return false;
        }

        public void OnDialogClosed()
        {
            
        }
        protected virtual void CloseDialog(Vorgang? parameter)
        {
            ButtonResult result = ButtonResult.None;
            IDialogParameters param = new DialogParameters();

            if (parameter == null)
                result = ButtonResult.Cancel;
      
                result = ButtonResult.Yes;
                param.Add("Results", _ArchivatorResults);
      
            RequestClose.Invoke(param, result);
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _ProductMaterials = parameters.GetValue<ListCollectionView> ("Archivate");
            _container = parameters.GetValue<IContainerExtension>("Container");
            
            StartArchivation();
        }
        private void StartArchivation()
        {
            HashSet<ProductToArchiv> archivation = [];
            MeasureFirstPartInfo firstPartInfo = new MeasureFirstPartInfo(_container);
            //using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            foreach (var m in _ProductMaterials)
            {
                
                foreach (var o in (m as ProductMaterial).ProdOrders)
                {
                    ProductOrder s = (ProductOrder)o;
                    if (s.ArchivState != Archivator.ArchivState.None ||
                        !s.Closed ||
                        s.Completed > DateTime.Now.AddDays(-Archivator.DelayDays))
                        continue;
                    
                    archivation.Add(new ProductToArchiv { Material = (m as ProductMaterial).TTNR, Order = s });
                }
            }
            ArchivProcessingCount = archivation.Count;
            foreach (var m in archivation)
            {
                var doku = firstPartInfo.CreateDocumentInfos([m.Material, m.Order.OrderNr]);
                int rulenr = 0;
                bool matched = false;
                foreach (var rule in Archivator.ArchiveRules)
                {
                    string? input = (rule.MatchTarget.Equals(Archivator.ArchivatorTarget.TTNR)) ? m.Material : m.Order.OrderNr;
                    if (Regex.IsMatch(input, rule.RegexString))
                    {
                        matched = true;
                        break;
                    }
                    rulenr++;
                }
                if (!matched)
                {
                    ArchivState4Count++;
                    continue;
                }
                var p = Path.Combine(doku[DocumentPart.RootPath], doku[DocumentPart.SavePath], doku[DocumentPart.Folder]);
                string Location;
                var state = Archivator.Archivate(p, rulenr, out Location);
                if (state == Archivator.ArchivState.Archivated || state == Archivator.ArchivState.NoFiles)
                    Directory.Delete(p, true);
                //var o = db.OrderRbs.Single(x => x.Aid == m.Order.OrderNr);
                //switch (state)
                //{
                //    case Archivator.ArchivState.Archivated:
                //        Archivated++;
                //        o.ArchivPath = Path.Combine(Location, m.Order.OrderNr);
                //        o.ArchivState = (int)state;
                //        break;
                //    case Archivator.ArchivState.NoFiles:
                //        ArchivState2Count++;
                //        o.ArchivState = (int)state;
                //        break;
                //    case Archivator.ArchivState.NoDirectory:
                //        ArchivState3Count++;
                //        o.ArchivState = (int)state;
                //        break;
                //}
                
                //db.SaveChangesAsync();
                
                _ArchivatorResults.Add(new ArchivatorResult() { OrderNr = m.Order.OrderNr,
                    Location = Path.Combine(Location, m.Order.OrderNr), State = state, Material = m.Material});
                ArchivProcessingCount--;
            }
            ArchivComplete = true;
        }
        internal struct ProductToArchiv
        {
            public string Material;
            public ProductOrder Order;
        }
        public struct ArchivatorResult
        {
            public string Material;
            public string OrderNr;
            public Archivator.ArchivState State;
            public string Location;
        }
    }
}
