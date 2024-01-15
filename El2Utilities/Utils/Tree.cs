using El2Core.Constants;
using El2Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public Tree<TreeNode<string>> GetPspBranch(Project project, string order, ref Tree<string> tree)
        {
            var root = project.ProjectPsp[..9];
  
            if (tree.level == 0)
            {
                tree.Begin(root);
            }

            var parent = tree.Nodes.Last();
            for (int i = 12; i < project.ProjectPsp.Length; i += 3)
            {
  
                var next = parent.Children.FirstOrDefault(x => x.Value == project.ProjectPsp[..i]);
                if(next == null)
                    {
                        next = parent.Add(project.ProjectPsp[..i]);
                    }
                parent = next;
                if(project.ProjectPsp.Length == i)
                {
                    parent.Description = project.ProjectInfo ??= string.Empty;
                    parent.ProjectType = (ProjectType) project.ProjectType;
                    parent.Add(order);
                }
 
            }
            
            while (tree.level > 0) {tree.End(); } // close all
            return null;
        }
    }

    public class TreeNode<T>
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
                NodeType = s.StartsWith("ds", StringComparison.OrdinalIgnoreCase) ? "PSP-Type" : "Order-Type";
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
}
