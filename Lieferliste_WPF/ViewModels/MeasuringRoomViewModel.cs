using CompositeCommands.Core;
using El2Core.Constants;
using El2Core.Models;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using GongSolutions.Wpf.DragDrop;
using Lieferliste_WPF.Planning;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Lieferliste_WPF.ViewModels
{
    internal class MeasuringRoomViewModel : ViewModelBase, IDropTarget, IViewModel
    {
        public string Title { get; } = "Messraum Zuteilung";
        public bool HasChange => _dbctx.ChangeTracker.HasChanges();
        private IContainerProvider _container;
        private IApplicationCommands _applicationCommands;
        private IUserSettingsService _settingsService;
        private RelayCommand? _textChangedCommand;
        public ICommand TextChangedCommand => _textChangedCommand ??= new RelayCommand(OnTextChanged);
        public NotifyTaskCompletion<ICollectionView> MemberTask { get; private set; }
        public NotifyTaskCompletion<ICollectionView> VorgangTask { get; private set; }
        private DB_COS_LIEFERLISTE_SQLContext _dbctx;
        private ObservableCollection<Vorgang> _vorgangsList = new();
        private List<PlanWorker> _emploeeList = new();
        private string _searchText = string.Empty;
        private static System.Timers.Timer _autoSaveTimer;

        public ICollectionView EmploeeList { get; private set; }
        public ICollectionView VorgangsView { get; private set; }

        public MeasuringRoomViewModel(IContainerProvider container, IApplicationCommands applicationCommands, IUserSettingsService settingsService)
        {
            _container = container;
            _applicationCommands = applicationCommands;
            _settingsService = settingsService;
            _dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            VorgangTask = new NotifyTaskCompletion<ICollectionView>(LoadDataAsync());
            MemberTask = new NotifyTaskCompletion<ICollectionView>(LoadMemberAsync());
            if (_settingsService.IsAutoSave) SetAutoSave();
        }
        private void SetAutoSave()
        {
            _autoSaveTimer = new System.Timers.Timer(60000);
            _autoSaveTimer.Elapsed += OnAutoSave;
            _autoSaveTimer.AutoReset = true;
            _autoSaveTimer.Enabled = true;
        }

        private void OnAutoSave(object? sender, ElapsedEventArgs e)
        {
            if (_dbctx.ChangeTracker.HasChanges()) _dbctx.SaveChangesAsync();
        }
        private async Task<ICollectionView> LoadDataAsync()
        {
            try
            {
                var ord = await _dbctx.Vorgangs
            .Include(x => x.AidNavigation)
            .ThenInclude(x => x.MaterialNavigation)
            .Include(x => x.AidNavigation.DummyMatNavigation)
            .Include(x => x.ArbPlSapNavigation)
            .Where(x => (x.ArbPlSapNavigation.Ressource != null) && x.ArbPlSapNavigation.Ressource.WorkAreaId == 5 &&
                    ((x.SysStatus != null) && x.SysStatus.Contains("RÜCK") == false))
            .ToListAsync();


                _vorgangsList.AddRange(ord.ExceptBy(_dbctx.UserVorgangs.Select(x => x.Vid), x => x.VorgangId));

                VorgangsView = CollectionViewSource.GetDefaultView(_vorgangsList);
                VorgangsView.Filter += FilterPredicate;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "MeasuringRoom Load Data", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return VorgangsView;

        }

        private async Task<ICollectionView> LoadMemberAsync()
        {
            var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var mem = await db.Users
                .Where(x => x.RessourceUsers.Any(x => x.RidNavigation.WorkAreaId == 5))
                .ToListAsync();

            var factory = _container.Resolve<PlanWorkerFactory>();
            foreach (var item in mem)
            {
                _emploeeList.Add(factory.CreatePlanWorker(item.UserIdent));
            }

            EmploeeList = CollectionViewSource.GetDefaultView(_emploeeList);
            return EmploeeList;
        }
        private void OnTextChanged(object obj)
        {
            if (obj is string s)
            {
                _searchText = s;
                VorgangsView.Refresh();
            }
        }
        private bool FilterPredicate(object obj)
        {
            bool accepted = true;
            if (obj is Vorgang vrg && _searchText != string.Empty)
            {
                if (!(accepted = vrg.Aid.Contains(_searchText, StringComparison.CurrentCultureIgnoreCase)))
                    if (!(accepted = vrg.AidNavigation.Material?.Contains(_searchText, StringComparison.CurrentCultureIgnoreCase) ?? false))
                        accepted = vrg.AidNavigation.MaterialNavigation?.Bezeichng?.Contains(_searchText, StringComparison.CurrentCultureIgnoreCase) ?? false;

            }
            return accepted;
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

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is Vorgang vrg)
            {
                var source = ((ListCollectionView)dropInfo.DragInfo.SourceCollection);
                if (source.IsAddingNew) { source.CommitNew(); }
                source.Remove(vrg);
                ((ListCollectionView)dropInfo.TargetCollection).AddNewItem(vrg);
                ((ListCollectionView)dropInfo.TargetCollection).CommitNew();
                _dbctx.UserVorgangs.RemoveRange(_dbctx.UserVorgangs.Where(x => x.Vid.Trim() == vrg.VorgangId));
 
            }
        }

        public void Closing()
        {
            if (_dbctx.ChangeTracker.HasChanges())
            {
                if (_settingsService.IsSaveMessage)
                {
                    var result = MessageBox.Show(string.Format("Sollen die Änderungen in {0} gespeichert werden?", Title),
                        Title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        _dbctx.SaveChanges();
                    }
                }
                else _dbctx.SaveChanges();
            }
        }
    }
}
