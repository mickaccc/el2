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
using System.Windows.Forms;
using System.Xml.Serialization;

namespace El2Core.Utils
{
    public class DocumentManager(IContainerExtension container)
    {
        
        private DocumentBuilder? builder;
        public void Construct(DocumentBuilder Docubuilder, string[] TTNR)
        {
            builder = Docubuilder;
            builder.Build(container, TTNR);
        }
        public void SaveDocumentData(string rootPath, string[] template, string RegEx)
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
    public abstract class DocumentBuilder()
    {
        public IContainerExtension container;
        //public Document Document { get; private set; } = new Document();
        public abstract void Build(IContainerExtension container, string[] ttnr);
        public abstract FileInfo GetDataSheet();
        public abstract void SaveDocumentData();
        public abstract void SaveDocumentData(string rootPath, string[] template, string RegEx);
        public abstract string Collect();
        public abstract string Collect(string target);
        public abstract DocumentBuilder GetBuilder();
 
    }
    public interface IDocument
    {
    }
    public abstract class Document()
    {
        private readonly Dictionary<DocumentPart, string> parts =[];
        public string this[DocumentPart key]
        {
            get => parts[key];
            set => parts[key] = value;
        }
        public int Count => parts.Count;
        public HashSet<DocumentPart> Keys => parts.Keys.ToHashSet();
    }
    public class FirstPartDocument : Document { }
    public class VmpbDocument : Document { }
    public class WorkAreaDocument : Document { }
    public abstract class DocumentInfo(IContainerExtension container)
    {
        private IContainerExtension Container => container;
        public abstract Document CreateDocumentInfos();
        public abstract Document CreateDocumentInfos(string[] folders);
        public abstract Document GetDocument();
        public void SaveDocumentData(Document document)
        {
            using var db = Container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            Rule rule;
            if(db.Rules.All(x => x.RuleValue != document[DocumentPart.Type]))
            {
                rule = new Rule()
                { RuleValue = document[DocumentPart.Type], RuleName = document[DocumentPart.Type] };
                db.Rules.Add(rule);
            }
            else rule = db.Rules.First(x => x.RuleValue == document[DocumentPart.Type]);

            StringWriter sw = new StringWriter();
            Serialize(sw, document);
            rule.RuleData = sw.ToString();

            db.SaveChanges();
        }
        public void Collect(Document document)
        {
            if (!Directory.Exists(document[DocumentPart.RootPath])) return;
            string path = document[DocumentPart.RootPath];
            foreach(var s in document[DocumentPart.SavePath].Split(Path.DirectorySeparatorChar))
            {
                path = Path.Combine(path, s);
                if(!Directory.Exists(path)) Directory.CreateDirectory(path);
            }
        }
        private static void Serialize(TextWriter writer, Document dictionary)
        {

            List<Entry> entries = [];

            foreach(var k in dictionary.Keys)
            {
                entries.Add(new Entry(k, dictionary[k]));
            }

            var serializer = XmlSerializerHelper.GetSerializer(typeof(List<Entry>));
            serializer.Serialize(writer, entries);
        }
    }
    public class MeasureFirstPartInfo : DocumentInfo
    {
        private Document document;

        public MeasureFirstPartInfo(IContainerExtension container) : base(container)
        {
        }

        public override Document CreateDocumentInfos(string[]? folders)
        {
            document = new FirstPartDocument();
            document[DocumentPart.Type] = "FirstPart";
            document[DocumentPart.RootPath] = string.Empty;
            document[DocumentPart.Template] = string.Empty;
            document[DocumentPart.RegularEx] = string.Empty;
            if (RuleInfo.Rules.Keys.Contains(document[DocumentPart.Type]) == false) return document;
            var xml = XmlSerializerHelper.GetSerializer(typeof(List<Entry>));

            TextReader reader = new StringReader(RuleInfo.Rules[document[DocumentPart.Type]].RuleData);
            List<Entry> doc = (List<Entry>)xml.Deserialize(reader);
            foreach (var entry in doc)
            {
                DocumentPart DokuPart = (DocumentPart)Enum.Parse(typeof(DocumentPart), entry.Key.ToString());
                document[DokuPart] = (string)entry.Value;
            }
            if (folders != null)
            {

                document[DocumentPart.TTNR] = folders[0];
                Regex regex = new Regex(document[DocumentPart.RegularEx]);
                Match match2 = regex.Match(folders[0]);
                StringBuilder nsb = new StringBuilder();
                foreach (Group ma in match2.Groups.Values.Skip(1))
                {
                    if (ma.Value != folders[0])
                    {
                        nsb.Append(ma.Value).Append(Path.DirectorySeparatorChar);
                    }
                }
                document[DocumentPart.SavePath] = nsb.ToString();
                FileInfo f = new(document[DocumentPart.Template]);
                document[DocumentPart.File] = Path.Combine(
                    document[DocumentPart.RootPath],
                    document[DocumentPart.SavePath],
                    f.Name.Replace("Messblatt", folders[0])); 
            }

            return document;
        }

        public override Document CreateDocumentInfos()
        {
            return CreateDocumentInfos(null);
        }
        public void SaveDocumentData()
        {
            base.SaveDocumentData(document);
        }
        public void Collect()
        {
            base.Collect(document);
        }

        public override Document GetDocument()
        {
            return document;
        }
    }
    public class VmpbDocumentInfo : DocumentInfo
    {
        private Document document;

        public VmpbDocumentInfo(IContainerExtension container) : base(container)
        {
        }

        public override Document CreateDocumentInfos(string[]? folders)
        {
            document = new VmpbDocument();
            document[DocumentPart.Type] = "VmpbPart";
            document[DocumentPart.RootPath] = string.Empty;
            document[DocumentPart.Template] = string.Empty;
            document[DocumentPart.RegularEx] = string.Empty;
            if (RuleInfo.Rules.Keys.Contains(document[DocumentPart.Type]) == false) return document;
            var xml = XmlSerializerHelper.GetSerializer(typeof(List<Entry>));

            TextReader reader = new StringReader(RuleInfo.Rules[document[DocumentPart.Type]].RuleData);
            List<Entry> doc = (List<Entry>)xml.Deserialize(reader);
            foreach (var entry in doc)
            {
                DocumentPart DokuPart = (DocumentPart)Enum.Parse(typeof(DocumentPart), entry.Key.ToString());
                document[DokuPart] = (string)entry.Value;
            }
            if (folders != null)
            {

                document[DocumentPart.TTNR] = folders[0];
                Regex regex = new Regex(document[DocumentPart.RegularEx]);
                Match match2 = regex.Match(folders[0]);
                StringBuilder nsb = new StringBuilder();
                foreach (Group ma in match2.Groups.Values.Skip(1))
                {
                    if (ma.Value != folders[0])
                    {
                        nsb.Append(ma.Value).Append(Path.DirectorySeparatorChar);
                    }
                }
                document[DocumentPart.SavePath] = nsb.Append(folders[1]).Append(Path.DirectorySeparatorChar).ToString();
                document[DocumentPart.File] = Path.Combine(
                    document[DocumentPart.RootPath],
                    document[DocumentPart.SavePath],
                    folders[0] + "_VMPB.dotx"); 
            }

            return document;
        }

        public override Document CreateDocumentInfos()
        {
            return CreateDocumentInfos(null);
        }
        public void SaveDocumentData()
        {
            base.SaveDocumentData(document);
        }
        public void Collect()
        {
            base.Collect(document);
        }

        public override Document GetDocument()
        {
            return document;
        }
    }
    public class WorkareaDocumentInfo : DocumentInfo
    {
        private Document document;

        public WorkareaDocumentInfo(IContainerExtension container) : base(container)
        {
        }

        public override Document CreateDocumentInfos(string[]? folders)
        {
            document = new FirstPartDocument();
            document[DocumentPart.Type] = "WorkPart";
            document[DocumentPart.RootPath] = string.Empty;
            document[DocumentPart.Template] = string.Empty;
            document[DocumentPart.RegularEx] = string.Empty;
            if (RuleInfo.Rules.Keys.Contains(document[DocumentPart.Type]) == false) return document;
            var xml = XmlSerializerHelper.GetSerializer(typeof(List<Entry>));

            TextReader reader = new StringReader(RuleInfo.Rules[document[DocumentPart.Type]].RuleData);
            List<Entry> doc = (List<Entry>)xml.Deserialize(reader);
            foreach (var entry in doc)
            {
                DocumentPart DokuPart = (DocumentPart)Enum.Parse(typeof(DocumentPart), entry.Key.ToString());
                document[DokuPart] = (string)entry.Value;
            }

            if (folders != null)
            {

                document[DocumentPart.TTNR] = folders[0];
                Regex regex = new Regex(document[DocumentPart.RegularEx]);
                Match match2 = regex.Match(folders[0]);
                StringBuilder nsb = new StringBuilder();
                foreach (Group ma in match2.Groups.Values.Skip(1))
                {
                    if (ma.Value != folders[0])
                    {
                        nsb.Append(ma.Value).Append(Path.DirectorySeparatorChar);
                    }
                }
                document[DocumentPart.SavePath] = nsb.Append(folders[1]).Append(Path.DirectorySeparatorChar)
                    .Append(folders[2]).Append(Path.DirectorySeparatorChar).ToString(); 
            }

            return document;
        }

        public override Document CreateDocumentInfos()
        {
            return CreateDocumentInfos(null);
        }
        public void SaveDocumentData()
        {
            base.SaveDocumentData(document);
        }
        public void Collect()
        {
            base.Collect(document);
        }

        public override Document GetDocument()
        {
            return document;
        }
    }
    //public class MeasureFirstPartBuilder : DocumentBuilder
    //{
    //    public CompositeNode<Shape> Root { get; private set; }
    //    public MeasureFirstPartBuilder() : base()
    //    { }
    //    public override void Build(IContainerExtension container, string[] ttnr)
    //    {
    //        base.container = container;
    //        if (RuleInfo.Rules.Keys.Contains("FirstPart") == false) return;
    //        var xml = XmlSerializerHelper.GetSerializer(typeof(List<Entry>));

    //        TextReader reader = new StringReader(RuleInfo.Rules["FirstPart"].RuleData);
    //        List<Entry> doc = (List<Entry>)xml.Deserialize(reader);
    //        foreach (var entry in doc)
    //        {
    //            DocumentPart DokuPart = (DocumentPart)Enum.Parse(typeof(DocumentPart), entry.Key.ToString());
    //            Document[DokuPart] = (string)entry.Value;
    //        }
    //        Root = new CompositeNode<Shape> { Node = new Shape(Document[DocumentPart.RootPath]) };
    //        Document[DocumentPart.TTNR] = ttnr[0];
    //        Regex regex = new Regex(Document[DocumentPart.RegularEx]);
    //        Match match2 = regex.Match(ttnr[0]);
    //        StringBuilder nsb = new StringBuilder();
    //        foreach (Group ma in match2.Groups.Values.Skip(1))
    //        {
    //            if (ma.Value != ttnr[0])
    //            {
    //                nsb.Append(ma.Value).Append(Path.DirectorySeparatorChar);
    //                Root.Add(new Shape(ma.Value));
    //            }
    //        }
    //        Document[DocumentPart.SavePath] = Path.Combine(Document[DocumentPart.RootPath], nsb.ToString());
    //        FileInfo f = new(Document[DocumentPart.Template]);
    //        Document[DocumentPart.File] = Path.Combine(Document[DocumentPart.SavePath], f.Name.Replace("Messblatt", ttnr[0]));
    //    }
    //    public override FileInfo GetDataSheet()
    //    {
    //        return new FileInfo(Document[DocumentPart.File]);
    //    }

    //    public override void SaveDocumentData()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void SaveDocumentData(string rootPath, string[] template, string RegEx)
    //    {
    //        var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
    //        var rule = db.Rules.SingleOrDefault(x => x.RuleValue == "M1");
    //        //var xml = XmlSerializerHelper.GetSerializer(typeof(Dictionary<string, string>));

    //        if(rule == null) { rule = new(); rule.RuleName = "FirstPart"; rule.RuleValue = "M1"; db.Rules.Add(rule); }
    //        var dict = new Dictionary<string, string>();
    //        Document[DocumentPart.RootPath] = rootPath;
    //        Document[DocumentPart.Template] = template[0];
    //        Document[DocumentPart.RegularEx] = RegEx;

    //        StringWriter sw = new StringWriter();
    //        Serialize(sw, Document);
    //        rule.RuleData = sw.ToString();

    //        db.SaveChanges();
    //    }
    //    private static void Serialize(TextWriter writer, Document dictionary)
    //    {
    //        List<Entry> entries = new List<Entry>();


    //        entries.Add(new Entry(DocumentPart.RootPath, dictionary[DocumentPart.RootPath]));
    //        entries.Add(new Entry(DocumentPart.Template, dictionary[DocumentPart.Template]));
    //        entries.Add(new Entry(DocumentPart.RegularEx, dictionary[DocumentPart.RegularEx]));

    //        var serializer = XmlSerializerHelper.GetSerializer(typeof(List<Entry>));
    //        serializer.Serialize(writer, entries);
    //    }
    //    private static void Deserialize(TextReader reader, IDictionary dictionary)
    //    {
    //        dictionary.Clear();
    //        XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
    //        List<Entry> list = (List<Entry>)serializer.Deserialize(reader);
    //        foreach (Entry entry in list)
    //        {
    //            dictionary[entry.Key] = entry.Value;
    //        }
    //    }

    //    public override string Collect()
    //    {
    //        string path = Root.Node.ToString();
    //        if (!Directory.Exists(path)) return string.Empty;
    //        List<CompositeNode<Shape>>? listdir = Root.Children;

    //        for (int i = 0; i < listdir?.Count; i++)
    //        {
    //            path = Path.Combine(path, listdir[i].Node.ToString());
    //            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    //        }
    //        return path;
    //    }

    //    public override string Collect(string target)
    //    {
    //        return Path.Combine(Collect(), target);
    //    }

    //    public override DocumentBuilder GetBuilder()
    //    {
    //        return this;
    //    }
    //}
    //public class VmpbPartBuilder : DocumentBuilder
    //{
    //    public CompositeNode<Shape> Root { get; private set; }
    //    public VmpbPartBuilder() : base()
    //    { }
    //    public override void Build(IContainerExtension container, string[] ttnr)
    //    {
    //        base.container = container;
    //        if (RuleInfo.Rules.Keys.Contains("VmpbPart") == false) return;
    //        var xml = XmlSerializerHelper.GetSerializer(typeof(List<Entry>));

    //        TextReader reader = new StringReader(RuleInfo.Rules["VmpbPart"].RuleData);
    //        List<Entry> doc = (List<Entry>)xml.Deserialize(reader);
    //        foreach (var entry in doc)
    //        {
    //            DocumentPart DokuPart = (DocumentPart)Enum.Parse(typeof(DocumentPart), entry.Key.ToString());
    //            Document[DokuPart] = (string)entry.Value;
    //        }
    //        Root = new CompositeNode<Shape> { Node = new Shape(Document[DocumentPart.RootPath]) };
    //        Document[DocumentPart.TTNR] = ttnr[0];
    //        Regex regex = new Regex(Document[DocumentPart.RegularEx]);
    //        Match match2 = regex.Match(ttnr[0]);
    //        StringBuilder nsb = new StringBuilder();
    //        foreach (Group ma in match2.Groups.Values.Skip(1))
    //        {
    //            if (ma.Value != ttnr[0])
    //            {
    //                nsb.Append(ma.Value).Append(Path.DirectorySeparatorChar);
    //                Root.Add(new Shape(ma.Value));
    //            }
    //        }

    //        Document[DocumentPart.SavePath] = Path.Combine(Document[DocumentPart.RootPath], nsb.ToString(), ttnr[1]);
    //        FileInfo f = new(Document[DocumentPart.Template]);
    //        Document[DocumentPart.File] = Path.Combine(Document[DocumentPart.SavePath], ttnr[0] + "_VMPB.dotx");
    //    }
    //    public override FileInfo GetDataSheet()
    //    {
    //        return new FileInfo(Document[DocumentPart.File]);
    //    }

    //    public override void SaveDocumentData()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void SaveDocumentData(string rootPath, string[] template, string RegEx)
    //    {
    //        var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
    //        var rule = db.Rules.SingleOrDefault(x => x.RuleValue == "M2");
    //        //var xml = XmlSerializerHelper.GetSerializer(typeof(Dictionary<string, string>));

    //        if (rule == null) { rule = new(); rule.RuleName = "VmpbPart"; rule.RuleValue = "M2"; db.Rules.Add(rule); }
    //        var dict = new Dictionary<string, string>();
    //        Document[DocumentPart.RootPath] = rootPath;
    //        Document[DocumentPart.Template] = template[0];
    //        Document[DocumentPart.Template_Size2] = template[1];
    //        Document[DocumentPart.Template_Size3] = template[2];
    //        Document[DocumentPart.RegularEx] = RegEx;

    //        StringWriter sw = new StringWriter();
    //        Serialize(sw, Document);
    //        rule.RuleData = sw.ToString();

    //        db.SaveChanges();
    //    }
    //    private static void Serialize(TextWriter writer, Document dictionary)
    //    {
    //        List<Entry> entries = new List<Entry>();


    //        entries.Add(new Entry(DocumentPart.RootPath, dictionary[DocumentPart.RootPath]));
    //        entries.Add(new Entry(DocumentPart.Template, dictionary[DocumentPart.Template]));
    //        entries.Add(new Entry(DocumentPart.Template_Size2, dictionary[DocumentPart.Template_Size2]));
    //        entries.Add(new Entry(DocumentPart.Template_Size3, dictionary[DocumentPart.Template_Size3]));
    //        entries.Add(new Entry(DocumentPart.RegularEx, dictionary[DocumentPart.RegularEx]));

    //        var serializer = XmlSerializerHelper.GetSerializer(typeof(List<Entry>));
    //        serializer.Serialize(writer, entries);
    //    }
    //    private static void Deserialize(TextReader reader, IDictionary dictionary)
    //    {
    //        dictionary.Clear();
    //        XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
    //        List<Entry> list = (List<Entry>)serializer.Deserialize(reader);
    //        foreach (Entry entry in list)
    //        {
    //            dictionary[entry.Key] = entry.Value;
    //        }
    //    }

    //    public override string Collect()
    //    {

    //        if (!Directory.Exists(Document[DocumentPart.RootPath])) return string.Empty;

    //        string path = Document[DocumentPart.SavePath];
    //        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

    //        return path;
    //    }

    //    public override string Collect(string target)
    //    {
    //        return Path.Combine(Collect(), target);
    //    }

    //    public override DocumentBuilder GetBuilder()
    //    {
    //        return this;
    //    }
    //}

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
        Template_Size2,
        Template_Size3,
        SavePath,
        TTNR,
        File,
        Type
    }
    public enum DocumentType
    {
        //file://///bosch.com/DfsRB/DfsAT/Loc/Hl/Abt/Technical_Functions/420_Musterbau/200_Bereiche/250_Vormuster/COS_Messraum/Vorlagen/Vorlagen_VMPB
        MeasureFirstPart,
        MeasureVMPB
    }

    abstract class DocumentCreator
    {
        public abstract IDocument FactoryMethod();
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
