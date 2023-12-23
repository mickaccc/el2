using CompositeCommands.Core;
using El2Core.Models;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Lieferliste_WPF.ViewModels
{
    internal class MeasuringRoomViewModel : ViewModelBase, IDropTarget
    {
        public string Title { get; } = "Messraum Zuteilung";
        private IContainerProvider _container;
        private IApplicationCommands _applicationCommands;
        public NotifyTaskCompletion<ICollectionView> MemberTask {get; private set;}
        public NotifyTaskCompletion<ICollectionView> VorgangTask { get; private set;}
        private DB_COS_LIEFERLISTE_SQLContext _dbctx;
        private ObservableCollection<Vorgang> _vorgangsList = new();
        private List<PlanWorker> _emploeeList = new();
        public ICollectionView EmploeeList { get; private set;}
        public ICollectionView VorgangsView { get; private set; }

        public MeasuringRoomViewModel(IContainerProvider container, IApplicationCommands applicationCommands)
        { 
            _container = container;
            _applicationCommands = applicationCommands;
            _dbctx = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            VorgangTask = new NotifyTaskCompletion<ICollectionView>(LoadDataAsync());
            MemberTask = new NotifyTaskCompletion<ICollectionView>(LoadMemberAsync());
        }

        private async Task<ICollectionView> LoadDataAsync()
        {
            var ord = await _dbctx.Vorgangs
                .Include(x => x.AidNavigation)
                .ThenInclude(x => x.MaterialNavigation)
                .Include(x => x.AidNavigation.DummyMatNavigation)
                .Include(x => x.ArbPlSapNavigation)
                .Where(x => x.ArbPlSapNavigation.Ressource.ProcessAddable == false)
                .ToListAsync();
            _vorgangsList.AddRange(ord);

            VorgangsView = CollectionViewSource.GetDefaultView(_vorgangsList);
            return VorgangsView;

        }
        private async Task<ICollectionView> LoadMemberAsync()
        {
            var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var mem = await db.Users
                .Where(x => x.RessourceUsers.Any(x => x.RidNavigation.ProcessAddable == false))
                .ToListAsync();

                var factory = _container.Resolve<PlanWorkerFactory>();
                foreach (var item in mem)
                {
                    _emploeeList.Add(factory.CreatePlanWorker(item.UserIdent));
                }



            EmploeeList = CollectionViewSource.GetDefaultView (_emploeeList);
            return EmploeeList;
        }
        public void DragOver(IDropInfo dropInfo)
        {
            
        }

        public void Drop(IDropInfo dropInfo)
        {
            
        }
    }
}
