using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace Sakuno.Collections.BindableViews.Tests
{
    public class ConcatenatedCollectionViewTests
    {
        [Fact]
        public void SimpleCollections()
        {
            var sources = new[]
            {
                new[] { 1, 2, 3 },
                new[] { 99 },
                new[] { 88, 9, 10 },
            };
            var concatenated = new ConcatenatedCollectionView<int>(sources);

            Assert.Equal(7, concatenated.Count);
            Assert.Equal(new[] { 1, 2, 3, 99, 88, 9, 10 }, concatenated);

            Assert.Equal(2, concatenated[1]);
            Assert.Equal(99, concatenated[3]);
            Assert.Equal(10, concatenated[6]);

            Assert.Equal(1, concatenated.IndexOf(2));
            Assert.Equal(3, concatenated.IndexOf(99));
            Assert.Equal(6, concatenated.IndexOf(10));
            Assert.True(concatenated.Contains(2));
            Assert.True(concatenated.Contains(99));
            Assert.True(concatenated.Contains(10));

            Assert.Equal(-1, concatenated.IndexOf(-1));
            Assert.False(concatenated.Contains(-1));
        }

        [Fact]
        public void OuterObservableCollections_Add()
        {
            var source = new ObservableCollection<int[]>()
            {
                new[] { 5 },
                new[] { 2, 3 },
            };
            var concatenated = new ConcatenatedCollectionView<int>(source);

            Assert.Equal(3, concatenated.Count);
            Assert.Equal(new[] { 5, 2, 3 }, concatenated);

            source.Add(new[] { 4, 5, 6 });

            Assert.Equal(6, concatenated.Count);
            Assert.Equal(new[] { 5, 2, 3, 4, 5, 6 }, concatenated);
        }

        [Fact]
        public void InnerObservableCollections_Add()
        {
            var a = new ObservableCollection<int>();
            var b = new ObservableCollection<int>();
            var concatenated = new ConcatenatedCollectionView<int>(new[] { a, b });

            Assert.Empty(concatenated);

            a.Add(1);
            b.Add(2);
            a.Add(3);

            Assert.Equal(3, concatenated.Count);
            Assert.Equal(new[] { 1, 3, 2 }, concatenated);
        }

        [Fact]
        public void InnerDuplicatedObservableCollections_Add()
        {
            var source = new ObservableCollection<int>();
            var concatenated = new ConcatenatedCollectionView<int>(new[] { source, source });

            Assert.Empty(concatenated);

            source.Add(1);
            source.Add(3);

            Assert.Equal(4, concatenated.Count);
            Assert.Equal(new[] { 1, 3, 1, 3 }, concatenated);
        }

        [Fact]
        public void BothObservableCollections_Add()
        {
            var outer = new ObservableCollection<ObservableCollection<int>>();
            var inner = new ObservableCollection<int>();
            var concatenated = new ConcatenatedCollectionView<int>(outer);

            Assert.Empty(concatenated);

            outer.Add(inner);
            Assert.Empty(concatenated);

            inner.Add(1);
            inner.Add(3);
            inner.Add(2);

            Assert.Equal(3, concatenated.Count);
            Assert.Equal(new[] { 1, 3, 2 }, concatenated);
        }

        [Fact]
        public void OuterObservableCollections_Remove()
        {
            var source = new ObservableCollection<int[]>()
            {
                new[] { 1 },
                new[] { 4, 5, 6 },
                new[] { 3, 2 },
            };
            var concatenated = new ConcatenatedCollectionView<int>(source);

            Assert.Equal(6, concatenated.Count);
            Assert.Equal(new[] { 1, 4, 5, 6, 3, 2}, concatenated);

            source.RemoveAt(1);

            Assert.Equal(3, concatenated.Count);
            Assert.Equal(new[] { 1, 3, 2 }, concatenated);
        }

        [Fact]
        public void InnerObservableCollections_Remove()
        {
            var a = new ObservableCollection<int>() { 1, 3, 2 };
            var b = new ObservableCollection<int>() { 4 };
            var c = new ObservableCollection<int>() { 9, 8 };
            var concatenated = new ConcatenatedCollectionView<int>(new[] { a, b, c });

            Assert.Equal(6, concatenated.Count);
            Assert.Equal(new[] { 1, 3, 2, 4, 9, 8 }, concatenated);

            a.Remove(3);
            b.Remove(4);

            Assert.Equal(4, concatenated.Count);
            Assert.Equal(new[] { 1, 2, 9, 8 }, concatenated);
        }

        [Fact]
        public void InnerDuplicatedObservableCollections_Remove()
        {
            var source = new ObservableCollection<int>() { 1, 3, 2 };
            var concatenated = new ConcatenatedCollectionView<int>(new[] { source, source });

            Assert.Equal(6, concatenated.Count);
            Assert.Equal(new[] { 1, 3, 2, 1, 3, 2 }, concatenated);

            source.Remove(3);

            Assert.Equal(4, concatenated.Count);
            Assert.Equal(new[] { 1, 2, 1, 2 }, concatenated);
        }

        [Fact]
        public void BothObservableCollections_Remove()
        {
            var inner = new ObservableCollection<int>() { 1, 5, 2, 4 };
            var outer = new ObservableCollection<ObservableCollection<int>>() { inner };
            var concatenated = new ConcatenatedCollectionView<int>(outer);

            Assert.Equal(new[] { 1, 5, 2, 4 }, concatenated);

            outer.Remove(inner);

            Assert.Empty(concatenated);

            inner.Remove(2);

            Assert.Empty(concatenated);
        }

        [Fact]
        public void OuterObservableCollections_Replace()
        {
            var source = new ObservableCollection<int[]>()
            {
                new[] { 2, 3 },
                new[] { 1 },
                new[] { 4, 5, 6 },
            };
            var concatenated = new ConcatenatedCollectionView<int>(source);

            Assert.Equal(6, concatenated.Count);
            Assert.Equal(new[] { 2, 3, 1, 4, 5, 6 }, concatenated);

            source[1] = new[] { 7, 8, 9, 10 };

            Assert.Equal(9, concatenated.Count);
            Assert.Equal(new[] { 2, 3, 7, 8, 9, 10, 4, 5, 6 }, concatenated);
        }

        [Fact]
        public void InnerObservableCollections_Replace()
        {
            var a = new ObservableCollection<int>() { 1, 3, 2 };
            var b = new ObservableCollection<int>() { 4, 5, 6 };
            var c = new ObservableCollection<int>() { 7, 8 };
            var concatenated = new ConcatenatedCollectionView<int>(new[] { a, b, c });

            Assert.Equal(8, concatenated.Count);
            Assert.Equal(new[] { 1, 3, 2, 4, 5, 6, 7, 8 }, concatenated);

            c[0] = 9;

            Assert.Equal(8, concatenated.Count);
            Assert.Equal(new[] { 1, 3, 2, 4, 5, 6, 9, 8 }, concatenated);
        }

        [Fact]
        public void InnerDuplicatedObservableCollections_Replace()
        {
            var source = new ObservableCollection<int>() { 1, 3, 2 };
            var concatenated = new ConcatenatedCollectionView<int>(new[] { source, source });

            Assert.Equal(6, concatenated.Count);
            Assert.Equal(new[] { 1, 3, 2, 1, 3, 2 }, concatenated);

            source[0] = -1;

            Assert.Equal(6, concatenated.Count);
            Assert.Equal(new[] { -1, 3, 2, -1, 3, 2 }, concatenated);
        }

        [Fact]
        public void OuterObservableCollections_Clear()
        {
            var source = new ObservableCollection<int[]>()
            {
                new[] { 1 },
                new[] { 2, 3 },
            };
            var concatenated = new ConcatenatedCollectionView<int>(source);

            Assert.Equal(3, concatenated.Count);
            Assert.Equal(new[] { 1, 2, 3 }, concatenated);

            source.Clear();

            Assert.Empty(concatenated);
        }

        [Fact]
        public void InnerObservableCollections_Clear()
        {
            var a = new ObservableCollection<int>() { 1, 2, 3 };
            var b = new ObservableCollection<int>() { 4, 5, 6 };
            var c = new ObservableCollection<int>() { 7, 8 };
            var concatenated = new ConcatenatedCollectionView<int>(new[] { a, b, c });

            Assert.Equal(8, concatenated.Count);
            Assert.Equal(Enumerable.Range(1, 8), concatenated);

            b.Clear();

            Assert.Equal(5, concatenated.Count);
            Assert.Equal(new[] { 1, 2, 3, 7, 8 }, concatenated);
        }

        [Fact]
        public void InnerDuplicatedObservableCollections_Clear()
        {
            var source = new ObservableCollection<int>() { 1, 2, 3 };
            var concatenated = new ConcatenatedCollectionView<int>(new[] { source, source });

            Assert.Equal(6, concatenated.Count);
            Assert.Equal(new[] { 1, 2, 3, 1, 2, 3 }, concatenated);

            source.Clear();

            Assert.Empty(concatenated);
        }
    }
}
