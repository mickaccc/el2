using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Input;

namespace Lieferliste_WPF.ViewModels
{
    internal class EmployNoteViewModel : ViewModelBase
    {
        public EmployNoteViewModel(IContainerProvider containerProvider)
        {
            container = containerProvider;
            _ctx = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            LoadingData();
            SubmitCommand = new ActionCommand(OnSubmitExecuted, OnSubmitCanExecute);
        }

        public string Title { get; } = "Arbeitszeiten";
        IContainerProvider container;
        private DB_COS_LIEFERLISTE_SQLContext _ctx;
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
                    ReferencePre = string.Format("{0} - {1}\n{2} {3}",
                        value.Auftrag, value.Vorgang, value.Material.Trim(), value.Bezeichnung);
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
        private DateTime _SelectedDate;

        public DateTime SelectedDate
        {
            get { return _SelectedDate; }
            set
            {
                if (_SelectedDate != value)
                {
                    _SelectedDate = value;
                    NotifyPropertyChanged(() => SelectedDate);
                    EmployeeNotesView.Refresh();
                }
            }
        }
        private string _Comment = string.Empty;

        public string Comment
        {
            get { return _Comment; }
            set { _Comment = value; }
        }
        private double NoteTime;
        private string _NoteTimePre;

        public string NoteTimePre
        {
            get { return _NoteTimePre; }
            set
            {
                _NoteTimePre = ConvertInputValue(value.ToString());
                NotifyPropertyChanged(() => NoteTimePre);
            }
        }

        public string[] SelectedRefs { get; } = [ "Reinigen", "Cip", "Anlernen" ];
        private string? _SelectedVrgPath;

        public string? SelectedVrgPath
        {
            get { return _SelectedVrgPath; }
            set { _SelectedVrgPath = value; }
        }
        private UserItem _SelectedUser;

        public UserItem SelectedUser
        {
            get { return _SelectedUser; }
            set
            {
                if (_SelectedUser != value)
                {
                    _SelectedUser = value;
                    NotifyPropertyChanged(() => SelectedUser);
                    EmployeeNotesView.Refresh();
                }
            }
        }

        private ObservableCollection<EmployeeNote> EmployeeNotes;
        public ICollectionView EmployeeNotesView { get; private set; }
        public List<string> CalendarWeeks { get; private set; }
        public ICommand SubmitCommand { get; private set; }
        private int _CalendarWeek;

        public int CalendarWeek
        {
            get { return _CalendarWeek; }
            set
            {
                if (_CalendarWeek != value)
                {
                    _CalendarWeek = value;
                    SelectedDate = Get_DateFromKW();
                    
                }
            }
        }

        public List<UserItem> Users { get; private set; }
        private DayOfWeek _SelectedWeekDay;

        public DayOfWeek SelectedWeekDay
        {
            get { return _SelectedWeekDay; }
            set
            {
                _SelectedWeekDay = value;
                SelectedDate = Get_DateFromKW();
            }
        }
        private double _SumTimes;

        public double SumTimes
        {
            get { return _SumTimes; }
            set
            {
                _SumTimes = value;
                NotifyPropertyChanged(() => SumTimes);
            }
        }

        private void LoadingData()
        {
            VorgangRef = [.. _ctx.Vorgangs.AsNoTracking()
                .Include(x => x.AidNavigation)
                .Include(x => x.AidNavigation.MaterialNavigation)
                .Include(x => x.AidNavigation.DummyMatNavigation)
                .Where(x => x.AidNavigation.Abgeschlossen)
                .OrderBy(x => x.Aid)
                .ThenBy(x => x.Vnr)
                .Select(s => new VorgItem(s.Aid, s.Vnr.ToString(), s.Text.Trim(),
                s.AidNavigation.Material.Trim(), s.AidNavigation.MaterialNavigation.Bezeichng.Trim()))];

            EmployeeNotes = _ctx.EmployeeNotes.Where(x => x.AccId.Equals(UserInfo.User.UserId)).OrderBy(x => x.Date)
                .ToObservableCollection<EmployeeNote>();
            EmployeeNotesView = CollectionViewSource.GetDefaultView(EmployeeNotes);
            
            CalendarWeeks = GetKW_List();
 
            Users = [];
            foreach (var cost in UserInfo.User.AccountCostUnits)
            {
                var us = _ctx.IdmAccounts.AsNoTracking().Where(x => x.AccountCosts.Any(y => y.CostId == cost.CostId))
                    .Select(u => new UserItem(u.AccountId, u.Firstname, u.Lastname)).ToList();
                foreach (var account in us)
                {
                    if (Users.All(x => x.User != account.User))
                        Users.Add(account);
                }
                SelectedUser = Users.FirstOrDefault(x => x.User == UserInfo.User.UserId);
                EmployeeNotesView.Filter += FilterPredicate;
                EmployeeNotesView.CollectionChanged += CollectionHasChanged;
            }
 
            SelectedWeekDay = DateTime.Today.DayOfWeek;
        }

        private void CollectionHasChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var o in e.OldItems)
                {
                    _ctx.EmployeeNotes.Remove((EmployeeNote)o);
                }
            }
            SumTimes = EmployeeNotesView.Cast<EmployeeNote>().Sum(x => (double?)x.Processingtime ?? 0);
        }

        private bool FilterPredicate(object obj)
        {
            if(obj is EmployeeNote note)
            {
                int cw = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Today.AddDays(CalendarWeek*7), CalendarWeekRule.FirstFourDayWeek,
                    DayOfWeek.Sunday);
                return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(note.Date, CalendarWeekRule.FirstFourDayWeek,
                    DayOfWeek.Sunday) == cw
                    && note.AccId == SelectedUser.User;
            }
            return false;
        }
        private bool OnSubmitCanExecute(object arg)
        {
            return true;
        }

        private void OnSubmitExecuted(object obj)
        {
            var emp = new EmployeeNote();
            emp.AccId = SelectedUser.User;
            emp.Reference = ReferencePre ?? string.Empty;
            emp.Comment = Comment;
            emp.Date = SelectedDate;
            emp.Processingtime = NoteTime;
            _ctx.EmployeeNotes.Add(emp);

            _ctx.SaveChanges();
            EmployeeNotes.Add(emp);
        }

        private List<string> GetKW_List()
        {
            List<string> ret = [];
            var date = DateTime.Today.AddDays(-7*(3-1));

            for (int i = 0; i < 3; i++)
            {
                ret.Add(string.Format("KW {0}", CultureInfo.CurrentCulture.Calendar
                    .GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday)));
                date = date.AddDays(7);
            }
            return ret;
        }
        private DateTime Get_DateFromKW()
        {
            var d = DateTime.Today.AddDays(CalendarWeek*7);
            d = d.AddDays((int)SelectedWeekDay - (int)d.DayOfWeek);
            return d;
        }
        private string ConvertInputValue(string input)
        {
            int hour = 0 , minute = 0;
            bool error = false;
            input = input.Trim();
            Regex reg = new Regex(@"(\d+):(\d+)");
            Match test = reg.Match(input);
            if (double.TryParse(input, out double t))
            {
                NoteTime = t;
                hour = (int)t;
                var m = t - Math.Truncate(t);
                m = Math.Round(m, 2)*60;
                minute = (int)m;
            }
            else if(test.Success)
            {
                if(test.Groups.Count == 3)
                {
                    hour = int.Parse(test.Groups[1].Value);
                    minute = int.Parse((test.Groups[2].Value));
                }
            }
            else
            {
                reg = new Regex(@"^(\d+)(\s*[a-zA-Z]+)?");
                test = reg.Match(input);
                
                for(int i = 0; i<2; i++)
                {
                    if (test.Success)
                    {
                        
                        if (test.Groups.Count == 3)
                        {
                            var sec = test.Groups[2].Value.Trim();

                            if (sec.StartsWith("s", StringComparison.CurrentCultureIgnoreCase))
                            {
                                hour += int.Parse(test.Groups[1].Value);
                            }
                            if (sec.StartsWith("m", StringComparison.CurrentCultureIgnoreCase))
                            {
                                minute += int.Parse(test.Groups[1].Value);
                            }
                        }
                    }
                    else { error = true; break; }
                    if (test.Value.Length < input.Length) { test = reg.Match(input[test.Value.Length..]); } else break;
                }
            }
            if (!error)
            {
                NoteTime = hour + minute / 60;
                return string.Format("{0}:{1}", hour.ToString(), minute.ToString("D2"));           
            }
            return input;
        }
        public void Closing()
        {
            var c = _ctx.ChangeTracker.HasChanges();         
            _ctx.SaveChanges();
        }
    }
    public class VorgItem(string Auftrag, string Vorgang, string? Kurztext, string? Material, string? Bezeichnung)
    {
        public string Auftrag { get; } = Auftrag;
        public string Vorgang { get; } = Vorgang;
        public string? Kurztext { get; } = Kurztext;
        public string? Material { get; } = Material;
        public string? Bezeichnung { get; } = Bezeichnung;
    }
    public class UserItem(string User, string Vorname, string Nachname)
    {
        public string User { get; } = User;
        public string Vorname { get; } = Vorname;
        public string Nachname { get; } = Nachname;
    }
}
