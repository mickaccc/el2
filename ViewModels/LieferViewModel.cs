
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
using System.ServiceModel.Channels;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using Remotion.Linq.Collections;

namespace Lieferliste_WPF.ViewModels
{
    class LieferViewModel : Base.ViewModelBase, IDisposable
    {

        public String? Title { get; private set; }
        public String? Material { get; private set; }
        public String? MaterialDescription { get; private set; }
        public int? Quantity { get; private set; }
        public String? sysStatus { get; private set; }
        private List<TblDummy> _dummys = new();
        private IDialogProvider DialogProvider { get; set; }
        private static IEnumerable _orders;
        public IEnumerable Orders { get; private set; }
        private static List<TblVorgang> _processes => new();
        private readonly DataContext _db = new();
        public LieferViewModel()
        { Initialize(); }
        private void Initialize()
        {

            try
            {
                _orders = _db.TblAuftrags
                .Include(m => m.MaterialNavigation)
                .Include(d => d.DummyMatNavigation)
                .Include(v => v.TblVorgangs.Where(x => x.Aktuell))
                    .ToObservableCollection();

                Orders = _orders;
            }
            catch (DataException e)
            {
                DialogProvider.ErrorMessage("ERROR on load Data: " + e.ToString());
            }
            catch(SqlNullValueException e)
            {
                DialogProvider.ErrorMessage("ERROR on load Data: " + e.ToString());
            }

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
                //var ord = (from o in _db.TblAuftrags
                //           join mat in _db.TblMaterials on o.Material equals mat.Ttnr
                //           join v in _db.TblVorgangs on o.Aid equals v.Aid
                //           select new {o, mat, v})
                //        .GroupBy(x => x.o.Aid);

                //List<TblVorgang> proc= new List<TblVorgang>();
                //if (ord.Any())
                //{
                    
                //    foreach (var entry in ord)
                //    {


                        
                //        _orders.Add(entry.First(x => x.o) as TblAuftrag);
                        

                //        //var ordpro = _db.TblVorgangs.Where(x => x.Aid == entry.o.Aid);
                //        //_processes.AddRange(ordpro);

                //    }
                //}
            }
            catch (DataException e)
            {

                DialogProvider.ErrorMessage("ERROR on load Data: " + e.ToString());
            }
            catch(SqlNullValueException e)
            {
                DialogProvider.ErrorMessage("ERROR on load Data: " + e.ToString());
            }

        }

       
        public IEnumerable Get_Orders()
        {

                var orde = _db.TblAuftrags
                .Include(m => m.MaterialNavigation)
                .Include(d => d.DummyMatNavigation)
                .Include(v => v.TblVorgangs)
                .Where(x => x.Aid == "2100786672")
                    .ToObservableCollection();

            return orde;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
