using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Sakuno.Collections.BindableViews
{
    public sealed partial class FilteredCollectionView<T> : DisposableObject, IReadOnlyList<T>, IList, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private readonly IReadOnlyList<T> _source;
        private readonly Predicate<T> _predicate;
        private readonly Predicate<string>? _shouldUpdate;

        private readonly List<T> _sourceSnapshot;
        private readonly List<int> _indexes;
        private readonly HashSet<INotifyPropertyChanged> _notifyPropertyChanged;

        public int Count => _indexes.Count;

        public T this[int index] => _sourceSnapshot[_indexes[index]];

        public event PropertyChangedEventHandler? PropertyChanged;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public FilteredCollectionView(IReadOnlyList<T> source, Predicate<T> predicate) : this(source, predicate, null) { }
        public FilteredCollectionView(IReadOnlyList<T> source, Predicate<T> predicate, Predicate<string>? shouldUpdate)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));

            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _shouldUpdate = shouldUpdate;

            _indexes = new List<int>(_source.Count + 4);
            _notifyPropertyChanged = new HashSet<INotifyPropertyChanged>();

            _sourceSnapshot = new List<T>(_source.Count + 4);
            _sourceSnapshot.AddRange(source);

            ProjectFromSource();

            if (_source is INotifyCollectionChanged sourceCollectionChanged)
                sourceCollectionChanged.CollectionChanged += OnSourceCollectionChanged;
            else if (shouldUpdate == null)
                GC.SuppressFinalize(this);
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        for (var i = 0; i < e.NewItems.Count; i++)
                        {
                            var newItem = (T)e.NewItems[i];
                            _sourceSnapshot.Insert(e.NewStartingIndex + i, newItem);

                            var index = _indexes.BinarySearch(e.NewStartingIndex + i).EnsurePositiveIndex();
                            for (var j = index; j < _indexes.Count; j++)
                                _indexes[j]++;

                            if (!_predicate(newItem))
                                continue;

                            _indexes.Insert(index, e.NewStartingIndex + i);

                            if (_shouldUpdate != null && newItem is INotifyPropertyChanged notifyPropertyChanged && _notifyPropertyChanged.Add(notifyPropertyChanged))
                                notifyPropertyChanged.PropertyChanged += OnItemPropertyChanged;

                            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, index));
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        var oldItems = new List<T>(e.OldItems.Count);
                        var startIndex = -1;

                        for (var i = 0; i < e.OldItems.Count; i++)
                        {
                            var oldItem = (T)e.OldItems[i];
                            if (_shouldUpdate != null && oldItem is INotifyPropertyChanged notifyPropertyChanged && _notifyPropertyChanged.Remove(notifyPropertyChanged))
                                notifyPropertyChanged.PropertyChanged -= OnItemPropertyChanged;

                            var index = _sourceSnapshot.IndexOf(oldItem);
                            _sourceSnapshot.Remove(oldItem);

                            index = _indexes.BinarySearch(index);

                            for (var j = index.EnsurePositiveIndex(); j < _indexes.Count; j++)
                                _indexes[j]--;

                            if (index < 0)
                                continue;

                            if (startIndex == -1)
                                startIndex = index;

                            _indexes.RemoveAt(index);
                            oldItems.Add(oldItem);

                            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        var oldItem = (T)e.OldItems[0];
                        var newItem = (T)e.NewItems[0];
                        var actualIndex = _indexes.BinarySearch(e.NewStartingIndex);

                        _sourceSnapshot[e.NewStartingIndex] = newItem;

                        if (_shouldUpdate != null)
                        {
                            if (oldItem is INotifyPropertyChanged oldNotifyPropertyChanged && _notifyPropertyChanged.Remove(oldNotifyPropertyChanged))
                                oldNotifyPropertyChanged.PropertyChanged -= OnItemPropertyChanged;

                            if (newItem is INotifyPropertyChanged newNotifyPropertyChanged && _notifyPropertyChanged.Add(newNotifyPropertyChanged))
                                newNotifyPropertyChanged.PropertyChanged += OnItemPropertyChanged;
                        }

                        var predication = _predicate(newItem);
                        if (actualIndex < 0 && !predication)
                            return;

                        if (actualIndex >= 0 && predication)
                        {
                            NotifyCollectionItemChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, actualIndex));
                            return;
                        }

                        if (actualIndex < 0)
                        {
                            actualIndex = ~actualIndex;
                            _indexes.Insert(actualIndex, e.NewStartingIndex);

                            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, actualIndex));
                        }
                        else
                        {
                            _indexes.RemoveAt(actualIndex);

                            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, actualIndex));
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException();

                case NotifyCollectionChangedAction.Reset:
                    foreach (var item in _notifyPropertyChanged)
                        item.PropertyChanged -= OnItemPropertyChanged;

                    _notifyPropertyChanged.Clear();
                    _indexes.Clear();
                    _sourceSnapshot.Clear();

                    if (_source.Count > 0)
                        ProjectFromSource();

                    NotifyCollectionChanged(KnownEventArgs.CollectionReset);
                    break;
            }
        }

        private void ProjectFromSource()
        {
            var destinationIndex = 0;

            for (var i = 0; i < _source.Count; i++)
            {
                var item = _source[i];

                if (_shouldUpdate != null && item is INotifyPropertyChanged notifyPropertyChanged && _notifyPropertyChanged.Add(notifyPropertyChanged))
                    notifyPropertyChanged.PropertyChanged += OnItemPropertyChanged;

                if (!_predicate(item))
                    continue;

                _indexes.Insert(destinationIndex++, i);
            }
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_shouldUpdate == null || !_shouldUpdate(e.PropertyName))
                return;

            var item = (T)sender;
            var index = _sourceSnapshot.IndexOf(item);
            var actualIndex = _indexes.BinarySearch(index);

            if (!(_predicate(item) ^ actualIndex >= 0))
                return;

            if (actualIndex < 0)
            {
                actualIndex = ~actualIndex;
                _indexes.Insert(actualIndex, index);

                NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, actualIndex));
            }
            else
            {
                _indexes.RemoveAt(actualIndex);

                NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, actualIndex));
            }
        }

        public int IndexOf(T item) => _sourceSnapshot.IndexOf(item);
        public bool Contains(T item) => IndexOf(item) != -1;

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var index in _indexes)
                yield return _sourceSnapshot[index];
        }

        private void NotifyCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, KnownEventArgs.CountPropertyChanged);
                propertyChanged(this, KnownEventArgs.IndexerPropertyChanged);
            }

            CollectionChanged?.Invoke(this, e);
        }
        private void NotifyCollectionItemChanged(NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, KnownEventArgs.IndexerPropertyChanged);
            CollectionChanged?.Invoke(this, e);
        }

        protected override void DisposeNativeResources()
        {
            if (_source is INotifyCollectionChanged sourceCollectionChanged)
                sourceCollectionChanged.CollectionChanged -= OnSourceCollectionChanged;

            foreach (var item in _notifyPropertyChanged)
                item.PropertyChanged -= OnItemPropertyChanged;
        }
    }
}
