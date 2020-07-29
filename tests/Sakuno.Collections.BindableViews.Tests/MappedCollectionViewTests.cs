using System.Collections.ObjectModel;
using Xunit;

namespace Sakuno.Collections.BindableViews.Tests
{
    public class MappedCollectionViewTests
    {
        [Fact]
        public void SimpleCollection()
        {
            var source = new[] { 2, 1, 5 };
            var mapped = new MappedCollectionView<int, int>(source, r => r * 2);

            Assert.Equal(3, mapped.Count);
            Assert.Equal(new[] { 4, 2, 10 }, mapped);
        }

        [Fact]
        public void ObservableCollection_Add()
        {
            var source = new ObservableCollection<int>() { 5 };
            var mapped = new MappedCollectionView<int, int>(source, r => r * 2);

            Assert.Single(mapped);
            Assert.Equal(new[] { 10 }, mapped);

            source.Add(1);

            Assert.Equal(2, mapped.Count);
            Assert.Equal(new[] { 10, 2 }, mapped);

            source.Insert(0, 9);

            Assert.Equal(3, mapped.Count);
            Assert.Equal(new[] { 18, 10, 2 }, mapped);
        }

        [Fact]
        public void ObservableCollection_Remove()
        {
            var source = new ObservableCollection<int>() { 5, 2, 8, 9 };
            var mapped = new MappedCollectionView<int, int>(source, r => r * 2);

            Assert.Equal(4, mapped.Count);
            Assert.Equal(new[] { 10, 4, 16, 18 }, mapped);

            source.Remove(8);

            Assert.Equal(3, mapped.Count);
            Assert.Equal(new[] { 10, 4, 18 }, mapped);
        }

        [Fact]
        public void ObservableCollection_Replace()
        {
            var source = new ObservableCollection<int>() { 5, 2, 8, 9 };
            var mapped = new MappedCollectionView<int, int>(source, r => r * 2);

            Assert.Equal(4, mapped.Count);
            Assert.Equal(new[] { 10, 4, 16, 18 }, mapped);

            source[1] = 7;

            Assert.Equal(4, mapped.Count);
            Assert.Equal(new[] { 10, 14, 16, 18 }, mapped);
        }

        [Fact]
        public void ObservableCollection_Clear()
        {
            var source = new ObservableCollection<int>() { 5, 2, 8, 9 };
            var mapped = new MappedCollectionView<int, int>(source, r => r * 2);

            Assert.Equal(4, mapped.Count);
            Assert.Equal(new[] { 10, 4, 16, 18 }, mapped);

            source.Clear();

            Assert.Empty(mapped);
        }
    }
}
