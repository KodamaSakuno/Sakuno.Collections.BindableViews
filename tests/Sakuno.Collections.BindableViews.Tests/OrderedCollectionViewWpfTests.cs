using System.Windows.Controls;
using Xunit;

namespace Sakuno.Collections.BindableViews.Tests
{
    public class OrderedCollectionViewWpfTests
    {
        [WpfFact]
        public void SimpleCollections()
        {
            var source = new[] { 5, 1, 9, 2, 4, 10, 8 };
            var itemsControl = new ItemsControl() { ItemsSource = new OrderedCollectionView<int>(source) };

            Assert.Equal<object>(new[] { 1, 2, 4, 5, 8, 9, 10 }, itemsControl.Items);
        }
    }
}
