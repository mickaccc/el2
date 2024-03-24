using El2Core.Models;
using Lieferliste_WPF.Utilities;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Lieferliste_WPF.ViewModels
{
    internal class HolidayEditViewModel
    {
        public HolidayEditViewModel(IContainerExtension container) 
        {
            _container = container;
            LoadData();
        }
        public ObservableCollection<HolidayRule> FixHolidays { get; set; } = [];
        public ObservableCollection<HolidayRule> VarHolidays { get; set; } = [];
        public ObservableCollection<Holiday> CloseHolidays { get; set; } = [];
        IContainerExtension _container;
        void LoadData()
        {
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var holi = db.Rules.First(x => x.RuleValue == "Holi");
            var xml = XmlSerializerHelper.GetSerializer(typeof(CloseAndHolidayRule));
            //var xml = DeserializeXmlData<CloseAndHolidayRule>(holi.RuleData);
            TextReader reader = new StringReader(holi.RuleData);
            CloseAndHolidayRule result;
            result = (CloseAndHolidayRule)xml.Deserialize(reader);

                FixHolidays.AddRange(result.FixHoliday);
                VarHolidays.AddRange(result.VariousHoliday);
                if(result.CloseDay != null) CloseHolidays.AddRange(result.CloseDay);
            
        }
        public static T DeserializeXmlData<T>(string xmlData)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using StringReader reader = new StringReader(xmlData);
            return (T)serializer.Deserialize(reader)!;
        }
    }
}
