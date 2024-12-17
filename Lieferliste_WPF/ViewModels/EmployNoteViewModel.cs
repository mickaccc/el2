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

        public IEnumerable<dynamic> VorgangRef { get; private set; }
        private object _SelectedVorgangItem;
        public object SelectedVorgangItem
        {
            get { return _SelectedVorgangItem; }
            set
            {
                _SelectedVorgangItem = value;
                var obj = (VorgItem) _SelectedVorgangItem;
                ReferencePre = string.Format("{0} - {1:D4}\n{2} {3}", obj.Auftrag, obj.Vorgang, obj.Material, obj.Bezeichnung);
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

        public ObservableCollection<EmployeeNote> EmployeeNotes { get; private set; } = [];
        private List<string> CalendarWeeks { get; set; }
        public ICollectionView CalendarWeeksView { get; private set; }
        private int _CalendarWeek;

        public int CalendarWeek
        {
            get { return _CalendarWeek; }
            set
            {
                if (_CalendarWeek != value)
                {
                    _CalendarWeek = value;
                }
            }
        }

        public List<User> Users { get; private set; }
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
                .Select(s => new VorgItem(s.Aid, s.Vnr.ToString(),
                s.AidNavigation.Material, s.AidNavigation.MaterialNavigation.Bezeichng))];

            EmployeeNotes.AddRange(db.EmployeeNotes.Where(x => x.AccId.Equals(UserInfo.User.UserId)).OrderBy(x => x.Date));
            CalendarWeeks = [.. GetKW_Array()];
            CalendarWeeksView = CollectionViewSource.GetDefaultView(CalendarWeeks);
            CalendarWeeksView.CurrentChanged += WeekChanged;
            var test = db.IdmAccounts.Where(x => x.AccountCosts.Join(UserInfo.User.AccountCostUnits, x => x.CostId,);//.Select(x => new User(x.AccountId, x.Firstname, x.Lastname, null )).ToList();
            SelectedWeekDay = DateTime.Today.DayOfWeek;

        }

        private void WeekChanged(object? sender, EventArgs e)
        {
            
        }

        private string[] GetKW_Array()
        {
            string[] ret = new string[3];
            var date = DateTime.Today.AddDays(-7*3);

            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = string.Format("KW {0}", CultureInfo.CurrentCulture.Calendar
                    .GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday));
                date = date.AddDays(7);
            }
            return ret;
        }
    }
    public class VorgItem
    {
        public string Auftrag { get; }
        public string Vorgang { get; }
        public string? Material { get; }
        public string? Bezeichnung { get; }
        public VorgItem(string Auftrag, string Vorgang, string? Material, string? Bezeichnung)
        {
            this.Auftrag = Auftrag;
            this.Vorgang = Vorgang;
            this.Material = Material;
            this.Bezeichnung = Bezeichnung;
        }
    }
}
