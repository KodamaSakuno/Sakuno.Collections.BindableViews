using System.Collections.ObjectModel;
using Xunit;

namespace Sakuno.Collections.BindableViews.Tests
{
    public class FilteredCollectionViewTests
    {
        [Fact]
        public void SimpleCollection()
        {
            var source = new[] { 2, 1, 4, 6, 8, 0, 7 };
            var filtered = new FilteredCollectionView<int>(source, r => r > 3);

            Assert.Equal(4, filtered.Count);
            Assert.Equal(new[] { 4, 6, 8, 7 }, filtered);
        }

        [Fact]
        public void ObservableCollection_Add()
        {
            var source = new ObservableCollection<int>() { 5 };
            var filtered = new FilteredCollectionView<int>(source, r => r >= 5);

            Assert.Single(filtered);
            Assert.Equal(new[] { 5 }, filtered);

            source.Add(1);

            Assert.Single(filtered);
            Assert.Equal(new[] { 5 }, filtered);

            source.Add(7);

            Assert.Equal(2, filtered.Count);
            Assert.Equal(new[] { 5, 7 }, filtered);

            source.Insert(0, 9);

            Assert.Equal(3, filtered.Count);
            Assert.Equal(new[] { 9, 5, 7 }, filtered);
        }

        [Fact]
        public void ObservableCollection_Remove()
        {
            var source = new ObservableCollection<int>() { 5, 2, 8, 9 };
            var filtered = new FilteredCollectionView<int>(source, r => r >= 5);

            Assert.Equal(3, filtered.Count);
            Assert.Equal(new[] { 5, 8, 9 }, filtered);

            source.Remove(8);

            Assert.Equal(2, filtered.Count);
            Assert.Equal(new[] { 5, 9 }, filtered);

            source.Remove(2);

            Assert.Equal(2, filtered.Count);
            Assert.Equal(new[] { 5, 9 }, filtered);
        }

        [Fact]
        public void ObservableCollection_Replace()
        {
            var source = new ObservableCollection<int>() { 5, 2, 8, 9 };
            var filtered = new FilteredCollectionView<int>(source, r => r >= 5);

            Assert.Equal(3, filtered.Count);
            Assert.Equal(new[] { 5, 8, 9 }, filtered);

            source[1] = 7;

            Assert.Equal(4, filtered.Count);
            Assert.Equal(new[] { 5, 7, 8, 9 }, filtered);

            source[2] = 1;

            Assert.Equal(3, filtered.Count);
            Assert.Equal(new[] { 5, 7, 9 }, filtered);
        }

        [Fact]
        public void ObservableCollection_Clear()
        {
            var source = new ObservableCollection<int>() { 5, 2, 8, 9 };
            var filtered = new FilteredCollectionView<int>(source, r => r >= 5);

            Assert.Equal(3, filtered.Count);
            Assert.Equal(new[] { 5, 8, 9 }, filtered);

            source.Clear();

            Assert.Empty(filtered);
        }
    }
}
