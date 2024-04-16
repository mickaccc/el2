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
        public void Construct(DocumentBuilder Docubuilder, string TTNR)
        {
            builder = Docubuilder;
            builder.Build(container, TTNR);
        }
        public void SaveDocumentData(string rootPath, string template, string RegEx)
        {
            builder?.SaveDocumentData(rootPath, template, RegEx);
        }
        public string Collect()
        {
            return builder?.Collect();
        }
        public string Collect(string target)
        {
            return builder?.Collect(target);
        }
        public DocumentBuilder? GetBuilder()
        {
            return builder;
        }
    }
    public abstract class DocumentBuilder(DocumentType documentType)
    {
        public IContainerExtension container;
        public Document Document { get; private set; } = new Document(documentType);
        public abstract void Build(IContainerExtension container, string ttnr);
        public abstract FileInfo GetDataSheet();
        public abstract void SaveDocumentData();
        public abstract void SaveDocumentData(string rootPath, string template, string RegEx);
        public abstract string Collect();
        public abstract string Collect(string target);
        public abstract DocumentBuilder GetBuilder();
 
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
        public int Count => parts.Count;
    }
    public class MeasureFirstPartBuilder : DocumentBuilder
    {
        public CompositeNode<Shape> Root { get; private set; }
        public MeasureFirstPartBuilder() : base(DocumentType.MeasureFirstPart)
        { }
        public override void Build(IContainerExtension container, string ttnr)
        {
            base.container = container;
            if (RuleInfo.Rules.Keys.Contains("FirstPart") == false) return;
            var xml = XmlSerializerHelper.GetSerializer(typeof(List<Entry>));

            TextReader reader = new StringReader(RuleInfo.Rules["FirstPart"].RuleData);
            List<Entry> doc = (List<Entry>)xml.Deserialize(reader);
            foreach (var entry in doc)
            {
                DocumentPart DokuPart = (DocumentPart)Enum.Parse(typeof(DocumentPart), entry.Key.ToString());
                Document[DokuPart] = (string)entry.Value;
            }
            Root = new CompositeNode<Shape> { Node = new Shape(Document[DocumentPart.RootPath]) };
            Document[DocumentPart.TTNR] = ttnr;
            Regex regex = new Regex(Document[DocumentPart.RegularEx]);
            Match match2 = regex.Match(ttnr);
            StringBuilder nsb = new StringBuilder();
            foreach (Group ma in match2.Groups.Values.Skip(1))
            {
                if (ma.Value != ttnr)
                {
                    nsb.Append(ma.Value).Append(Path.DirectorySeparatorChar);
                    Root.Add(new Shape(ma.Value));
                }
            }
            Document[DocumentPart.SavePath] = Path.Combine(Document[DocumentPart.RootPath], nsb.ToString());
            FileInfo f = new(Document[DocumentPart.Template]);
            Document[DocumentPart.File] = Path.Combine(Document[DocumentPart.SavePath], f.Name.Replace("Messblatt", ttnr));
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
            Document[DocumentPart.RootPath] = rootPath;
            Document[DocumentPart.Template] = template;
            Document[DocumentPart.RegularEx] = RegEx;

            StringWriter sw = new StringWriter();
            Serialize(sw, Document);
            rule.RuleData = sw.ToString();
            
            db.SaveChanges();
        }
        private static void Serialize(TextWriter writer, Document dictionary)
        {
            List<Entry> entries = new List<Entry>();
    
                
            entries.Add(new Entry(DocumentPart.RootPath, dictionary[DocumentPart.RootPath]));
            entries.Add(new Entry(DocumentPart.Template, dictionary[DocumentPart.Template]));
            entries.Add(new Entry(DocumentPart.RegularEx, dictionary[DocumentPart.RegularEx]));

            var serializer = XmlSerializerHelper.GetSerializer(typeof(List<Entry>));
            serializer.Serialize(writer, entries);
        }
        private static void Deserialize(TextReader reader, IDictionary dictionary)
        {
            dictionary.Clear();
            XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
            List<Entry> list = (List<Entry>)serializer.Deserialize(reader);
            foreach (Entry entry in list)
            {
                dictionary[entry.Key] = entry.Value;
            }
        }

        public override string Collect()
        {
            string path = Root.Node.ToString();
            if (!Directory.Exists(path)) return string.Empty;
            List<CompositeNode<Shape>>? listdir = Root.Children;

            for (int i = 0; i < listdir?.Count; i++)
            {
                path = Path.Combine(path, listdir[i].Node.ToString());
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            }
            return path;
        }

        public override string Collect(string target)
        {
            return Path.Combine(Collect(), target);
        }

        public override DocumentBuilder GetBuilder()
        {
            return this;
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
        //file://///bosch.com/DfsRB/DfsAT/Loc/Hl/Abt/Technical_Functions/420_Musterbau/200_Bereiche/250_Vormuster/COS_Messraum/Vorlagen/Vorlagen_VMPB
        MeasureFirstPart,
        MeasureVMPB
    }

    /// <summary>
    /// Generic tree node class
    /// </summary>
    /// <typeparam name="T">Node type</typeparam>
    public class CompositeNode<T> where T : IComparable<T>
    {
        // Add a child tree node
        public CompositeNode<T> Add(T child)
        {
            var newNode = new CompositeNode<T> { Node = child };
            Children.Add(newNode);
            return newNode;
        }
        // Remove a child tree node
        public void Remove(T child)
        {
            foreach (var compositeNode in Children)
            {
                if (compositeNode.Node.CompareTo(child) == 0)
                {
                    Children.Remove(compositeNode);
                    return;
                }
            }
        }
        // Gets or sets the node
        public T Node { get; set; } = default!;
        // Gets treenode children
        public List<CompositeNode<T>> Children { get; } = [];
        // Recursively displays node and its children 
        public static void Display(CompositeNode<T> node, int indentation)
        {
            var line = new string('-', indentation);
            //WriteLine(line + " " + node.Node);
            node.Children.ForEach(n => Display(n, indentation + 1));
        }
    }
    /// <summary>
    /// Shape class
    /// <remarks>
    /// Implements generic IComparable interface
    /// </remarks>
    /// </summary>
    public class Shape(string name) : IComparable<Shape>
    {
        public override string ToString() => name;
        // IComparable<Shape> Member
        public int CompareTo(Shape? other) => (this == other) ? 0 : -1;
    }

}
