using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Lieferliste_WPF.ViewModels
{
    internal class HolidayEditViewModel : ViewModelBase
    {
        public HolidayEditViewModel(IContainerExtension container)
        {
            _container = container;
            var factory = _container.Resolve<ILoggerFactory>();
            _Logger = factory.CreateLogger<HolidayEditViewModel>();
            LoadData();
            SaveHolidayCommand = new ActionCommand(onSaveExecuted, onSaveCanExecute);
            DeleteHolidayCommand = new ActionCommand(onDelExecuted, onDelCanExecute);
            FixHolidays.CollectionChanged += FixHolidayChanged;
            VarHolidays.CollectionChanged += VarHolidaysChanged;
            CloseHolidays.CollectionChanged += CloseHolidayChanged;
        }

        private bool onDelCanExecute(object arg)
        {
            return true;
        }

        private void onDelExecuted(object obj)
        {
            if (obj is Holiday holi)
            {
                CloseHolidays.Remove(holi);
                _isChanged = true;
            }
            else if (obj is HolidayRule rule)
            {
                if (rule.DayDistance != null)
                {
                    FixHolidays.Remove(rule);
                    _isChanged = true;
                }
                else
                {
                    VarHolidays.Remove(rule);
                    _isChanged = true;
                }
            }
        }

        private bool onSaveCanExecute(object arg)
        {
            return _isChanged;
        }

        private void onSaveExecuted(object obj)
        {
            try
            {
                using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
                var holi = db.Rules.First(x => x.RuleValue == "Holi");
                var xml = XmlSerializerHelper.GetSerializer(typeof(CloseAndHolidayRule));

                CloseAndHolidayRule data = new();
                data.FixHoliday.AddRange(FixHolidays);
                data.VariousHoliday.AddRange(VarHolidays);
                data.CloseDay.AddRange(CloseHolidays);
                StringWriter sw = new StringWriter();
                xml.Serialize(sw, data);
                holi.RuleData = sw.ToString();
                db.SaveChanges();
                _isChanged = false;
                _Logger.LogInformation("{message}", holi.RuleData);
            }
            catch (Exception e)
            {

                _Logger.LogError("{message}", e.ToString());
            }
        }


        private void CloseHolidayChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            _isChanged = e.Action == NotifyCollectionChangedAction.Add ||
                e.Action == NotifyCollectionChangedAction.Remove;
        }

        private void VarHolidaysChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            _isChanged = e.Action == NotifyCollectionChangedAction.Add ||
                e.Action == NotifyCollectionChangedAction.Remove;
        }

        private void FixHolidayChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            _isChanged = e.Action == NotifyCollectionChangedAction.Add ||
                 e.Action == NotifyCollectionChangedAction.Remove;
        }

        public string Title { get; } = "Feiertag Editor";
        public ObservableCollection<HolidayRule> FixHolidays { get; set; } = [];
        public ObservableCollection<HolidayRule> VarHolidays { get; set; } = [];
        public ObservableCollection<Holiday> CloseHolidays { get; set; } = [];
        IContainerExtension _container;
        ILogger _Logger;
        public ICommand SaveHolidayCommand { get; private set; }
        public ICommand DeleteHolidayCommand { get; private set; }
        private bool _isChanged;
        void LoadData()
        {
            try
            {
                var xml = XmlSerializerHelper.GetSerializer(typeof(CloseAndHolidayRule));

                TextReader reader = new StringReader(RuleInfo.Rules["Feiertage"].RuleData);
                CloseAndHolidayRule result;
                result = (CloseAndHolidayRule)xml.Deserialize(reader);

                FixHolidays.AddRange(result.FixHoliday);
                VarHolidays.AddRange(result.VariousHoliday);
                CloseHolidays.AddRange(result.CloseDay);
                foreach (var c in CloseHolidays)
                {
                    c.PropertyChanged += onPropertyChanged;
                }
                _Logger.LogInformation("Fix {message} Var {1} Close {2}", FixHolidays.Count, VarHolidays.Count, CloseHolidays.Count);
            }
            catch (Exception e)
            {
                _Logger.LogError("{message}", e.ToString());
            }

        }

        private void onPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _isChanged = true;
        }

        public static T DeserializeXmlData<T>(string xmlData)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using StringReader reader = new StringReader(xmlData);
            return (T)serializer.Deserialize(reader)!;
        }
    }
}
