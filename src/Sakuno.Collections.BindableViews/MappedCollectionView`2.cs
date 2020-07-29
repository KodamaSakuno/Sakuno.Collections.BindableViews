using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Sakuno.Collections.BindableViews
{
    public sealed partial class MappedCollectionView<TSource, TDestination> : DisposableObject, IReadOnlyList<TDestination>, IList, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private readonly IReadOnlyList<TSource> _source;
        private readonly Func<TSource, TDestination> _mapper;

        private readonly List<TSource> _sourceSnapshot;
        private readonly List<TDestination> _destination;

        public int Count => _destination.Count;

        public TDestination this[int index] => _destination[index];

        public event PropertyChangedEventHandler? PropertyChanged;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public MappedCollectionView(IReadOnlyList<TSource> source, Func<TSource, TDestination>? mapper)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            _sourceSnapshot = new List<TSource>(_source.Count + 4);
            _sourceSnapshot.AddRange(_source);

            _destination = new List<TDestination>(_source.Count + 4);
            ProjectFromSource();

            if (_source is INotifyCollectionChanged sourceCollectionChanged)
                sourceCollectionChanged.CollectionChanged += OnSourceCollectionChanged;
            else
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
                            var newSourceItem = (TSource)e.NewItems[i];
                            var newItem = _mapper(newSourceItem);

                            _sourceSnapshot.Insert(e.NewStartingIndex + i, newSourceItem);
                            _destination.Insert(e.NewStartingIndex + i, newItem);

                            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, e.NewStartingIndex + i));
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        for (var i = 0; i < e.OldItems.Count; i++)
                        {
                            var oldItem = _destination[e.OldStartingIndex];

                            _sourceSnapshot.RemoveAt(e.OldStartingIndex);
                            _destination.RemoveAt(e.OldStartingIndex);

                            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, e.OldStartingIndex));
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    {
                        var newSourceItem = (TSource)e.NewItems[0];

                        var oldItem = _destination[e.OldStartingIndex];
                        var newItem = _mapper(newSourceItem);

                        _sourceSnapshot[e.OldStartingIndex] = newSourceItem;
                        _destination[e.OldStartingIndex] = newItem;

                        NotifyCollectionItemChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, e.NewStartingIndex));
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    var movedItem = _destination[e.OldStartingIndex];
                    var movedItemOfSource = _sourceSnapshot[e.OldStartingIndex];

                    _destination.RemoveAt(e.OldStartingIndex);
                    _sourceSnapshot.RemoveAt(e.OldStartingIndex);

                    _destination.Insert(e.NewStartingIndex, movedItem);
                    _sourceSnapshot.Insert(e.NewStartingIndex, movedItemOfSource);

                    NotifyCollectionItemChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, movedItem, e.NewStartingIndex, e.OldStartingIndex));
                    break;

                case NotifyCollectionChangedAction.Reset:
                    _destination.Clear();
                    _sourceSnapshot.Clear();

                    if (_source.Count > 0)
                    {
                        _sourceSnapshot.AddRange(_source);
                        ProjectFromSource();
                    }

                    NotifyCollectionChanged(KnownEventArgs.CollectionReset);
                    break;
            }
        }

        private void ProjectFromSource()
        {
            for (var i = 0; i < _source.Count; i++)
                _destination.Insert(i, _mapper(_source[i]));
        }

        public int IndexOf(TDestination item) => _destination.IndexOf(item);
        public bool Contains(TDestination item) => IndexOf(item) != -1;

        public List<TDestination>.Enumerator GetEnumerator() => _destination.GetEnumerator();

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
        }
    }
}
