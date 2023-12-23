using El2Core.ViewModelBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace El2Core.Utils
{
    public class TreeNode : IEnumerable<TreeNode>
    {
        private readonly ObservableCollection<TreeNode>? _children =
                                            new ();
        public ObservableCollection<TreeNode>? Children { get { return _children; } }

        public string PSP { get; }
        public string NodeType { get; }
        public bool IsChanged { get; private set; }
        public bool ChangeTracker { get; set; } = false;
        private string? _description;
        public string? Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    if (ChangeTracker) IsChanged = true;
                }
            }
        }
        public TreeNode? Parent { get; private set; }

        public TreeNode(string psp)
        {
            this.PSP = psp;
            this.NodeType = (psp.StartsWith("DS")) ? "PSP-Type" : "Order-Type";
        }

        public TreeNode? GetChild(string psp)
        {
            return this._children?.FirstOrDefault(x => x.PSP == psp);
        }

        public void Add(TreeNode item)
        {
            if (item.Parent != null)
            {
                item.Parent._children?.Remove(item);
            }

            item.Parent = this;
            this._children?.Add(item);
        }

        public IEnumerator<TreeNode>? GetEnumerator()
        {
            if (_children != null)
            {
                return this._children.GetEnumerator();
            }
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int? Count
        {
            get { return this._children?.Count; }
        }
    }
}
