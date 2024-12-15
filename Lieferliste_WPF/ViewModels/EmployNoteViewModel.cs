using El2Core.Models;
using El2Core.Utils;
using El2Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using El2Core.ViewModelBase;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Automation;

namespace Lieferliste_WPF.ViewModels
{
    internal class EmployNoteViewModel : ViewModelBase
    {
        public EmployNoteViewModel(IContainerProvider containerProvider)
        {
            container = containerProvider;
            LoadingData();
        }


        public string Title { get; } = "Arbeitszeiten";
        IContainerProvider container;
        public ListItem[] VorgangRef { get; private set; }
        public ICollectionView VorgangRefView { get; private set; }
        private string? _selectedVrg;
        public string? SelectedVrg
        {
            get { return _selectedVrg; }
            set
            {
                if (_selectedVrg != value)
                {
                    _selectedVrg = value;
                    VorgangRefView.Refresh();
                }
            }
        }
        private string? _SelectedVrgPath;

        public string? SelectedVrgPath
        {
            get { return _SelectedVrgPath; }
            set { _SelectedVrgPath = value; }
        }

        public ObservableCollection<EmployeeNote> EmployeeNotes { get; private set; } = [];
        public List<string> CalendarWeeks { get; private set; }
        private DayOfWeek _SelectedWeekDay;

        public DayOfWeek SelectedWeekDay
        {
            get { return _SelectedWeekDay; }
            set
            {
                _SelectedWeekDay = value;
                NotifyPropertyChanged(() => SelectedWeekDay);
            }
        }

        private void LoadingData()
        {
            using var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            VorgangRef = [.. db.Vorgangs
                .Include(x => x.AidNavigation)
                .Include(x => x.AidNavigation.MaterialNavigation)
                .Include(x => x.AidNavigation.DummyMatNavigation)
                .Where(x => x.AidNavigation.Abgeschlossen == false)
                .OrderBy(x => x.Aid)
                .ThenBy(x => x.Vnr)
                .Select(static s => new ListItem {
                    Auftrag = s.Aid,
                    Vorgang = s.Vnr.ToString(),
                    Material = s.AidNavigation.Material,
                    Bezeichnung = s.AidNavigation.MaterialNavigation.Bezeichng })];

            EmployeeNotes.AddRange(db.EmployeeNotes.Where(x => x.AccId.Equals(UserInfo.User.UserId)).OrderBy(x => x.Date));
            CalendarWeeks = ["KW30", "KW31", "KW32"];
            SelectedWeekDay = DateTime.Today.DayOfWeek;
            VorgangRefView = CollectionViewSource.GetDefaultView(VorgangRef);

        }

        private bool FilterPredicate(object obj)
        {
            if (obj is Vorgang v && SelectedVrg != null)
            {
                return v.Aid.Contains(SelectedVrg);
            }
            return true;
        }

        public string[] SelectedRef { get; } =
        [
            "Reinigen", "Cip", "Anlernen"
        ];
        public struct ListItem
        {
            public string Auftrag;
            public string Vorgang;
            public string? Material;
            public string? Bezeichnung;
        }
    }
}
