﻿using CompositeCommands.Core;
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
using Microsoft.IdentityModel.Tokens;

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
        private Tree<string> tree;
        public ICollectionView PSP_NodeCollectionView { get; private set; }
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
                }
            }
        }
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
            var tree = (TreeNode<string>)obj;
            if (!string.IsNullOrEmpty(tree.Description))
            {
                using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                var pinfo = db.Projects.First(x => x.Project1 == tree.Value).ProjectInfo;
                if (pinfo != tree.Description)
                {
                    db.Projects.First(x => x.Project1 == tree.Value).ProjectInfo = tree.Description;
                    db.SaveChanges();
                }
            }
        }

        private void OnProjectSearch(object obj)
        {
            if (obj != null)
            {
                ProjectSearchText = ConvertPsp((string)obj);
                if (_projectSearchText.Length >= 5 || _projectSearchText.Length == 0)
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
            var root = tree.Nodes.FirstOrDefault(x => psp[..9] == x.Value);
            if (root == null)
            {
                var t = tree.Begin(psp[..9]);
                t.End();
                root = t.Nodes.Last();
            }
            for (int i = 12; i <= psp.Length; i += 3)
            {
                var pre = root.Children.FirstOrDefault(x => psp[..i] == x.Value);
                if (pre == null)
                {
                    pre = root.Add(psp[..i]);
                }
                if (i == psp.Length)
                {
                    if (pre.Children.All(x => x.Value != aid))
                    {
                        pre.Add(aid);
                    }
                }
                root = pre;
            }

            if (db.Projects.All(x => x.Project1 != psp)) db.Database.ExecuteSqlRaw("INSERT INTO DBO.Project(Project) VALUES({0})", psp);

            db.OrderRbs.First(x => x.Aid == aid).ProId = psp;
            db.SaveChanges();
            PSP_NodeCollectionView.Refresh();
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
                .ToListAsync();

            Tree<string> taskTree = new();
            await Task.Factory.StartNew(() =>
            {
                
                foreach (var item in proj.OrderBy(x => x.Project1))
                {
                    var p = item.Project1.Trim();

                    var root = taskTree.Nodes.FirstOrDefault(y => p[..9] == y.Value);
                    if (root == null)
                    {
                        var tr = taskTree.Begin(p[..9]);
                        root = tr.Nodes.Last();

                    }
                    for (int i = 12; i <= p.Length; i += 3)
                    {
                        var pre = root.Children.FirstOrDefault(x => x.Value == p[..i]);
                        if (pre == null)
                        {
                            pre = root.Add(p[..i]);
                        }

                        root = pre;
                    }
                    if (item.Project1.Trim().Length == p.Length)
                    {
                        foreach (var o in item.OrderRbs)
                        {
                            if (root.Children.All(x => x.Value != o.Aid))
                            {
                                var n = new TreeNode<string>(o.Aid, root);
                                root.Children.Add(n);
                            }
                        }
                        root.Description = item.ProjectInfo ?? string.Empty;
                    }
                    while (taskTree.level > 0)
                        taskTree.End();                  
                }
            });
            tree = taskTree;
            PSP_NodeCollectionView = CollectionViewSource.GetDefaultView(tree.Nodes);
            PSP_NodeCollectionView.Filter += FilterPredicatePsp;
            return PSP_NodeCollectionView;
        }

        private bool FilterPredicatePsp(object obj)
        {
            var psp = (TreeNode<string>)obj;

            bool accepted = psp.Value.Contains(_projectSearchText, StringComparison.CurrentCultureIgnoreCase);
            if (psp.Children != null)
            {
                foreach (var tree in psp.Children)
                {
                    accepted = tree.Value.Contains(_projectSearchText, StringComparison.CurrentCultureIgnoreCase);
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