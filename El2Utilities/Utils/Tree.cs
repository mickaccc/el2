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
    public class Tree<T>
    {
        private Stack<TreeNode<T>> m_Stack = new Stack<TreeNode<T>>();
        public int level = 0;
        public HashSet<TreeNode<T>> Nodes { get; } = new HashSet<TreeNode<T>>();

        public Tree<T> Begin(T val)
        {
            if (m_Stack.Count == 0)
            {
                var node = new TreeNode<T>(val, null);
                Nodes.Add(node);
                m_Stack.Push(node);
            }
            else
            {
                var node = m_Stack.Peek().Add(val);
                m_Stack.Push(node);
            }
            level++;
            return this;
        }

        public Tree<T> Add(T val)
        {
            m_Stack.Peek().Add(val);
            return this;
        }

        public Tree<T> End()
        {
            m_Stack.Pop();
            level--;
            return this;
        }
    }

    public class TreeNode<T> : ViewModelBase.ViewModelBase
    {
        private string _description = string.Empty;
        public string Description
        {
            get { return _description; }
            set
            {
                if(_description != value)
                {
                    _description = value;
                    NotifyPropertyChanged(() => Description);
                }
            }
        }
        private ProjectType _projectType = 0;

        public ProjectType ProjectType
        {
            get { return _projectType; }
            set
            {
                if(value != _projectType)
                {
                    _projectType = value;
                    NotifyPropertyChanged(() => ProjectType);
                }
            }
        }
        public bool HasOrder { get { return Children.Any(x => x.NodeType == "Order-Type"); } }
        public string NodeType { get; }
        public T Value { get; }

        public TreeNode<T> Parent { get; }
        public List<TreeNode<T>> Children { get; }

        public TreeNode(T val, TreeNode<T> parent)
        {
            Value = val;
            Parent = parent;
            Children = new List<TreeNode<T>>();
            if (val is string s)
            {
                NodeType = s.StartsWith("ds", StringComparison.OrdinalIgnoreCase) ||
                    s.StartsWith("sc-pr", StringComparison.OrdinalIgnoreCase) ? "PSP-Type" : "Order-Type";
            }
            else NodeType = "invalid";

        }

        public TreeNode<T> Add(T val)
        {
            var node = new TreeNode<T>(val, this);
            Children.Add(node);
            return node;
        }
    }
    public static class TreeHelper
    {
        public static bool TryBuildPspTree(Project project, ref Tree<string> tree)
        {
            var psp = project.ProjectPsp.Trim();
            var root = psp[..9];
            var parent = tree.Nodes.FirstOrDefault(x => x.Value == root);
            if (parent == null)
            {
                tree.Begin(root);
                parent = tree.Nodes.Last();
            }


            for (int i = 9; i <= psp.Length; i += 3)
            {
                var next = parent.Children.FirstOrDefault(x => x.Value == psp[..i]);
                if (next == null && parent.Value != root)
                {
                    next = parent.Add(psp[..i]);
                }
                if (next != null) parent = next;
                if (psp.Length == i)
                {
                    parent.Description = project.ProjectInfo ??= string.Empty;
                    parent.ProjectType = (ProjectType)project.ProjectType;

                    foreach (var o in project.OrderRbs)
                    {
                        parent.Children.Add(new TreeNode<string>(o.Aid, parent));
                    }
                }

            }

            while (tree.level > 0) { tree.End(); } // close all
            return true;
        }

    }
    public class PspTree
    {
        private Stack<BranchNode> m_Stack = new Stack<BranchNode>();
        public int level = 0;
        public HashSet<BranchNode> Nodes { get; } = new HashSet<BranchNode>();

        public PspTree Begin(Project val, string? order)
        {
            if (m_Stack.Count == 0)
            {
                var node = new BranchNode(val, null);
                Nodes.Add(node);
                m_Stack.Push(node);
            }
            else
            {
                var node = m_Stack.Peek().Add(val, order);
                m_Stack.Push(node);
            }
            level++;
            return this;
        }

        public PspTree Add(Project pro, string? order)
        {
            m_Stack.Peek().Add(pro, order);
            return this;
        }

        public PspTree End()
        {
            m_Stack.Pop();
            level--;
            return this;
        }
    }
    public class BranchNode
    {
        private string _description = string.Empty;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        private ProjectType _projectType = ProjectType.None;
        public ProjectType ProjectType
        {
            get { return _projectType; }
            set { _projectType = value; }
        }
        public string Value { get; set; }
        public string NodeType { get; }
        public BranchNode Parent { get; }
        public List<BranchNode> Children { get; }
        public BranchNode(Project project, string? order)
        {
            Value = project.ProjectPsp;
            _description = project.ProjectInfo ??= string.Empty;
            _projectType = (ProjectType)project.ProjectType;
            Parent = this;
            Children = new List<BranchNode>();

            if (project.ProjectPsp is string s)
            {
                NodeType = s.StartsWith("ds", StringComparison.OrdinalIgnoreCase) ||
                    s.StartsWith("sc-pr", StringComparison.OrdinalIgnoreCase) ? "PSP-Type" : "Order-Type";
            }
            else NodeType = "invalid";
        }

        public BranchNode Add(Project project, string? order)
        {
            var node = new BranchNode(project, order);
            Children.Add(node);
            return node;
        }
        public static bool TryBuildPspBranch(Project project, ref PspTree tree)
        {
            var psp = project.ProjectPsp.Trim();
            var root = psp[..9];
            var parent = tree.Nodes.FirstOrDefault(x => x.Value == root);
            if (parent == null)
            {
                var pro = new Project() { ProjectPsp = root };
                tree.Begin(pro, null);
                parent = tree.Nodes.Last();
            }

            for (int i = 9; i <= psp.Length; i += 3)
            {
                var next = parent.Children.FirstOrDefault(x => x.Value == psp[..i]);
                if (next == null && parent.Value != root)
                {
                    var pro = new Project() { ProjectPsp = psp[..i] };
                    next = parent.Add(pro, null);
                }
                if (next != null) parent = next;
                if (psp.Length == i)
                {
                    parent.Description = project.ProjectInfo ??= string.Empty;
                    parent.ProjectType = (ProjectType)project.ProjectType;

                    foreach (var o in project.OrderRbs)
                    {
                        parent.Children.Add(new BranchNode(project, o.Aid));
                    }
                }

            }

            while (tree.level > 0) { tree.End(); } // close all
            return true;
        }
    }
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
        private string? _description = string.Empty;
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
        private ProjectType _projectType = ProjectType.None;

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
