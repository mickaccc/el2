using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace El2Core.Utils
{
    public class ObservableSortedSet<T> : ICollection<T>,
                                          INotifyCollectionChanged,
                                          INotifyPropertyChanged
    {
        readonly SortedSet<T> _innerCollection = new SortedSet<T>();

        public IEnumerator<T> GetEnumerator()
        {
            return _innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            if (item != null)
            {
                if (_innerCollection.Add(item))
                    OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
            }
        }

        public void Clear()
        {
            if (_innerCollection.Any())
            {
                _innerCollection.Clear();
                //OnCollectionChanged(NotifyCollectionChangedAction.Reset);
            }
        }

        public bool Contains(T item)
        {
            return _innerCollection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _innerCollection.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var ret = _innerCollection.Remove(item);
            if (ret)
            {
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
            }
            return ret;
        }

        public int Count
        {
            get { return _innerCollection.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((ICollection<T>)_innerCollection).IsReadOnly; }
        }
        private void OnCollectionChanged(NotifyCollectionChangedAction action, T elements)
        {
            if(CollectionChanged != null && elements != null)
            {
                    var eventArgs = new NotifyCollectionChangedEventArgs(action, elements);
                    CollectionChanged(this, eventArgs); 
                
            }
        }
        // TODO: possibly add some specific methods, if needed

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
