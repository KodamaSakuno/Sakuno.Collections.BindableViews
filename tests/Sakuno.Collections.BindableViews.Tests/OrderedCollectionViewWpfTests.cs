using System.Collections.ObjectModel;
using System.Windows.Controls;
using Xunit;

namespace Sakuno.Collections.BindableViews.Tests
{
    public class OrderedCollectionViewWpfTests
    {
        [WpfFact]
        public void SimpleCollection()
        {
            var source = new[] { 5, 1, 9, 2, 4, 10, 8 };
            var itemsControl = new ItemsControl() { ItemsSource = new OrderedCollectionView<int>(source) };

            Assert.Equal<object>(new[] { 1, 2, 4, 5, 8, 9, 10 }, itemsControl.Items);
        }

        [WpfFact]
        public void ObservableCollection_Add()
        {
            var source = new ObservableCollection<int>() { 5, 1, 1 };
            var itemsControl = new ItemsControl() { ItemsSource = new OrderedCollectionView<int>(source) };

            Assert.Equal<object>(new[] { 1, 1, 5 }, itemsControl.Items);

            source.Add(2);

            Assert.Equal<object>(new[] { 1, 1, 2, 5 }, itemsControl.Items);
        }

        [WpfFact]
        public void ObservableCollection_Remove()
        {
            var source = new ObservableCollection<int>() { 5, 1, 8, 4 };
            var itemsControl = new ItemsControl() { ItemsSource = new OrderedCollectionView<int>(source) };

            Assert.Equal<object>(new[] { 1, 4, 5, 8 }, itemsControl.Items);

            source.Remove(4);

            Assert.Equal<object>(new[] { 1, 5, 8 }, itemsControl.Items);
        }

        [WpfFact]
        public void ObservableCollection_Replace()
        {
            var source = new ObservableCollection<int>() { 5, 1, 8, 4 };
            var itemsControl = new ItemsControl() { ItemsSource = new OrderedCollectionView<int>(source) };

            Assert.Equal<object>(new[] { 1, 4, 5, 8 }, itemsControl.Items);

            source[3] = 9;

            Assert.Equal<object>(new[] { 1, 5, 8, 9 }, itemsControl.Items);
        }

        [WpfFact]
        public void ObservableCollection_Clear()
        {
            var source = new ObservableCollection<int>() { 3, 1, 2 };
            var itemsControl = new ItemsControl() { ItemsSource = new OrderedCollectionView<int>(source) };

            Assert.Equal<object>(new[] { 1, 2, 3 }, itemsControl.Items);

            source.Clear();

            Assert.Empty(itemsControl.Items);
        }
    }
}
