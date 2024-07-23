using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Lieferliste_WPF.ViewModels
{
    public class ProjectEditViewModel : ViewModelBase
    {
        public string Title { get; } = "Projekt Editor";

        private IContainerProvider _container;
        private IApplicationCommands _applicationCommands;
        public IApplicationCommands ApplicationCommands
        {
            get { return _applicationCommands; }
            set { _applicationCommands = value; }
        }
        private RelayCommand? _orderSearchCommand;
        private RelayCommand? _projectSearchCommand;
        public ICommand OrderSearchCommand => _orderSearchCommand ??= new RelayCommand(OnOrderSearch);
        public ICommand ProjectSearchCommand => _projectSearchCommand ??= new RelayCommand(OnProjectSearch);

        private ObservableCollection<OrderRb> _ordersList = [];
        public ICollectionView? OrdersCollectionView { get; private set; }
        private PspNode<Shape> Projects = new PspNode<Shape> { Node = new ("Projekte")};
        public ICollectionView? PSP_NodeCollectionView { get; private set; }
        private string _orderSearchText = string.Empty;
        private string _projectSearchText = string.Empty;
        public string ProjectSearchText
        {
            get { return _projectSearchText; }
            set
            {
                if (_projectSearchText != value)
                {
                    _projectSearchText = value;
                    NotifyPropertyChanged(() => ProjectSearchText);
                    NotifyPropertyChanged(() => IsExpanded);
                }
            }
        }

        public bool IsExpanded
        {
            get { return _projectSearchText.Length >= 5; }
        }

        private static readonly object _lock = new();

        public NotifyTaskCompletion<ICollectionView>? OrdTask { get; private set; }
        public NotifyTaskCompletion<ICollectionView>? PspTask { get; private set; }

        public ProjectEditViewModel(IContainerProvider container, IApplicationCommands applicationCommands)
        {
            _container = container;
            _applicationCommands = applicationCommands;

            OrdTask = new NotifyTaskCompletion<ICollectionView>(LoadOrderDataAsync());
            PspTask = new NotifyTaskCompletion<ICollectionView>(LoadPspDataAsync());             
        }


        private void OnProjectSearch(object obj)
        {
            if (obj != null)
            {
                ProjectSearchText = ConvertPsp((string)obj);
                if (_projectSearchText.Length >= 5 || _projectSearchText.Length == 0)
                    PSP_NodeCollectionView?.Refresh();
            }
        }
        private void OnOrderSearch(object obj)
        {
            if (obj != null)
            {
                _orderSearchText = (string)obj;
                OrdersCollectionView?.Refresh();
            }
        }

        private async Task<ICollectionView> LoadOrderDataAsync()
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var orders = await db.OrderRbs.AsNoTracking()
                .Include(x => x.MaterialNavigation)
                .Where(x => x.Abgeschlossen == false).ToListAsync();
            _ordersList.AddRange(orders);
            OrdersCollectionView = CollectionViewSource.GetDefaultView(_ordersList);
            OrdersCollectionView.Filter += FilterPredicateOrder;
            return OrdersCollectionView;
        }

        private async Task<ICollectionView> LoadPspDataAsync()
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var proj = await db.Projects
                .Include(x => x.OrderRbs)
                .ThenInclude(x => x.MaterialNavigation)
                .Where(x => x.OrderRbs.Any(x => x.Abgeschlossen == false))
                .ToListAsync();

            var uiContext = TaskScheduler.FromCurrentSynchronizationContext();

            await Task.Factory.StartNew(() =>
            {

                foreach (var item in proj.OrderBy(x => x.ProjectPsp))
                {
                    var p = item.ProjectPsp.Trim();
                    Regex regex = new Regex("");
                    foreach(var scheme in RuleInfo.ProjectSchemes)
                    {
                        if(p.StartsWith(scheme.Key, StringComparison.OrdinalIgnoreCase))
                            regex = new Regex(scheme.Value.Regex);
                    }
                    Match match = regex.Match(p);
                    if (match.Success && match.Groups.Count > 1)
                    {
                        string psp = string.Empty;
                        PspNode<Shape> stepNode = new PspNode<Shape>();
                        foreach(var m in match.Groups.Values.Skip(1))
                        {
                            if (m.Value == "") break;
                            psp += m.Value;
                            var node = Projects.Children.FirstOrDefault(x => psp.StartsWith(x.Node.ToString()));
                            if (node == null)
                            {                                
                                stepNode = Projects.Add(new Shape(psp), "Psp-Type");
                                stepNode.Node.Description = item.ProjectInfo;
                                stepNode.Node.ProjectType = (ProjectTypes.ProjectType)item.ProjectType;
                                stepNode.Node.PropertyChanged += OnProjectPropertyChanged;
                            }
                            else if(node.Node.ToString() == psp)
                            {
                                stepNode = node;
                            }
                            else
                            {
                                stepNode = node.Add(new Shape(psp), "Psp-Type");
                                stepNode.Node.Description = item.ProjectInfo;
                                stepNode.Node.ProjectType = (ProjectTypes.ProjectType)item.ProjectType;
                                stepNode.Node.PropertyChanged += OnProjectPropertyChanged;
                            }

                        }
 
                        foreach (var o in item.OrderRbs)
                        {
                            var sh = new Shape(o.Aid);
                            sh.Description = string.Format("{0} {1}", o.Material, o.MaterialNavigation?.Bezeichng);
                            stepNode.Add(sh, "Order-Type");
                        }
                    }
                }

            }, CancellationToken.None, TaskCreationOptions.None, uiContext);
            
            PSP_NodeCollectionView = CollectionViewSource.GetDefaultView(Projects.Children);
            PSP_NodeCollectionView.Filter += FilterPredicatePsp;
            return PSP_NodeCollectionView;
        }

        private void OnProjectPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var tn = sender as Shape;
            if (tn != null)
            {
                using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                var pr = db.Projects.Single(x => x.ProjectPsp == tn.ToString());
                if (e.PropertyName == "ProjectType" &&
                    PermissionsProvider.GetInstance().GetUserPermission(Permissions.ProjectTypeChange)) pr.ProjectType = (int)tn.ProjectType;
                if (e.PropertyName == "Description" &&
                    PermissionsProvider.GetInstance().GetUserPermission(Permissions.ProjectDesript)) pr.ProjectInfo = tn.Description;

                db.SaveChanges();
            }

        }

        private bool FilterPredicatePsp(object obj)
        {
            var psp = (PspNode<Shape>)obj;
            var search = ClearPsp(_projectSearchText);

            bool accepted = ClearPsp(psp.Node.ToString()).Contains(search, StringComparison.CurrentCultureIgnoreCase);
            if (!accepted)
            {
                if (psp.Children != null && search != string.Empty)
                {
                    accepted = psp.Children.Any(x => x.Node.ToString().Contains(search, StringComparison.CurrentCultureIgnoreCase));
                }
            }
            return accepted;

        }

        private bool FilterPredicateOrder(object obj)
        {
            if (_orderSearchText != string.Empty)
            {
                var ord = (OrderRb)obj;
                return ord.Aid.Contains(_orderSearchText, StringComparison.CurrentCultureIgnoreCase);
            }
            else
            {
                return true;
            }
        }

        private string ClearPsp(string pspIn)
        {
            Regex reg = new Regex("\\s+|[-]+|[.]+");
            string psp = reg.Replace(pspIn, "");
            return psp;
        }
        private bool IsValidPsp(string psp)
        {

            if ((psp.Length >= 9) && ((psp.Length % 3) == 0))
            {
                return true;
            }
            else return false;
        }
        private string ConvertPsp(string psp)
        {
            psp = ClearPsp(psp.ToUpper().Trim());

            Regex regex = psp.StartsWith("ds", StringComparison.InvariantCultureIgnoreCase) ?
                new Regex("(DS)([0-9]{6})([0-9]{2})*") : new Regex("(SC-PR)([0-9]{9})([0-9]{2})*");
            var match = regex.Match(psp);
            if (match.Success)
            {
                string retVal;
                retVal = match.Groups[1] + "-" + match.Groups[2];
                foreach (var m in match.Groups[3].Captures.Cast<Capture>())
                {
                    retVal += "-" + m.Value;
                }
                return retVal;
            }

            return psp;
        }


    }
}
