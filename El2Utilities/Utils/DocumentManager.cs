using El2Core.Models;
using Newtonsoft.Json.Linq;
using Prism.Ioc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace El2Core.Utils
{
    public class DocumentManager(IContainerExtension container)
    {
        
        private DocumentBuilder? builder;
        public void Construct(DocumentBuilder Docubuilder, string TTNR, string OrderNr)
        {
            builder = Docubuilder;
            builder.Build(container, TTNR, OrderNr);
        }
        public void SaveDocumentData(string rootPath, string template, string RegEx)
        {
            builder?.SaveDocumentData(rootPath, template, RegEx);
        }
        public void FinalzeDocument()
        {
            builder?.Document.FinalizeDocu();
        }
    }
    public abstract class DocumentBuilder(DocumentType documentType)
    {
        public IContainerExtension container;
        public Document Document { get; private set; } = new Document(documentType);
        public abstract void Build(IContainerExtension container, string ttnr, string OrderNr);
        public abstract FileInfo GetDataSheet();
        public abstract void SaveDocumentData();
        public abstract void SaveDocumentData(string rootPath, string template, string RegEx);
    }

    public class Document(DocumentType documentType)
    {
        private readonly Dictionary<DocumentPart, string> parts =[];
        private readonly DocumentType documentType = documentType;
        public string this[DocumentPart key]
        {
            get => parts[key];
            set => parts[key] = value;
        }
        public IEnumerator enumerator => parts.GetEnumerator();
        public int Count => parts.Count;
        public void FinalizeDocu() { }
    }
    public class MeasureFirstPartBuilder : DocumentBuilder
    {
        
        public MeasureFirstPartBuilder() : base(DocumentType.MeasureFirstPart)
        { }

        public override void Build(IContainerExtension container, string ttnr, string orderNr)
        {
            base.container = container;
            using var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var rule = db.Rules.SingleOrDefault(x => x.RuleValue == "M1");
            if (rule != null)
            {
                var xml = XmlSerializerHelper.GetSerializer(typeof(List<Entry>));

                TextReader reader = new StringReader(rule.RuleData);
                List<Entry> doc = (List<Entry>)xml.Deserialize(reader);
                foreach (var entry in doc)
                {
                    DocumentPart DokuPart = (DocumentPart)Enum.Parse(typeof(DocumentPart), (string)entry.Key);
                    Document[DokuPart] = (string)entry.Value;
                }
            }
        }
        public override FileInfo GetDataSheet()
        {
            throw new NotImplementedException();
        }

        public override void SaveDocumentData()
        {
            throw new NotImplementedException();
        }

        public override void SaveDocumentData(string rootPath, string template, string RegEx)
        {
            var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var rule = db.Rules.SingleOrDefault(x => x.RuleValue == "M1");
            //var xml = XmlSerializerHelper.GetSerializer(typeof(Dictionary<string, string>));

            if(rule == null) { rule = new(); rule.RuleName = "FirstPart"; rule.RuleValue = "M1"; db.Rules.Add(rule); }
            var dict = new Dictionary<string, string>();
            dict.Add(DocumentPart.RootPath.ToString(), rootPath);
            dict.Add(DocumentPart.Template.ToString(), template);
            dict.Add(DocumentPart.RegularEx.ToString(), RegEx);
            //Document[DocumentPart.RootPath] = rootPath;
            //Document[DocumentPart.Template] = template;
            //Document[DocumentPart.RegularEx] = RegEx;

            StringWriter sw = new StringWriter();
            Serialize(sw, Document);
            rule.RuleData = sw.ToString();
            
            db.SaveChanges();
        }
        public static void Serialize(TextWriter writer, Document dictionary)
        {
            List<Entry> entries = new List<Entry>(dictionary.Count);
            dictionary.enumerator.MoveNext();
            do
            {
                var curr = dictionary.enumerator.Current;
                
                entries.Add(new Entry(DocumentPart.Order, dictionary.enumerator.Current));
            }while(dictionary.enumerator.MoveNext());
            var serializer = XmlSerializerHelper.GetSerializer(typeof(List<Entry>));
            serializer.Serialize(writer, entries);
        }
        public static void Deserialize(TextReader reader, IDictionary dictionary)
        {
            dictionary.Clear();
            XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
            List<Entry> list = (List<Entry>)serializer.Deserialize(reader);
            foreach (Entry entry in list)
            {
                dictionary[entry.Key] = entry.Value;
            }
        }
    }

    public class Entry
    {
        public object Key;
        public object Value;
        public Entry()
        {
        }

        public Entry(object key, object value)
        {
            Key = key;
            Value = value;
        }
    }
    public enum DocumentPart
    {
        RootPath,
        Template,
        RegularEx,
        SavePath,
        TTNR,
        Order
    }
    public enum DocumentType
    {
        MeasureFirstPart,
        MeasureVMPB
    }

}
