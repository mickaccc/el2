using CompositeCommands.Core;
using El2Core.Models;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System;
using El2Core.Converters;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCompress.Common;
using System.Text.RegularExpressions;
using System.Windows.Input;
using El2Core.Utils;
using El2Core.Constants;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace Lieferliste_WPF.ViewModels
{
    public class ProjectEditViewModel : ViewModelBase
    {
        public string Title { get; } = "Projekt Editor";
        IContainerProvider _container;
        IApplicationCommands _applicationCommands;
        private RelayCommand? _orderSearchCommand;
        private RelayCommand? _projectSearchCommand;
        public ICommand OrderSearchCommand => _orderSearchCommand ??= new RelayCommand(OnOrderSearch);
        public ICommand ProjectSearchCommand => _projectSearchCommand ??= new RelayCommand(OnProjectSearch);
        public ICommand ConcatCommand { get; private set; }
        public ICommand DescriptLostFocusCommand { get; private set; }
        private ObservableCollection<OrderRb> _ordersList = [];
        public ICollectionView OrdersCollectionView { get; private set; }
        private ObservableCollection<TreeNode> _PSP_Nodes = [];
        public ICollectionView PSP_NodeCollectionView { get; private set; }
        private string _orderSearchText = string.Empty;
        private string _projectSearchText = string.Empty;
        private static readonly object _lock = new();

        public NotifyTaskCompletion<ICollectionView>? OrdTask { get; private set; }
        public NotifyTaskCompletion<ICollectionView>? PspTask { get; private set; }

        public ProjectEditViewModel(IContainerProvider container, IApplicationCommands applicationCommands)
        {
            _container = container;
            _applicationCommands = applicationCommands;
            ConcatCommand = new ActionCommand(OnConcatExecuted, OnConcatCanExecute);
            DescriptLostFocusCommand = new ActionCommand(OnDescriptLostFocusExecuted, OnDescriptLostFocusCanExecute);
            OrdTask = new NotifyTaskCompletion<ICollectionView>(LoadOrderDataAsync());
            PspTask = new NotifyTaskCompletion<ICollectionView>(LoadPspDataAsync());
        }

        private bool OnDescriptLostFocusCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.ProjectDesript);
        }

        private void OnDescriptLostFocusExecuted(object obj)
        {
            var tree = (TreeNode)obj;
            if (tree.IsChanged)
            {
                using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                db.Projects.First(x => x.Project1 == tree.PSP).ProjectInfo = tree.Description;
                db.SaveChanges();
            }
        }

        private void OnProjectSearch(object obj)
        {
            if (obj != null)
            {
                _projectSearchText = (string)obj;
                if (_projectSearchText.Length >= 5)
                    PSP_NodeCollectionView.Refresh();
            }
        }
        private void OnOrderSearch(object obj)
        {
            if (obj != null)
            {
                _orderSearchText = (string)obj;
                OrdersCollectionView.Refresh();
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
            psp = ConvertPsp(psp);
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
            var pspNode = _PSP_Nodes.FirstOrDefault(x => psp.Contains(x.PSP));
            if (pspNode != null)
            {
                for (int i = 12; i <= psp.Length; i += 3)
                {
                    var p = pspNode.GetChild(psp[..i]);
                    if (p != null) { pspNode = p; } else { pspNode.Add(new TreeNode(psp)); pspNode = pspNode.GetChild(psp); }
                }
            }
            else
            {
                pspNode = new TreeNode(psp);
                db.Projects.Add(new Project() { Project1 = psp });
            }

            pspNode.Add(new TreeNode(aid));
            db.OrderRbs.First(x => x.Aid == aid).ProId = psp;
            db.SaveChanges();
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
                .Skip(200)
                .Include(x => x.OrderRbs)
                .ToListAsync();

            await Task.Factory.StartNew(() =>
            {
                HashSet<TreeNode> nodes = new();
                lock (_lock)
                {
                    foreach (var project in proj)
                    {
                        string psp = project.Project1.Trim();
                        TreeNode tree;
                        if (nodes.All(x => x.PSP != psp[..9]))
                        {
                            tree = new TreeNode(psp[..9]);
                            tree.Description = project.ProjectInfo;
                            if(project.OrderRbs != null)
                            {
                                foreach (var rbs in project.OrderRbs)
                                    tree.Add(new TreeNode(rbs.Aid));
                            }
                            tree.ChangeTracker = true;
                            nodes.Add(tree);
                        }
                        else
                        {
                            tree = nodes.First(x => x.PSP == psp[..9]);
                        }
                        for (int i = 12; i <= psp.Length; i += 3)
                        {
                            if (tree.GetChild(psp[..i]) == null)
                            {
                                var t = new TreeNode(psp[..i]);
                                t.Description = project.ProjectInfo;
                                if (project.OrderRbs != null)
                                {
                                    foreach (var rbs in project.OrderRbs)
                                        tree.Add(new TreeNode(rbs.Aid));
                                }
                                t.ChangeTracker = true;

                                tree.Add(t);
                            }
                        }
                    }
                    _PSP_Nodes.AddRange(nodes);
                }              
            });

            PSP_NodeCollectionView = CollectionViewSource.GetDefaultView(_PSP_Nodes);
            PSP_NodeCollectionView.Filter += FilterPredicatePsp;
            return PSP_NodeCollectionView;
        }

        private bool FilterPredicatePsp(object obj)
        {
            var psp = (TreeNode)obj;

            bool accepted = ClearPsp(psp.PSP).Contains(_projectSearchText, StringComparison.CurrentCultureIgnoreCase);
            if (psp.Children != null)
            {
                foreach (var tree in psp.Children)
                {
                    accepted = ClearPsp(tree.PSP).Contains(_projectSearchText, StringComparison.CurrentCultureIgnoreCase);
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

            Regex regex = new Regex("(DS)([0-9]{6})([0-9]{2})*");
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
