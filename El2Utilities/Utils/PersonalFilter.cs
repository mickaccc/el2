﻿using El2Core.Models;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.Serialization;

namespace El2Core.Utils
{
    public interface IPersonalFilterContainer
    {
        IContainerProvider container { get; }
    }
    public sealed class PersonalFilterContainer
    {

        Dictionary<string, PersonalFilter> _filters = [];
        private static readonly PersonalFilterContainer Instance = new ();
        private PersonalFilterContainer()
        {
            Load ();
        }

        public PersonalFilter this[string key]
        {
            get => _filters[key];
            set => _filters[key] = value;
        }
        public string[] Keys => [.. _filters.Keys];

        public IContainerProvider container;

        public static PersonalFilterContainer GetInstance()
        {
            return Instance;
        }
        public bool IsChanged { get; private set; } = false;
        private void Load()
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);

            var companyName = versionInfo.CompanyName.Replace('/', '_');
            var env = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            FileInfo fileInfo = new FileInfo(Path.Combine(env.ToString(), companyName, "Perfilter.xml"));
            if (fileInfo.Exists)
            {

                try
                {
                    DeserializeObject(fileInfo.FullName);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public void Reload()
        {
            _filters.Clear();
            Load();
        }
        public void Remove(string key)
        {
            _filters.Remove(key);
            IsChanged = true;
        }
        public void Add(string name, PersonalFilter filter)
        {
            _filters.Add(name, filter);
            IsChanged = true;
        }
        public void Save()
        {
            if (IsChanged)
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);

                var companyName = versionInfo.CompanyName.Replace('/', '_');
                var env = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string fileName = Path.Combine(env.ToString(), companyName, "Perfilter.xml");

                SerializeObject(fileName);
                IsChanged = false;
                Reload();
            }
        }

        public void SerializeObject(string filename)
        {
            // Each overridden field, property, or type requires
            // an XmlAttributes instance.  
            XmlAttributes attrs = new XmlAttributes(); 
            XmlElementAttribute attr = new XmlElementAttribute();
            attr.ElementName = "Filter";
            attr.Type = typeof(PersonalFilterVorgang);

            // Adds the element to the collection of elements.  
            attrs.XmlElements.Add(attr);
            attr.Type = typeof(PersonalFilterOrderRb);

            // Adds the element to the collection of elements.  
            attrs.XmlElements.Add(attr);
            // Creates the XmlAttributeOverrides instance.  
            XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();

            // Adds the type of the class that contains the overridden
            // member, as well as the XmlAttributes instance to override it
            // with, to the XmlAttributeOverrides.  
            attrOverrides.Add(typeof(PersonalFilter), "Filters", attrs);

            // Creates the XmlSerializer using the XmlAttributeOverrides.  
            XmlSerializer s =
            new XmlSerializer(typeof(List<PersonalFilter>), attrOverrides);

            // Writing the file requires a TextWriter instance.  
            TextWriter writer = new StreamWriter(filename);

            // Creates the object to be serialized.  
            List<PersonalFilter> filters = new List<PersonalFilter>();
            foreach (var filter in _filters)
            {
                filters.Add(filter.Value);
            }

            // Serializes the object.  
            s.Serialize(writer, filters);
            writer.Close();
        }

        public void DeserializeObject(string filename)
        {
            XmlAttributeOverrides attrOverrides =
                new XmlAttributeOverrides();
            XmlAttributes attrs = new XmlAttributes();

            XmlElementAttribute attr = new XmlElementAttribute();
            attr.ElementName = "Filter";
            attr.Type = typeof(PersonalFilterVorgang);

            // Adds the XmlElementAttribute to the collection of objects.  
            attrs.XmlElements.Add(attr);
            attr.Type = typeof(PersonalFilterOrderRb);

            // Adds the element to the collection of elements.  
            attrs.XmlElements.Add(attr);
            attrOverrides.Add(typeof(PersonalFilter), "Filters", attrs);

            // Creates the XmlSerializer using the XmlAttributeOverrides.  
            XmlSerializer s =
            new XmlSerializer(typeof(PersonalFilter[]), attrOverrides);

            FileStream fs = new FileStream(filename, FileMode.Open);
            var filters = (PersonalFilter[])s.Deserialize(fs);

            foreach(var filter in filters)
            {
                _filters.Add(filter.Name, filter);
            }
        }
    }

    [XmlInclude(typeof(PersonalFilterVorgang))]
    [XmlInclude(typeof (PersonalFilterOrderRb))]
    [XmlInclude(typeof(PersonalFilterMaterial))]
    [XmlInclude(typeof(PersonalFilterRessource))]
    [XmlInclude(typeof(PersonalFilterProject))]
    [Serializable]
    public abstract class PersonalFilter
    {
        public abstract string Name {get; set;}
        public abstract string Pattern { get; set; }
        public abstract (string, string, int) Field { get; set; }
        public abstract Regex GetRegEx();

        public abstract string GetTestString(Vorgang vorgang, IContainerProvider container);


        public bool TestValue(Vorgang vorgang, IContainerProvider container)
        {
            var Reg = GetRegEx();
            var test = GetTestString(vorgang, container);
            return Reg.Match(test).Success;
        }
    }
    [Serializable]
    public class PersonalFilterVorgang : PersonalFilter
    {
        public PersonalFilterVorgang() { }
        public PersonalFilterVorgang(string name, string regex, (string, string, int) field) { Name = name; RegEx = new Regex(regex); _Field = field; }
        private Regex RegEx;
        public override string Name { get; set; }
        private (string, string, int) _Field;
        public override (string, string, int) Field
        {
            get { return _Field; }
            set { _Field = value; }
        }
        public override string Pattern
        {
            get { return RegEx.ToString(); }
            set { RegEx = new Regex(value);}
        }

   
        public override Regex GetRegEx()
        {
            return RegEx;
        }

        public override string GetTestString(Vorgang vorgang, IContainerProvider container)
        {
            PropertyInfo? info = vorgang.GetType().GetProperty(Field.Item2);
            if (info != null)
                return info.GetValue(vorgang, null)?.ToString() ?? string.Empty;
            return string.Empty;
        }
    }
    [Serializable]
    public class PersonalFilterOrderRb : PersonalFilter
    {
        public override string Name { get; set; }
        private Regex RegEx;
        private (string, string, int) _Field;
        public PersonalFilterOrderRb() { }
        public PersonalFilterOrderRb(string name, string regex, (string, string, int) field)
        {
            Name = name;
            RegEx = new Regex(regex);
            _Field = field;
        }
        public override string Pattern
        {
            get { return RegEx.ToString(); }
            set { RegEx = new Regex(value); }
        }

        public override (string, string, int) Field
        {
            get { return _Field; }
            set { _Field = value; }
        }

        public override Regex GetRegEx()
        {
            return RegEx;
        }

        public override string GetTestString(Vorgang vorgang, IContainerProvider container)
        {

            using var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>(); 
 
            var modelData = db.Vorgangs.EntityType;

            var nav = modelData.FindDeclaredNavigation("AidNavigation");
            if (nav != null)
            {
                modelData = nav.TargetEntityType;
                var value = modelData.FindDeclaredProperty(Field.Item2).PropertyInfo.GetValue(vorgang.AidNavigation, null);
                return (value != null) ? value.ToString() : string.Empty;
            }
                     
            return string.Empty;
        }
    }
    [Serializable]
    public class PersonalFilterMaterial : PersonalFilter
    {
        public override string Name { get; set; }
        private Regex RegEx;
        private (string, string, int) _Field;
        public PersonalFilterMaterial() { }
        public PersonalFilterMaterial(string name, string regex, (string, string, int) field)
        {
            Name = name;
            RegEx = new Regex(regex);
            _Field = field;
        }
        public override string Pattern
        {
            get { return RegEx.ToString(); }
            set { RegEx = new Regex(value); }
        }

        public override (string, string, int) Field
        {
            get { return _Field; }
            set { _Field = value; }
        }

        public override Regex GetRegEx()
        {
            return RegEx;
        }

        public override string GetTestString(Vorgang vorgang, IContainerProvider container)
        {

            using var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();

            var modelData = db.Vorgangs.EntityType;

            var nav = modelData.FindDeclaredNavigation("AidNavigation");
            modelData = nav.TargetEntityType;
            nav = modelData.FindDeclaredNavigation("MaterialNavigation");
            if (nav != null)
            {

                modelData = nav.TargetEntityType;
                if (vorgang.AidNavigation.MaterialNavigation != null)
                {
                    var value = modelData.FindDeclaredProperty(Field.Item2).PropertyInfo.GetValue(vorgang.AidNavigation.MaterialNavigation, null);
                    return (value != null) ? value.ToString() : string.Empty;
                }
            }

            return string.Empty;
        }
    }
    [Serializable]
    public class PersonalFilterRessource : PersonalFilter
    {
        public override string Name { get; set; }
        private Regex RegEx;
        private string Navigation;
        private string Property;
        private (string, string, int) _Field;
        public PersonalFilterRessource() { }
        public PersonalFilterRessource(string name, string regex, (string, string, int) field)
        {
            Name = name;
            RegEx = new Regex(regex);
            _Field = field;
        }
        public override string Pattern
        {
            get { return RegEx.ToString(); }
            set { RegEx = new Regex(value); }
        }

        public override (string, string, int) Field
        {
            get { return _Field; }
            set { _Field = value; }
        }

        public override Regex GetRegEx()
        {
            return RegEx;
        }

        public override string GetTestString(Vorgang vorgang, IContainerProvider container)
        {

            using var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();

            var modelData = db.Vorgangs.EntityType;

            var nav = modelData.FindDeclaredNavigation("RidNavigation");
            if (nav != null)
            {
                modelData = nav.TargetEntityType;
                if (vorgang.RidNavigation != null)
                {
                    var value = modelData.FindDeclaredProperty(Field.Item2).PropertyInfo.GetValue(vorgang.RidNavigation, null);
                    return (value != null) ? value.ToString() : string.Empty;
                }
            }

            return string.Empty;
        }
    }
    [Serializable]
    public class PersonalFilterProject : PersonalFilter
    {
        public override string Name { get; set; }
        private Regex RegEx;
        private string Navigation;
        private string Property;
        private (string, string, int) _Field;
        public PersonalFilterProject() { }
        public PersonalFilterProject(string name, string regex, (string, string, int) field)
        {
            Name = name;
            RegEx = new Regex(regex);
            _Field = field;
        }
        public override string Pattern
        {
            get { return RegEx.ToString(); }
            set { RegEx = new Regex(value); }
        }

        public override (string, string, int) Field
        {
            get { return _Field; }
            set { _Field = value; }
        }

        public override Regex GetRegEx()
        {
            return RegEx;
        }

        public override string GetTestString(Vorgang vorgang, IContainerProvider container)
        {

            using var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();

            var modelData = db.Vorgangs.EntityType;

            var nav = modelData.FindDeclaredNavigation("AidNavigation");
            modelData = nav.TargetEntityType;
            nav = modelData.FindDeclaredNavigation("Pro");
            if (nav != null)
            {
                modelData = nav.TargetEntityType;
                if (vorgang.AidNavigation.Pro != null)
                {
                    var value = modelData.FindDeclaredProperty(Field.Item2).PropertyInfo.GetValue(vorgang.AidNavigation.Pro, null);
                    return (value != null) ? value.ToString() : string.Empty;
                }
            }

            return string.Empty;
        }
    }
    public readonly struct PropertyPair
    {
        //Type 1 == Vorgang
        //Type 2 == OrderRb
        //Type 3 == Material
        //Type 4 == Ressource
        //Type 5 == Project
        public static ValueTuple<string, string, int> OrderNumber = ValueTuple.Create("Auftragsnummer", "Aid", 1);
        public static ValueTuple<string, string, int> ProcessDescription = ValueTuple.Create("KurzText", "Text", 1);
        public static ValueTuple<string, string, int> Material = ValueTuple.Create("Material", "Material", 2);
        public static ValueTuple<string, string, int> MaterialDescription = ValueTuple.Create("MaterialBezeichnung", "Bezeichng", 3);
        public static ValueTuple<string, string, int> RessourceName = ValueTuple.Create("Maschinenname", "RessName", 4);
        public static ValueTuple<string, string, int> LieferTermin = ValueTuple.Create("Liefertermin", "Liefertermin", 2);
        public static ValueTuple<string, string, int> PrioText = ValueTuple.Create("PrioText", "Prio", 2);
        public static ValueTuple<string, string, int> Project = ValueTuple.Create("Projekt", "ProId", 2);
        public static ValueTuple<string, string, int> ProjectInfo = ValueTuple.Create("Projekt Info", "RessName", 5);

    }

}
