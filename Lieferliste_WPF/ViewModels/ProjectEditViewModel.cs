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
        public ICommand ConcatCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        private ObservableCollection<OrderRb> _ordersList = [];
        public ICollectionView? OrdersCollectionView { get; private set; }
        private Tree<string>? tree;
        private PspTree? PspTree;
        public ICollectionView? PSP_NodeCollectionView { get; private set; }
        private List<Tree<string>> treeList = new();
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
            DeleteCommand = new ActionCommand(OnDeleteExecuted, OnDeleteCanExecute);
            OrdTask = new NotifyTaskCompletion<ICollectionView>(LoadOrderDataAsync());
            PspTask = new NotifyTaskCompletion<ICollectionView>(LoadPspDataAsync()); 
            
        }


        private bool OnDeleteCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.ProjectDel);
        }

        private void OnDeleteExecuted(object obj)
        {
            TreeNode<string> t = (TreeNode<string>)obj;
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            if (t.NodeType == "PSP-Type")
            {
                var psp = db.Projects.FirstOrDefault(x => x.ProjectPsp == t.Value);
                if (psp != null)
                {
                    db.Projects.Remove(psp);
                    tree?.Nodes.Remove(t);
                    PSP_NodeCollectionView?.Refresh();
                }
            }
            else
            {
                var ord = db.OrderRbs.FirstOrDefault(x => x.Aid == t.Value);
                if (ord != null)
                {
                    ord.ProId = null;
                    tree?.Nodes.Remove(t);
                    PSP_NodeCollectionView?.Refresh();
                }
            }
            db.SaveChanges();
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

        private bool OnConcatCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.ConcatProject);
        }

        private void OnConcatExecuted(object obj)
        {
            if (obj == null) return;
            var values = (object[])obj;
            var psp = (string)values[0];
            var aid = (string)values[1];

            if (!IsValidPsp(psp))
            {
                MessageBox.Show("PSP-Element ist ungültig!\nBitte korrigieren.", "Eingabefehler",
                MessageBoxButton.OK, MessageBoxImage.Exclamation); return;
            }
            if (aid == null)
            {
                MessageBox.Show("Auftragsnummer fehlt!\nBitte eintragen", "Eingabefehler",
                MessageBoxButton.OK, MessageBoxImage.Exclamation); return;
            }
            if (_ordersList.All(x => x.Aid != aid))
            {
                MessageBox.Show("Auftragsnummer ist nicht vorhanden!", "Eingabefehler",
                MessageBoxButton.OK, MessageBoxImage.Exclamation); return;
            }

            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            int rootLength = psp.StartsWith("ds", StringComparison.OrdinalIgnoreCase) ? 9 : 15;
            var root = tree?.Nodes.FirstOrDefault(x => psp[..rootLength] == x.Value);
            if (root == null)
            {
                var t = tree?.Begin(psp[..rootLength]);
                root = t.Nodes.Last();
            }
            for (int i = rootLength+3; i <= psp.Length; i += 3)
            {
                var pre = root.Children.FirstOrDefault(x => psp[..i] == x.Value);
                if (pre == null)
                {
                    pre = root.Add(psp[..i]);
                }


                root = pre;
            }
            if (root.Value.Length == psp.Length)
            {
                if (root.Children.All(x => x.Value != aid))
                {
                    root.Add(aid);
                }
            }
            while (tree?.level > 0) { tree.End(); }

            if (db.Projects.All(x => x.ProjectPsp != psp)) db.Database.ExecuteSqlRaw("INSERT INTO DBO.Project(ProjectPsp) VALUES({0})", psp);

            db.OrderRbs.First(x => x.Aid == aid).ProId = psp;
            db.SaveChanges();
            PSP_NodeCollectionView?.Refresh();
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
                .ToListAsync();

            var uiContext = TaskScheduler.FromCurrentSynchronizationContext();
            int typeLength;

            Tree<string> taskTree = new();

            await Task.Factory.StartNew(() =>
            {
                Tree<string> preTree = new();

                foreach (var item in proj.OrderBy(x => x.ProjectPsp))
                {
                    var p = item.ProjectPsp.Trim();
                    typeLength = p.StartsWith("ds", StringComparison.OrdinalIgnoreCase) ? 9 : 15;
                    var root = taskTree.Nodes.FirstOrDefault(y => p[..typeLength] == y.Value);
                    if (root == null)
                    {
                        var tr = taskTree.Begin(p[..typeLength]);
                        root = tr.Nodes.Last();

                    }
                    for (int i = typeLength+3; i <= p.Length; i += 3)
                    {
                        var pre = root.Children.FirstOrDefault(x => x.Value == p[..i]);
                        if (pre == null)
                        {
                            pre = root.Add(p[..i]);
                        }

                        root = pre;
                    }
                    if (item.ProjectPsp.Trim().Length == p.Length)
                    {
                        foreach (var o in item.OrderRbs)
                        {
                            if (root.Children.All(x => x.Value != o.Aid))
                            {
                                var n = new TreeNode<string>(o.Aid, root);
                                n.Description = string.Format("{0} {1}", o.Material, o.MaterialNavigation?.Bezeichng);
                                root.Children.Add(n);
                            }
                        }
                        root.Description = item.ProjectInfo ?? string.Empty;
                        root.ProjectType = (ProjectTypes.ProjectType)item.ProjectType;
                        root.PropertyChanged += OnPspPropertyChanged;
                    }
                    while (taskTree.level > 0)
                        taskTree.End();
                }

            }, CancellationToken.None, TaskCreationOptions.None, uiContext);
            tree = taskTree;
            PSP_NodeCollectionView = CollectionViewSource.GetDefaultView(tree.Nodes);
            PSP_NodeCollectionView.Filter += FilterPredicatePsp;
            return PSP_NodeCollectionView;
        }

        private void OnPspPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var tn = sender as TreeNode<string>;
            if (tn != null)
            {
                using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                var pr = db.Projects.Single(x => x.ProjectPsp == tn.Value);
                if (e.PropertyName == "ProjectType" &&
                    PermissionsProvider.GetInstance().GetUserPermission(Permissions.ProjectTypeChange)) pr.ProjectType = (int)tn.ProjectType;
                if (e.PropertyName == "Description" &&
                    PermissionsProvider.GetInstance().GetUserPermission(Permissions.ProjectDesript)) pr.ProjectInfo = tn.Description;

                db.SaveChanges();
            }
            
        }

        private bool FilterPredicatePsp(object obj)
        {
            var psp = (TreeNode<string>)obj;
            var search = ClearPsp(_projectSearchText);

            bool accepted = ClearPsp(psp.Value).Contains(search, StringComparison.CurrentCultureIgnoreCase);
            if (!accepted)
            {
                if (psp.Children != null && search != string.Empty)
                {
                    foreach (var tree in psp.Children)
                    {
                        accepted = ClearPsp(tree.Value).Contains(search, StringComparison.CurrentCultureIgnoreCase);
                        if (accepted) break;
                    }
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
