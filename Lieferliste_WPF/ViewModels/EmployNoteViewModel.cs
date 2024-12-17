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
using System.Reflection;

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

        public IEnumerable<dynamic> VorgangRef { get; private set; }
        private VorgItem _SelectedVorgangItem;
        public VorgItem SelectedVorgangItem
        {
            get { return _SelectedVorgangItem; }
            set
            {

                _SelectedVorgangItem = value;
                if (value != null)
                {
                    ReferencePre = string.Format("{0} - {1:D4}\n{2} {3}",
                        value.Auftrag, value.Vorgang, value.Material, value.Bezeichnung);
                }
            }
        }
        private string _ReferencePre;

        public string ReferencePre
        {
            get { return _ReferencePre; }
            set
            {
                if (_ReferencePre != value)
                {
                    _ReferencePre = value;
                    NotifyPropertyChanged(()  => ReferencePre);
                }
            }
        }

        private string? _selectedRef;
        public string? SelectedRef
        {
            get { return _selectedRef; }
            set
            {
                if (_selectedRef != value)
                {
                    _selectedRef = value;
                    ReferencePre = value ?? string.Empty;
                }
            }
        }
        public string[] SelectedRefs { get; } = [ "Reinigen", "Cip", "Anlernen" ];
        private string? _SelectedVrgPath;

        public string? SelectedVrgPath
        {
            get { return _SelectedVrgPath; }
            set { _SelectedVrgPath = value; }
        }
        private IdmAccount _SelectedUser;

        public IdmAccount SelectedUser
        {
            get { return _SelectedUser; }
            set
            {
                if (_SelectedUser != value)
                {
                    _SelectedUser = value;
                    EmployeeNotesView.Refresh();
                }
            }
        }

        private ObservableCollection<EmployeeNote> EmployeeNotes { get; } = [];
        public ICollectionView EmployeeNotesView { get; private set; }
        public List<string> CalendarWeeks { get; private set; }
  
        private int _CalendarWeek;

        public int CalendarWeek
        {
            get { return _CalendarWeek; }
            set
            {
                if (_CalendarWeek != value)
                {
                    _CalendarWeek = value;
                    EmployeeNotesView.Refresh();
                }
            }
        }

        public List<IdmAccount> Users { get; private set; }
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
                .Where(x => x.AidNavigation.Abgeschlossen)
                .OrderBy(x => x.Aid)
                .ThenBy(x => x.Vnr)
                .Select(s => new VorgItem(s.VorgangId, s.Aid, s.Vnr.ToString(),
                s.AidNavigation.Material, s.AidNavigation.MaterialNavigation.Bezeichng))];

            EmployeeNotes.AddRange(db.EmployeeNotes.Where(x => x.AccId.Equals(UserInfo.User.UserId)).OrderBy(x => x.Date));
            EmployeeNotesView = CollectionViewSource.GetDefaultView(EmployeeNotes);
            EmployeeNotesView.Filter += FilterPredicate;
            CalendarWeeks = GetKW_Array();
 
            Users = [];
            foreach (var cost in UserInfo.User.AccountCostUnits)
            {
                Users.AddRange(db.IdmAccounts.Where(x => x.AccountCosts.Any(y => y.CostId == cost.CostId)));
            }
 
            SelectedWeekDay = DateTime.Today.DayOfWeek;
        }

        private bool FilterPredicate(object obj)
        {
            if(obj is EmployeeNote note)
            {
                int cw = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Today.AddDays(CalendarWeek), CalendarWeekRule.FirstFourDayWeek,
                    DayOfWeek.Sunday);
                return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(note.Date, CalendarWeekRule.FirstFourDayWeek,
                    DayOfWeek.Sunday) == cw
                    && note.AccId == SelectedUser.AccountId;
            }
            return false;
        }

        private List<string> GetKW_Array()
        {
            List<string> ret = [];
            var date = DateTime.Today.AddDays(-7*3);

            for (int i = 0; i < 3; i++)
            {
                ret.Add(string.Format("KW {0}", CultureInfo.CurrentCulture.Calendar
                    .GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday)));
                date = date.AddDays(7);
            }
            return ret;
        }
    }
    public class VorgItem
    {
        public string VorgangId { get; }
        public string Auftrag { get; }
        public string Vorgang { get; }
        public string? Material { get; }
        public string? Bezeichnung { get; }
        public VorgItem(string VorgangId, string Auftrag, string Vorgang, string? Material, string? Bezeichnung)
        {
            this.VorgangId = VorgangId;
            this.Auftrag = Auftrag;
            this.Vorgang = Vorgang;
            this.Material = Material;
            this.Bezeichnung = Bezeichnung;
        }
    }
}
