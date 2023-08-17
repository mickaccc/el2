using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Lieferliste_WPF.Utilities
{


    public class SortableObservableCollection<T> : ObservableCollection<T>
    {

        public void Sort()
        {
            Sort(Comparer<T>.Default);
        }

        public void Sort(IComparer<T> comparer)
        {

            int i;

            for (i = 1; i < Count; i++)
            {

                var index = this[i];     //If you can't read it, it should be index = this[x], where x is i :-)

                var j = i;

                while ((j > 0) && (comparer.Compare(this[j - 1], index) == 1))
                {

                    this[j] = this[j - 1];

                    j = j - 1;

                }

                this[j] = index;

            }

        }

        public static explicit operator SortableObservableCollection<T>(List<object> v)
        {
            throw new NotImplementedException();
        }
    }



    public class ObservableList<T> : IList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties
        private List<T> _List
        {
            get;
        }

        public T this[int index]
        {
            get
            {
                return _List[index];
            }
            set
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException("The specified index is out of range.");
                var oldItem = _List[index];
                _List[index] = value;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem, index));
            }
        }

        public int Count
        {
            get => _List.Count;
            
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IList<T>)_List).IsReadOnly;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return ((IList)_List).IsFixedSize;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return ((IList)_List)[index];
            }
            set
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException("The specified index is out of range.");
                var oldItem = ((IList)_List)[index];
                ((IList)_List)[index] = value;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem, index));
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return ((IList)_List).IsSynchronized;
            }
        }

        public object SyncRoot
        {
            get
            {
                return ((IList)_List).SyncRoot;
            }
        }
        #endregion

        #region Methods
        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, args);
        }



        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddRange(IEnumerable<T> collection)
        {
            _List.AddRange(collection);
            var iList = collection as IList;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public int IndexOf(T item)
        {
            return _List.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _List.Insert(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException("The specified index is out of range.");
            var oldItem = _List[index];
            _List.RemoveAt(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
        }

        public void Add(T item)
        {
            _List.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Clear()
        {
            _List.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(T item)
        {
            return _List.Contains(item);
        }
        public void Sort(IComparer<T> comparer)
        {
            _List.Sort(comparer);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _List.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var result = _List.Remove(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return result;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        int IList.Add(object value)
        {
            var result = ((IList)_List).Add(value);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
            return result;
        }

        bool IList.Contains(object value)
        {
            return ((IList)_List).Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return ((IList)_List).IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            ((IList)_List).Insert(index, value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
        }

        void IList.Remove(object value)
        {
            ((IList)_List).Remove(value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));
        }

        public void CopyTo(Array array, int index)
        {
            ((IList)_List).CopyTo(array, index);
        }
        #endregion

        #region Initialization
        public ObservableList()
        {
            _List = new List<T>();
        }

        public ObservableList(int capacity)
        {
            _List = new List<T>(capacity);
        }

        public ObservableList(IEnumerable<T> collection)
        {
            _List = new List<T>(collection);
        }
        #endregion
    }
    /// <summary>
    /// This class is a LinkedList that can be used in a WPF MVVM scenario. Composition was used instead of inheritance,
    /// because inheriting from LinkedList does not allow overriding its methods.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableLinkedList<T> : INotifyCollectionChanged, IEnumerable
    {
        private LinkedList<T> m_UnderLyingLinkedList;

        #region Variables accessors
        public int Count
        {
            get { return m_UnderLyingLinkedList.Count; }
        }

        public LinkedListNode<T> First => m_UnderLyingLinkedList.First;

        public LinkedListNode<T> Last
        {
            get { return m_UnderLyingLinkedList.Last; }
        }
        #endregion

        #region Constructors
        public ObservableLinkedList()
        {
            m_UnderLyingLinkedList = new LinkedList<T>();
        }

        public ObservableLinkedList(IEnumerable<T> collection)
        {
            m_UnderLyingLinkedList = new LinkedList<T>(collection);
        }
        #endregion

        #region LinkedList<T> Composition
        public LinkedListNode<T> AddAfter(LinkedListNode<T> prevNode, T value)
        {
            LinkedListNode<T> ret = m_UnderLyingLinkedList.AddAfter(prevNode, value);
            OnNotifyCollectionChanged();
            return ret;
        }

        public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            m_UnderLyingLinkedList.AddAfter(node, newNode);
            OnNotifyCollectionChanged();
        }

        public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)
        {
            LinkedListNode<T> ret = m_UnderLyingLinkedList.AddBefore(node, value);
            OnNotifyCollectionChanged();
            return ret;
        }

        public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            m_UnderLyingLinkedList.AddBefore(node, newNode);
            OnNotifyCollectionChanged();
        }

        public LinkedListNode<T> AddFirst(T value)
        {
            LinkedListNode<T> ret = m_UnderLyingLinkedList.AddFirst(value);
            OnNotifyCollectionChanged();
            return ret;
        }

        public void AddFirst(LinkedListNode<T> node)
        {
            m_UnderLyingLinkedList.AddFirst(node);
            OnNotifyCollectionChanged();
        }

        public LinkedListNode<T> AddLast(T value)
        {
            LinkedListNode<T> ret = m_UnderLyingLinkedList.AddLast(value);
            OnNotifyCollectionChanged();
            return ret;
        }

        public void AddLast(LinkedListNode<T> node)
        {
            m_UnderLyingLinkedList.AddLast(node);
            OnNotifyCollectionChanged();
        }

        public void Clear()
        {
            m_UnderLyingLinkedList.Clear();
            OnNotifyCollectionChanged();
        }

        public bool Contains(T value)
        {
            return m_UnderLyingLinkedList.Contains(value);
        }

        public void CopyTo(T[] array, int index)
        {
            m_UnderLyingLinkedList.CopyTo(array, index);
        }

        public bool LinkedListEquals(object obj)
        {
            return m_UnderLyingLinkedList.Equals(obj);
        }

        public LinkedListNode<T> Find(T value)
        {
            return m_UnderLyingLinkedList.Find(value);
        }

        public LinkedListNode<T> FindLast(T value)
        {
            return m_UnderLyingLinkedList.FindLast(value);
        }

        public Type GetLinkedListType()
        {
            return m_UnderLyingLinkedList.GetType();
        }

        public bool Remove(T value)
        {
            bool ret = m_UnderLyingLinkedList.Remove(value);
            OnNotifyCollectionChanged();
            return ret;
        }

        public void Remove(LinkedListNode<T> node)
        {
            m_UnderLyingLinkedList.Remove(node);
            OnNotifyCollectionChanged();
        }

        public void RemoveFirst()
        {
            m_UnderLyingLinkedList.RemoveFirst();
            OnNotifyCollectionChanged();
        }

        public void RemoveLast()
        {
            m_UnderLyingLinkedList.RemoveLast();
            OnNotifyCollectionChanged();
        }

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public void OnNotifyCollectionChanged()
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (m_UnderLyingLinkedList as IEnumerable).GetEnumerator();
        }

        #endregion

    }
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";
        private const string KeysName = "Keys";
        private const string ValuesName = "Values";
        private IDictionary<TKey, TValue> _Dictionary;
        protected IDictionary<TKey, TValue> Dictionary
        {
            get { return _Dictionary; }
        }
        #region Constructors
        public ObservableDictionary()
        {
            _Dictionary = new Dictionary<TKey, TValue>();
        }
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _Dictionary = new Dictionary<TKey, TValue>(dictionary);
        }
        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            _Dictionary = new Dictionary<TKey, TValue>(comparer);
        }
        public ObservableDictionary(int capacity)
        {
            _Dictionary = new Dictionary<TKey, TValue>(capacity);
        }
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            _Dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }
        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            _Dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }
        #endregion
        #region IDictionary<TKey,TValue> Members
        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }
        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }
        public ICollection<TKey> Keys
        {
            get { return Dictionary.Keys; }
        }
        public bool Remove(TKey key)
        {
            if (key == null) throw new ArgumentNullException("key");
            TValue value;
            Dictionary.TryGetValue(key, out value);
            var removed = Dictionary.Remove(key);
            if (removed)
                //OnCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value));
                OnCollectionChanged();
            return removed;
        }
        public bool TryGetValue(TKey key, out TValue value)
        {
            return Dictionary.TryGetValue(key, out value);
        }
        public ICollection<TValue> Values
        {
            get { return Dictionary.Values; }
        }
        public TValue this[TKey key]
        {
            get
            {
                return Dictionary[key];
            }
            set
            {
                Insert(key, value, false);
            }
        }
        #endregion
        #region ICollection<KeyValuePair<TKey,TValue>> Members
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Insert(item.Key, item.Value, true);
        }
        public void Clear()
        {
            if (Dictionary.Count > 0)
            {
                Dictionary.Clear();
                OnCollectionChanged();
            }
        }
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Dictionary.Contains(item);
        }
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Dictionary.CopyTo(array, arrayIndex);
        }
        public int Count
        {
            get { return Dictionary.Count; }
        }
        public bool IsReadOnly
        {
            get { return Dictionary.IsReadOnly; }
        }
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }
        #endregion
        #region IEnumerable<KeyValuePair<TKey,TValue>> Members
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }
        #endregion
        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Dictionary).GetEnumerator();
        }
        #endregion
        #region INotifyCollectionChanged Members
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        public void AddRange(IDictionary<TKey, TValue> items)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (items.Count > 0)
            {
                if (Dictionary.Count > 0)
                {
                    if (items.Keys.Any((k) => Dictionary.ContainsKey(k)))
                        throw new ArgumentException("An item with the same key has already been added.");
                    else
                        foreach (var item in items) Dictionary.Add(item);
                }
                else
                    _Dictionary = new Dictionary<TKey, TValue>(items);
                OnCollectionChanged(NotifyCollectionChangedAction.Add, items.ToArray());
            }
        }
        private void Insert(TKey key, TValue value, bool add)
        {
            if (key == null) throw new ArgumentNullException("key");
            TValue item;
            if (Dictionary.TryGetValue(key, out item))
            {
                if (add) throw new ArgumentException("An item with the same key has already been added.");
                if (Equals(item, value)) return;
                Dictionary[key] = value;
                OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, item));
            }
            else
            {
                Dictionary[key] = value;
                OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
            }
        }
        private void OnPropertyChanged()
        {
            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
            OnPropertyChanged(KeysName);
            OnPropertyChanged(ValuesName);
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        private void OnCollectionChanged()
        {
            OnPropertyChanged();
            if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem)
        {
            OnPropertyChanged();
            if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem));
        }
        private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
        {
            OnPropertyChanged();
            if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem));
        }
        private void OnCollectionChanged(NotifyCollectionChangedAction action, IList newItems)
        {
            OnPropertyChanged();
            if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItems));
        }

        ///// <summary>
        ///// Sortiert die Elemente einer <see cref="ObservableCollection{TSource}"/>.
        ///// </summary>
        ///// <typeparam name="TSource">Der Typ der Elemente in der Auflistung.</typeparam>
        ///// <typeparam name="TKey">Der Typ nam dem Sortiert werden soll.</typeparam>
        ///// <param name="source">Die zu sortierende Auflistung.</param>
        ///// <param name="keySelector">Eine Funktion die den Schlüssel zum sortieren auswählt.</param>
        ///// <param name="comparer">Ein <see cref="IComparer{TKey}"/> der eine benutzerdefinierte Sortierung durchführt.<para/>
        ///// Sollte <c>null</c> übergeben werden, wird der Standartvergleich angewendet.</param>
        //public static void Sort<TSource, TKey>(this ObservableCollection<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        //{
        //    TSource[] sortedList;//Ein Array, damit die Elemente durch source.Clear nicht gelöscht werden
        //    if (comparer == null)
        //        sortedList = source.OrderBy(keySelector).ToArray();
        //    else
        //        sortedList = source.OrderBy(keySelector, comparer).ToArray();
        //    source.Clear();
        //    foreach (var item in sortedList)
        //        source.Add(item);
        //}

    }


    public class GroupFilter
    {
        private List<Predicate<object>> _filters;

        public Predicate<object> Filter { get; private set; }

        public GroupFilter()
        {
            _filters = new List<Predicate<object>>();
            Filter = InternalFilter;
        }

        private bool InternalFilter(object o)
        {
            foreach (var filter in _filters)
            {
                if (!filter(o))
                {
                    return false;
                }
            }

            return true;
        }

        public void AddFilter(Predicate<object> filter)
        {
            _filters.Add(filter);
        }

        public void RemoveFilter(Predicate<object> filter)
        {
            if (_filters.Contains(filter))
            {
                _filters.Remove(filter);
            }
        }
    }

    public static class Helper
    {
        public static ObservableCollection<T> ToObservableCollection<T>
             (this IEnumerable<T> en)
        {
            var ob = new ObservableCollection<T>(en);
            return ob;
        }
    }

}
