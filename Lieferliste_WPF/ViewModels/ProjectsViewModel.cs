using El2Core.Models;
using El2Core.Services;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Lieferliste_WPF.ViewModels
{
    class ProjectsViewModel : ViewModelBase, IViewModel
    {
        private string _title = "Projektübersicht";
        public string Title => _title;

        public bool HasChange => _dbctx.ChangeTracker.HasChanges();
        private DB_COS_LIEFERLISTE_SQLContext _dbctx;
        private IUserSettingsService _userSettingsService;
        private IContainerProvider _container;
        private NotifyTaskCompletion<ICollectionView> _projTask;
        public NotifyTaskCompletion<ICollectionView> ProjTask
        {
            get { return _projTask; }
            set
            {
                if (_projTask != value)
                {
                    _projTask = value;
                    NotifyPropertyChanged(() => ProjTask);
                }
            }
        }
        private List<Project> _projects;
        public ICollectionView ProjectView { get; private set; }
        public ProjectsViewModel(IContainerProvider container, IUserSettingsService userSettingsService)
        {
            _container = container;
            _userSettingsService = userSettingsService;
            _dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();

            ProjTask = new NotifyTaskCompletion<ICollectionView>(LoadAsync());
        }

        private async Task<ICollectionView> LoadAsync()
        {
            var pro = await _dbctx.Projects
                .Include(x => x.OrderRbs)
                .ThenInclude(x => x.MaterialNavigation)
                .ToListAsync();

            _projects = new List<Project>(pro);
            ProjectView = CollectionViewSource.GetDefaultView(_projects);
            return ProjectView;
        }

        public void Closing()
        {
            if (_dbctx.ChangeTracker.HasChanges())
            {
                if (_userSettingsService.IsSaveMessage)
                {
                    var result = MessageBox.Show(string.Format("Sollen die Änderungen in {0} gespeichert werden?", _title),
                        _title, MessageBoxButton.YesNo, MessageBoxImage.Question);
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
