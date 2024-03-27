﻿using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Lieferliste_WPF.Utilities;
using Microsoft.IdentityModel.Tokens;
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
            LoadData();
            SaveHolidayCommand = new ActionCommand(onSaveExecuted, onSaveCanExecute);
            FixHolidays.CollectionChanged += FixHolidayChanged;
            VarHolidays.CollectionChanged += VarHolidaysChanged;
            CloseHolidays.CollectionChanged += CloseHolidayChanged;
        }

        private bool onSaveCanExecute(object arg)
        {         
            return _isChanged;
        }

        private void onSaveExecuted(object obj)
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
        public ICommand SaveHolidayCommand { get; private set; }
        private bool _isChanged;
        void LoadData()
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var holi = db.Rules.First(x => x.RuleValue == "Holi");
            var xml = XmlSerializerHelper.GetSerializer(typeof(CloseAndHolidayRule));

            TextReader reader = new StringReader(holi.RuleData);
            CloseAndHolidayRule result;
            result = (CloseAndHolidayRule)xml.Deserialize(reader);

                FixHolidays.AddRange(result.FixHoliday);
                VarHolidays.AddRange(result.VariousHoliday);
                CloseHolidays.AddRange(result.CloseDay);
            foreach(var c in CloseHolidays)
            {
                c.PropertyChanged += onPropertyChanged;
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
