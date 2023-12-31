using BionicCode.Utilities.Net.Standard.Extensions;
using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Utils;
using GongSolutions.Wpf.DragDrop;
using Lieferliste_WPF.Utilities;
using Lieferliste_WPF.ViewModels;
using Lieferliste_WPF.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Lieferliste_WPF.Planning
{ 
    public interface IPlanWorkerFactory
    {
        IContainerProvider Container {  get; }
        IApplicationCommands ApplicationCommands { get; }
        IEventAggregator EventAggregator { get; }
    }
    internal class PlanWorkerFactory : IPlanWorkerFactory
    {
        public IContainerProvider Container { get; }

        public IApplicationCommands ApplicationCommands { get; }

        public IEventAggregator EventAggregator { get; }

        public PlanWorkerFactory(IContainerProvider container, IApplicationCommands applicationCommands, IEventAggregator eventAggregator)
        {
            this.Container = container;
            this.ApplicationCommands = applicationCommands;
            this.EventAggregator = eventAggregator;
        }
        public PlanWorker CreatePlanWorker(string UserId)
        {
            return new PlanWorker(UserId, Container, ApplicationCommands, EventAggregator);
        }
    }
    public interface IPlanWorker
    {
        public string UserId { get; }
    }
    internal class PlanWorker : DependencyObject, IPlanWorker, IDropTarget
    {

        #region Constructors
 
        public PlanWorker(string UserId, IContainerProvider container, IApplicationCommands applicationCommands, IEventAggregator eventAggregator)
        {
            _container = container;
            _userId = UserId;
            _applicationCommands = applicationCommands;
            _eventAggregator = eventAggregator;
            Initialize();
            LoadData();
        }

        #endregion

        public ICommand? SetMarkerCommand { get; private set; }
        public ICommand? MachinePrintCommand {  get; private set; }

        private readonly string _userId;

        public string UserId => _userId;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? InventNo { get; private set; }
        public WorkArea? WorkArea { get; set; }
        public List<int> CostUnits { get; set; } = [];
        private DB_COS_LIEFERLISTE_SQLContext _dbctx;
        protected MachinePlanViewModel? Owner { get; }

        public ObservableCollection<Vorgang>? Processes { get; set; }

        public ICollectionView ProcessesCV { get { return ProcessesCVSource.View; } }
        private IContainerProvider _container;
        private IEventAggregator _eventAggregator;
        private IApplicationCommands? _applicationCommands;

        public IApplicationCommands? ApplicationCommands
        {
            get { return _applicationCommands; }
            set
            {
                if (_applicationCommands != value)
                {
                    _applicationCommands = value;
                }
            }
        }

        internal CollectionViewSource ProcessesCVSource { get; set; } = new CollectionViewSource();

        

        private void LoadData()
        {
            var usr = _dbctx.Users.AsNoTracking()
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .Include(x => x.UserCosts)
                .Include(x => x.RessourceUsers)
                .ThenInclude(x => x.RidNavigation)
                .Single(x => x.UserIdent == UserId);

            var vrg = _dbctx.Vorgangs
                .Include(x => x.UserVorgangs)
                .Where(x => x.UserVorgangs.Any(x => x.UserId == UserId))
                .ToList();

            Processes.AddRange(vrg);
            Name = usr.UsrName;
            Description = usr.UsrInfo;
    
        }
        private void Initialize()
        {
            _dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            SetMarkerCommand = new ActionCommand(OnSetMarkerExecuted, OnSetMarkerCanExecute);
            MachinePrintCommand = new ActionCommand(OnMachinePrintExecuted, OnMachinePrintCanExecute);
            Processes = new ObservableCollection<Vorgang>();
            ProcessesCVSource.Source = Processes;
            ProcessesCV.SortDescriptions.Add(new SortDescription("Spos", ListSortDirection.Ascending));
            ProcessesCV.Filter = f => !((Vorgang)f).SysStatus?.Contains("RÜCK") ?? false;

            var live = ProcessesCV as ICollectionViewLiveShaping;
            if (live != null)
            {
                live.IsLiveSorting = false;
                live.LiveFilteringProperties.Add("SysStatus");
                live.IsLiveFiltering = true;
            }
            _eventAggregator.GetEvent<MessageVorgangChanged>().Subscribe(MessageReceived);

        }

        private void MessageReceived(Vorgang vorgang)
        {
            var pr = Processes?.FirstOrDefault(x => x.VorgangId == vorgang.VorgangId);
            if (pr != null)
            {
                pr.SysStatus = vorgang.SysStatus;
                pr.QuantityMiss = vorgang.QuantityMiss;
                pr.QuantityScrap = vorgang.QuantityScrap;
                pr.QuantityRework = vorgang.QuantityRework;
                pr.QuantityYield = vorgang.QuantityYield;
                pr.BemT = vorgang.BemT;
                pr.CommentMach = vorgang.CommentMach;

            }
        }

        private bool OnMachinePrintCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.MachPrint);
        }

        private void OnMachinePrintExecuted(object obj)
        {
            try
            {
                Printing.DoThePrint(Printing.CreateFlowDocument(obj));
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message, "MachinePrint", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static bool OnSetMarkerCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.MessSetMark);
        }

        private void OnSetMarkerExecuted(object obj)
        {
            try
            {
                if (obj == null) return;
                var values = (object[])obj;
                var name = (string)values[0];
                var desc = (Vorgang)values[1];

                if (name == "DelBullet") desc.Bullet = Brushes.White.ToString();
                if (name == "Bullet1") desc.Bullet = Brushes.Red.ToString();
                if (name == "Bullet2") desc.Bullet = Brushes.Green.ToString();
                if (name == "Bullet3") desc.Bullet = Brushes.Yellow.ToString();
                if (name == "Bullet4") desc.Bullet = Brushes.Blue.ToString();

                ProcessesCV.Refresh();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message, "SetMarkerMess", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
 
        public void Drop(IDropInfo dropInfo)
        {

            try
            {
                var vrg = (Vorgang)dropInfo.Data;
                var s = dropInfo.DragInfo.SourceCollection as ListCollectionView;
                var t = dropInfo.TargetCollection as ListCollectionView;
                if (s.CanRemove) s.Remove(vrg);
                var v = dropInfo.InsertIndex;

                if (v > t?.Count)
                {
                    ((IList)t.SourceCollection).Add(vrg);
                }
                else
                {
                    ((IList)t.SourceCollection).Insert(v, vrg);
                }
                var p = t.SourceCollection as Collection<Vorgang>;
                
                for (var i = 0; i < p.Count; i++)
                {
                    p[i].Spos = i;

                }

                _dbctx.UserVorgangs.AddAsync(new UserVorgang() { UserId = this.UserId, Vid = vrg.VorgangId });
                _dbctx.SaveChanges();

                t.Refresh();

            }
            catch (Exception e)
            {
                string str = string.Format("{0}\n{1}",e.Message, e.InnerException);
                MessageBox.Show(str, "ERROR", MessageBoxButton.OK);
            }
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (PermissionsProvider.GetInstance().GetUserPermission(Permissions.MessDrop))
            {
                if (dropInfo.Data is Vorgang)
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = DragDropEffects.Move;
                } 
            }
        }
    }
}
