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
using System.Text.RegularExpressions;
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

        public IEnumerator enumerator => this.parts.GetEnumerator();
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
 
            var xml = XmlSerializerHelper.GetSerializer(typeof(List<Entry>));

            TextReader reader = new StringReader(RuleInfo.Rules["FirstPart"].RuleData);
            List<Entry> doc = (List<Entry>)xml.Deserialize(reader);
            foreach (var entry in doc)
            {
                DocumentPart DokuPart = (DocumentPart)Enum.Parse(typeof(DocumentPart), (string)entry.Key);
                Document[DokuPart] = (string)entry.Value;
            }
            Document[DocumentPart.Order] = orderNr;
            Document[DocumentPart.TTNR] = ttnr;
            Regex regex = new Regex(Document[DocumentPart.RegularEx]);
            Match match2 = regex.Match(ttnr);
            StringBuilder nsb = new StringBuilder();
            foreach (Group ma in match2.Groups.Values)
            {
                if (ma.Value != ttnr)
                    nsb.Append(ma.Value).Append(Path.DirectorySeparatorChar);
            }
            nsb.Append(orderNr);
            Document[DocumentPart.SavePath] = Path.Combine(Document[DocumentPart.RootPath], nsb.ToString());
            FileInfo f = new(Document[DocumentPart.Template]);
            Document[DocumentPart.File] = new StringBuilder(Document[DocumentPart.SavePath]).Append(Path.DirectorySeparatorChar).Append(f.Name).ToString();
        }
        public override FileInfo GetDataSheet()
        {
            return new FileInfo(Document[DocumentPart.File]);
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
            List<Entry> entries = new List<Entry>();
    
                
            entries.Add(new Entry(DocumentPart.RootPath, dictionary[DocumentPart.RootPath]));
            entries.Add(new Entry(DocumentPart.Template, dictionary[DocumentPart.Template]));
            entries.Add(new Entry(DocumentPart.RegularEx, dictionary[DocumentPart.RegularEx]));

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
        Order,
        File
    }
    public enum DocumentType
    {
        MeasureFirstPart,
        MeasureVMPB
    }

}
