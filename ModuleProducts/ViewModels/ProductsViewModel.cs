﻿using CompositeCommands.Core;
using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ModuleProducts.ViewModels
{
    internal class ProductsViewModel : ViewModelBase
    {
        public ProductsViewModel(IContainerProvider container, IApplicationCommands applicationCommands)
        {
            _container = container;
            _applicationCommands = applicationCommands;
            var loggerFactory = _container.Resolve<ILoggerFactory>();
            _Logger = loggerFactory.CreateLogger<ProductsViewModel>();

            MaterialTask = new NotifyTaskCompletion<ICollectionView>(OnLoadMaterialsAsync());
        }

        IContainerProvider _container;
        ILogger _Logger;
        public ICollectionView ProductsView { get; private set; }
        private List<TblMaterial> _Materials;
        private IApplicationCommands _applicationCommands;
        public IApplicationCommands ApplicationCommands
        {
            get { return _applicationCommands; }
            set
            {
                if (_applicationCommands != null)
                {
                    _applicationCommands = value;
                    NotifyPropertyChanged(() => ApplicationCommands);
                }
            }
        }
        private NotifyTaskCompletion<ICollectionView>? _materialTask;

        public NotifyTaskCompletion<ICollectionView>? MaterialTask
        {
            get { return _materialTask; }
            set
            {
                if (_materialTask != value)
                {
                    _materialTask = value;
                    NotifyPropertyChanged(() => MaterialTask);
                }
            }
        }
        private async Task<ICollectionView> OnLoadMaterialsAsync()
        {
            try
            {
                using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();

                var mat = await db.TblMaterials.AsNoTracking()
                    .Include(x => x.OrderRbs)
                    .OrderBy(x => x.OrderRbs.First().Eckstart)
                    .ToListAsync();

                _Materials = mat;
                ProductsView = CollectionViewSource.GetDefaultView(_Materials);
                ProductsView.Filter += OnFilterPredicate;
            }
            catch (Exception e)
            {
                _Logger.LogError("{message}", e.ToString());
            }
            return ProductsView;
        }

        private bool OnFilterPredicate(object obj)
        {
            bool accept = false;
            if (obj is TblMaterial mat)
            {

            }
            return accept;
        }
    }
}
