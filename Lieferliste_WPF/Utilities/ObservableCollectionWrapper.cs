using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace Lieferliste_WPF.Utilities
{
    public class ObservableCollectionWrapper<T> : ICollection<T>, INotifyCollectionChanged
    {
        private readonly ObservableCollection<T> _collection;
        private readonly Dispatcher _dispatcher;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ObservableCollectionWrapper(ObservableCollection<T> collection, Dispatcher dispatcher)
        {
            _collection = collection;
            _dispatcher = dispatcher;
            collection.CollectionChanged += Internal_CollectionChanged;
        }

        private void Internal_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _dispatcher.Invoke(() =>
            {
                this.CollectionChanged?.Invoke(sender, e);
            });
        }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int Count => _collection.Count;

        public bool IsReadOnly => throw new NotImplementedException();
        /* Implement the rest of the ICollection<T> interface */
    }
}
