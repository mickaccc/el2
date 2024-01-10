using BionicCode.Utilities.Net.Standard.Extensions;
using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
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
        IContainerProvider Container { get; }
        IApplicationCommands ApplicationCommands { get; }
        IEventAggregator EventAggregator { get; }
        IUserSettingsService SettingsService { get; }
    }
    internal class PlanWorkerFactory : IPlanWorkerFactory
    {
        public IContainerProvider Container { get; }

        public IApplicationCommands ApplicationCommands { get; }

        public IEventAggregator EventAggregator { get; }

        public IUserSettingsService SettingsService { get; }

        public PlanWorkerFactory(IContainerProvider container, IApplicationCommands applicationCommands, IEventAggregator eventAggregator, IUserSettingsService settingsService)
        {
            this.Container = container;
            this.ApplicationCommands = applicationCommands;
            this.EventAggregator = eventAggregator;
            this.SettingsService = settingsService;
        }
        public PlanWorker CreatePlanWorker(string UserId)
        {
            return new PlanWorker(UserId, Container, ApplicationCommands, EventAggregator, SettingsService);
        }
    }
    public interface IPlanWorker
    {
        public string UserId { get; }
    }
    internal class PlanWorker : DependencyObject, IPlanWorker, IDropTarget, IViewModel
    {

        #region Constructors

        public PlanWorker(string UserId, IContainerProvider container,
            IApplicationCommands applicationCommands,
            IEventAggregator eventAggregator,
            IUserSettingsService settingsService)
        {
            _container = container;
            _userId = UserId;
            _applicationCommands = applicationCommands;
            _eventAggregator = eventAggregator;
            _settingsService = settingsService;
            Initialize();
            LoadData();

        }

        #endregion

        public ICommand? SetMarkerCommand { get; private set; }
        public ICommand? WorkerPrintCommand { get; private set; }

        private readonly string _userId;

        public string UserId => _userId;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? PersNo { get; private set; }
        public WorkArea? WorkArea { get; set; }
        public List<int> CostUnits { get; set; } = [];
        private DB_COS_LIEFERLISTE_SQLContext _dbctx;
        protected MachinePlanViewModel? Owner { get; }

        public ObservableCollection<Vorgang>? Processes { get; set; }

        public ICollectionView ProcessesCV { get { return ProcessesCVSource.View; } }
        private IContainerProvider _container;
        private IEventAggregator _eventAggregator;
        private IUserSettingsService _settingsService;
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
        private string _title = "Messtechnik";
        public string Title => _title;

        public bool HasChange => _dbctx.ChangeTracker.HasChanges();

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
                .Include(x => x.AidNavigation)
                .ThenInclude(x => x.MaterialNavigation)
                .Where(x => x.UserVorgangs.Any(x => x.UserId == UserId) && x.SysStatus.Contains("RÜCK") == false)
                .ToList();


            Processes.AddRange(vrg);
            Name = usr.UsrName;
            Description = usr.UsrInfo;
            PersNo = usr.Personalnumber;

            ProcessesCV.SortDescriptions.Add(new SortDescription("Spos", ListSortDirection.Ascending));
            ProcessesCV.Filter = f => !((Vorgang)f).SysStatus?.Contains("RÜCK") ?? false;

            var live = ProcessesCV as ICollectionViewLiveShaping;
            if (live != null)
            {
                live.IsLiveSorting = false;
                live.LiveFilteringProperties.Add("SysStatus");
                live.IsLiveFiltering = true;
            }
        }
        private void Initialize()
        {
            _dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            SetMarkerCommand = new ActionCommand(OnSetMarkerExecuted, OnSetMarkerCanExecute);
            WorkerPrintCommand = new ActionCommand(OnWorkerPrintExecuted, OnWorkerPrintCanExecute);
            Processes = new ObservableCollection<Vorgang>();
            ProcessesCVSource.Source = Processes;

            _eventAggregator.GetEvent<MessageVorgangChanged>().Subscribe(MessageReceived);

        }

        private void MessageReceived(List<string> vorgangIdList)
        {
            try
            {
                foreach (var v in vorgangIdList)
                {

                    var pr = Processes?.FirstOrDefault(x => x.VorgangId == v);
                    if (pr != null)
                    {
                        _dbctx.ChangeTracker.Entries<Vorgang>().First(x => x.Entity.VorgangId == pr.VorgangId).ReloadAsync();

                        pr.RunPropertyChanged();
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "MsgReceivedPlanWorker", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool OnWorkerPrintCanExecute(object arg)
        {
            return PermissionsProvider.GetInstance().GetUserPermission(Permissions.MachPrint);
        }

        private void OnWorkerPrintExecuted(object obj)
        {
            try
            {
                Printing.DoPrintPreview(Printing.CreateFlowDocument(obj));
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message, "WorkerPrint", MessageBoxButton.OK, MessageBoxImage.Error);
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

                t.Refresh();

            }
            catch (Exception e)
            {
                string str = string.Format("{0}\n{1}", e.Message, e.InnerException);
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

        public void Closing()
        {
            if (_dbctx.ChangeTracker.HasChanges())
            {
                SaveQuestion();
            }
        }
        private bool SaveQuestion()
        {
            if (!_settingsService.IsSaveMessage)
            {
                _dbctx.SaveChangesAsync();
                return true;
            }
            else
            {
                var result = MessageBox.Show(string.Format("Sollen die Änderungen in {0} gespeichert werden?", _title),
                    _title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _dbctx.SaveChangesAsync();
                    return true;
                }
                else return false;
            }
        }
    }
}
