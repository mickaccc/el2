
using Lieferliste_WPF.ViewModels.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using Lieferliste_WPF.Data;
using Lieferliste_WPF.Data.Models;
using System.Security.Cryptography;
using System.Collections;
using Lieferliste_WPF.Utilities;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Lieferliste_WPF.ViewModels
{
    class OrderViewModel : IDisposable
    {
        //DB_COS_LIEFERLISTE_SQLEntities ctx = new DB_COS_LIEFERLISTE_SQLEntities();
        //public List<OrderList_Result> Processes { get; set; }
        private static OrderViewModel _this = new();
        public String? Title { get; private set; }
        public String? Material { get; private set; }
        public String? MaterialDescription { get; private set; }
        public int? Quantity { get; private set; }
        public String? sysStatus { get; private set; }
        private List<TblDummy> _dummys = new();
        private static ObservableCollection<TblAuftrag> _orders => new();
        private static List<TblVorgang> _processes => new();
        private DataContext _db = new();
        private OrderViewModel()
        { Initialize(); }
        private void Initialize()
        {

            LoadData();

        }

        private ObservableCollection<dynamic> IEnumeratorToObservableCollection(IEnumerable source)
        {

            ObservableCollection<dynamic> SourceCollection = new ObservableCollection<dynamic>();

            IEnumerator enumItem = source.GetEnumerator();
            var gType = source.GetType();
            string collectionFullName = gType.FullName;
            Type[] genericTypes = gType.GetGenericArguments();
            string className = genericTypes[0].Name;
            string classFullName = genericTypes[0].FullName;
            string assName = (classFullName.Split('.'))[0];

            // Get the type contained in the name string
            Type type = Type.GetType(classFullName, true);

            // create an instance of that type
            object instance = Activator.CreateInstance(type);
            List<PropertyInfo> oProperty = instance.GetType().GetProperties().ToList();
            while (enumItem.MoveNext())
            {

                Object instanceInner = Activator.CreateInstance(type);
                var x = enumItem.Current;

                foreach (var item in oProperty)
                {
                    if (x.GetType().GetProperty(item.Name) != null)
                    {
                        var propertyValue = x.GetType().GetProperty(item.Name).GetValue(x, null);
                        if (propertyValue != null)
                        {
                            PropertyInfo prop = type.GetProperty(item.Name);
                            prop.SetValue(instanceInner, propertyValue, null);
                        }
                    }
                }

                SourceCollection.Add(instanceInner);
            }

            return SourceCollection;
        }

        public void LoadData()
        {

            try
            {
                var ord = (from o in _db.TblAuftrags
                           join mat in _db.TblMaterials on o.Material equals mat.Ttnr
                           where o.Abgeschlossen == false
                           select new { o, mat });
                List<TblVorgang> proc= new List<TblVorgang>();
                foreach (var entry in ord)
                {
                    
                    _orders.Add(entry.o);
                 
                    var ordpro = _db.TblVorgangs.Where(x => x.Aid == entry.o.Aid);
                    _processes.AddRange(ordpro);

                }
                       }
            catch (Exception)
            {

                throw;
            }

        }

        public static OrderViewModel This => _this;
        public static ObservableCollection<TblAuftrag> Get_Orders()
        {
            foreach(TblAuftrag o in _orders)
            {
                o.TblVorgangs = _processes.Where(x => x.Aid == o.Aid).ToList();
            }
            return _orders;
        }
        public void ReLoad(String OrderNumber)
        {

  
            //RaisePropertyChanged("Processes");
            //RaisePropertyChanged("Title");
            //RaisePropertyChanged("Material");
            //RaisePropertyChanged("MaterialDescription");
            //RaisePropertyChanged("Quantity");
            //RaisePropertyChanged("sysStatus");

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
