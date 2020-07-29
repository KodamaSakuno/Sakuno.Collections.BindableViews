using System.Collections.ObjectModel;
using Xunit;

namespace Sakuno.Collections.BindableViews.Tests
{
    public class OrderedCollectionViewTests
    {
        [Fact]
        public void SimpleCollection()
        {
            var source = new[] { 5, 1, 9, 2, 4, 10, 8 };
            var ordered = new OrderedCollectionView<int>(source);

            Assert.Equal(7, ordered.Count);
            Assert.Equal(new[] { 1, 2, 4, 5, 8, 9, 10 }, ordered);

            Assert.Equal(4, ordered[2]);
            Assert.Equal(9, ordered[5]);
            Assert.Equal(10, ordered[6]);

            Assert.Equal(2, ordered.IndexOf(4));
            Assert.Equal(5, ordered.IndexOf(9));
            Assert.Equal(6, ordered.IndexOf(10));
            Assert.True(ordered.Contains(4));
            Assert.True(ordered.Contains(9));
            Assert.True(ordered.Contains(1));

            Assert.Equal(-1, ordered.IndexOf(-1));
            Assert.False(ordered.Contains(-1));
        }

        [Fact]
        public void ObservableCollection_Add()
        {
            var source = new ObservableCollection<int>() { 5, 1, 1 };
            var ordered = new OrderedCollectionView<int>(source);

            Assert.Equal(3, ordered.Count);
            Assert.Equal(new[] { 1, 1, 5 }, ordered);

            source.Add(2);

            Assert.Equal(4, ordered.Count);
            Assert.Equal(new[] { 1, 1, 2, 5 }, ordered);
        }

        [Fact]
        public void ObservableCollection_Remove()
        {
            var source = new ObservableCollection<int>() { 5, 1, 8, 4 };
            var ordered = new OrderedCollectionView<int>(source);

            Assert.Equal(4, ordered.Count);
            Assert.Equal(new[] { 1, 4, 5, 8 }, ordered);

            source.Remove(4);

            Assert.Equal(3, ordered.Count);
            Assert.Equal(new[] { 1, 5, 8 }, ordered);
        }

        [Fact]
        public void ObservableCollection_Replace()
        {
            var source = new ObservableCollection<int>() { 5, 1, 8, 4 };
            var ordered = new OrderedCollectionView<int>(source);

            Assert.Equal(4, ordered.Count);
            Assert.Equal(new[] { 1, 4, 5, 8 }, ordered);

            source[3] = 9;

            Assert.Equal(new[] { 1, 5, 8, 9 }, ordered);
        }

        [Fact]
        public void ObservableCollection_Clear()
        {
            var source = new ObservableCollection<int>() { 3, 1, 2 };
            var ordered = new OrderedCollectionView<int>(source);

            Assert.Equal(3, ordered.Count);
            Assert.Equal(new[] { 1, 2, 3 }, ordered);

            source.Clear();

            Assert.Empty(ordered);
        }
    }
}
