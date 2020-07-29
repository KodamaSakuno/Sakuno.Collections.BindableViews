using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Xunit;

namespace Sakuno.Collections.BindableViews.Tests
{
    public class ConcatenatedCollectionViewWpfTests
    {
        [WpfFact]
        public void OuterObservableCollection_Add()
        {
            var source = new ObservableCollection<int[]>()
            {
                new[] { 5 },
                new[] { 2, 3 },
            };
            var itemsControl = new ItemsControl() { ItemsSource = new ConcatenatedCollectionView<int>(source) };

            Assert.Equal<object>(new[] { 5, 2, 3 }, itemsControl.Items);

            source.Add(new[] { 4, 5, 6 });

            Assert.Equal<object>(new[] { 5, 2, 3, 4, 5, 6 }, itemsControl.Items);
        }

        [WpfFact]
        public void InnerObservableCollection_Add()
        {
            var a = new ObservableCollection<int>();
            var b = new ObservableCollection<int>();
            var itemsControl = new ItemsControl() { ItemsSource = new ConcatenatedCollectionView<int>(new[] { a, b }) };

            Assert.Empty(itemsControl.Items);

            a.Add(1);
            b.Add(2);
            a.Add(3);

            Assert.Equal<object>(new[] { 1, 3, 2 }, itemsControl.Items);
        }

        [WpfFact]
        public void InnerDuplicatedObservableCollection_Add()
        {
            var source = new ObservableCollection<int>();
            var itemsControl = new ItemsControl() { ItemsSource = new ConcatenatedCollectionView<int>(new[] { source, source }) };

            Assert.Empty(itemsControl.Items);

            source.Add(1);
            source.Add(3);

            Assert.Equal<object>(new[] { 1, 3, 1, 3 }, itemsControl.Items);
        }

        [WpfFact]
        public void BothObservableCollection_Add()
        {
            var outer = new ObservableCollection<ObservableCollection<int>>();
            var inner = new ObservableCollection<int>();
            var itemsControl = new ItemsControl() { ItemsSource = new ConcatenatedCollectionView<int>(outer) };

            Assert.Empty(itemsControl.Items);

            outer.Add(inner);
            Assert.Empty(itemsControl.Items);

            inner.Add(1);
            inner.Add(3);
            inner.Add(2);

            Assert.Equal<object>(new[] { 1, 3, 2 }, itemsControl.Items);
        }

        [WpfFact]
        public void OuterObservableCollection_Remove()
        {
            var source = new ObservableCollection<int[]>()
            {
                new[] { 1 },
                new[] { 4, 5, 6 },
                new[] { 3, 2 },
            };
            var itemsControl = new ItemsControl() { ItemsSource = new ConcatenatedCollectionView<int>(source) };

            Assert.Equal<object>(new[] { 1, 4, 5, 6, 3, 2 }, itemsControl.Items);

            source.RemoveAt(1);

            Assert.Equal<object>(new[] { 1, 3, 2 }, itemsControl.Items);
        }

        [WpfFact]
        public void InnerObservableCollection_Remove()
        {
            var a = new ObservableCollection<int>() { 1, 3, 2 };
            var b = new ObservableCollection<int>() { 4 };
            var c = new ObservableCollection<int>() { 9, 8 };
            var itemsControl = new ItemsControl() { ItemsSource = new ConcatenatedCollectionView<int>(new[] { a, b, c }) };

            Assert.Equal<object>(new[] { 1, 3, 2, 4, 9, 8 }, itemsControl.Items);

            a.Remove(3);
            b.Remove(4);

            Assert.Equal<object>(new[] { 1, 2, 9, 8 }, itemsControl.Items);
        }

        [WpfFact]
        public void InnerDuplicatedObservableCollection_Remove()
        {
            var source = new ObservableCollection<int>() { 1, 3, 2 };
            var itemsControl = new ItemsControl() { ItemsSource = new ConcatenatedCollectionView<int>(new[] { source, source }) };

            Assert.Equal<object>(new[] { 1, 3, 2, 1, 3, 2 }, itemsControl.Items);

            source.Remove(3);

            Assert.Equal<object>(new[] { 1, 2, 1, 2 }, itemsControl.Items);
        }

        [WpfFact]
        public void BothObservableCollection_Remove()
        {
            var inner = new ObservableCollection<int>() { 1, 5, 2, 4 };
            var outer = new ObservableCollection<ObservableCollection<int>>() { inner };
            var itemsControl = new ItemsControl() { ItemsSource = new ConcatenatedCollectionView<int>(outer) };

            Assert.Equal<object>(new[] { 1, 5, 2, 4 }, itemsControl.Items);

            outer.Remove(inner);

            Assert.Empty(itemsControl.Items);

            inner.Remove(2);

            Assert.Empty(itemsControl.Items);
        }

        [WpfFact]
        public void OuterObservableCollection_Replace()
        {
            var source = new ObservableCollection<int[]>()
            {
                new[] { 2, 3 },
                new[] { 1 },
                new[] { 4, 5, 6 },
            };
            var itemsControl = new ItemsControl() { ItemsSource = new ConcatenatedCollectionView<int>(source) };

            Assert.Equal<object>(new[] { 2, 3, 1, 4, 5, 6 }, itemsControl.Items);

            source[1] = new[] { 7, 8, 9, 10 };

            Assert.Equal<object>(new[] { 2, 3, 7, 8, 9, 10, 4, 5, 6 }, itemsControl.Items);
        }

        [WpfFact]
        public void InnerObservableCollection_Replace()
        {
            var a = new ObservableCollection<int>() { 1, 3, 2 };
            var b = new ObservableCollection<int>() { 4, 5, 6 };
            var c = new ObservableCollection<int>() { 7, 8 };
            var itemsControl = new ItemsControl() { ItemsSource = new ConcatenatedCollectionView<int>(new[] { a, b, c }) };

            Assert.Equal<object>(new[] { 1, 3, 2, 4, 5, 6, 7, 8 }, itemsControl.Items);

            c[0] = 9;

            Assert.Equal<object>(new[] { 1, 3, 2, 4, 5, 6, 9, 8 }, itemsControl.Items);
        }

        [WpfFact]
        public void InnerDuplicatedObservableCollection_Replace()
        {
            var source = new ObservableCollection<int>() { 1, 3, 2 };
            var itemsControl = new ItemsControl() { ItemsSource = new ConcatenatedCollectionView<int>(new[] { source, source }) };

            Assert.Equal<object>(new[] { 1, 3, 2, 1, 3, 2 }, itemsControl.Items);

            source[0] = -1;

            Assert.Equal<object>(new[] { -1, 3, 2, -1, 3, 2 }, itemsControl.Items);
        }

        [WpfFact]
        public void OuterObservableCollection_Clear()
        {
            var source = new ObservableCollection<int[]>()
            {
                new[] { 1 },
                new[] { 2, 3 },
            };
            var itemsControl = new ItemsControl() { ItemsSource = new ConcatenatedCollectionView<int>(source) };

            Assert.Equal<object>(new[] { 1, 2, 3 }, itemsControl.Items);

            source.Clear();

            Assert.Empty(itemsControl.Items);
        }

        [WpfFact]
        public void InnerObservableCollection_Clear()
        {
            var a = new ObservableCollection<int>() { 1, 2, 3 };
            var b = new ObservableCollection<int>() { 4, 5, 6 };
            var c = new ObservableCollection<int>() { 7, 8 };
            var itemsControl = new ItemsControl() { ItemsSource = new ConcatenatedCollectionView<int>(new[] { a, b, c }) };

            Assert.Equal<object>(Enumerable.Range(1, 8), itemsControl.Items);

            b.Clear();

            Assert.Equal<object>(new[] { 1, 2, 3, 7, 8 }, itemsControl.Items);
        }

        [WpfFact]
        public void InnerDuplicatedObservableCollection_Clear()
        {
            var source = new ObservableCollection<int>() { 1, 2, 3 };
            var itemsControl = new ItemsControl() { ItemsSource = new ConcatenatedCollectionView<int>(new[] { source, source }) };

            Assert.Equal<object>(new[] { 1, 2, 3, 1, 2, 3 }, itemsControl.Items);

            source.Clear();

            Assert.Empty(itemsControl.Items);
        }
    }
}
