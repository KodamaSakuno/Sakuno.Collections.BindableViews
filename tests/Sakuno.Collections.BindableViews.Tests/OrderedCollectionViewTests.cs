using Xunit;

namespace Sakuno.Collections.BindableViews.Tests
{
    public class OrderedCollectionViewTests
    {
        [Fact]
        public void SimpleCollections()
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
    }
}
