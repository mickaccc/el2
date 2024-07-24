using El2Core.Models;
using El2Core.ViewModelBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.Data.Xml.Dom;
using static El2Core.Constants.ProjectTypes;

namespace El2Core.Utils
{

    /// <summary>
    /// Generic tree node class
    /// </summary>
    /// <typeparam name="T">Node type</typeparam>
    public class PspNode<T> where T : IComparable<T>
    {
        // Add a child tree node
        public PspNode<T> Add(T child, string nodeType)
        {
            var newNode = new PspNode<T> { Node = child };
            newNode.NodeType = nodeType;
            Children.Add(newNode);
            return newNode;
        }
        // Remove a child tree node
        public void Remove(T child)
        {
            foreach (var treeNode in Children)
            {
                if (treeNode.Node.CompareTo(child) == 0)
                {
                    Children.Remove(treeNode);
                    return;
                }
            }
        }
        // Gets or sets the node
        public T Node { get; set; } = default!;
        // Gets treenode children
        public List<PspNode<T>> Children { get; } = [];
        public string NodeType { get; private set; } = "unknown";
        public bool HasOrder { get { return Children.Any(x => x.NodeType == "Order-Type"); } }
  
        // Recursively displays node and its children 
        public static void Display(PspNode<T> node, int indentation)
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
    public class Shape(string name) : ViewModelBase.ViewModelBase, IComparable<Shape>
    {
        private string? _description;
        public string? Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    NotifyPropertyChanged(() => Description);
                }
            }
        }
        private ProjectType _projectType;

        public ProjectType ProjectType
        {
            get { return _projectType; }
            set
            {
                if (value != _projectType)
                {
                    _projectType = value;
                    NotifyPropertyChanged(() => ProjectType);
                }
            }
        }
        public override string ToString() => name;

        // IComparable<Shape> Member
        public int CompareTo(Shape? other) => (this == other) ? 0 : -1;

    }
}
