using System.Collections.ObjectModel;
using System.Windows.Controls;
using Xunit;

namespace Sakuno.Collections.BindableViews.Tests
{
    public class FilteredCollectionViewWpfTests
    {
        [WpfFact]
        public void SimpleCollection()
        {
            var source = new[] { 2, 1, 4, 6, 8, 0, 7 };
            var itemsControl = new ItemsControl() { ItemsSource = new FilteredCollectionView<int>(source, r => r > 3) };

            Assert.Equal<object>(new[] { 4, 6, 8, 7 }, itemsControl.Items);
        }

        [WpfFact]
        public void ObservableCollection_Add()
        {
            var source = new ObservableCollection<int>() { 5 };
            var itemsControl = new ItemsControl() { ItemsSource = new FilteredCollectionView<int>(source, r => r >= 5) };

            Assert.Equal<object>(new[] { 5 }, itemsControl.Items);

            source.Add(1);

            Assert.Equal<object>(new[] { 5 }, itemsControl.Items);

            source.Add(7);

            Assert.Equal<object>(new[] { 5, 7 }, itemsControl.Items);

            source.Insert(0, 9);

            Assert.Equal<object>(new[] { 9, 5, 7 }, itemsControl.Items);
        }

        [WpfFact]
        public void ObservableCollection_Remove()
        {
            var source = new ObservableCollection<int>() { 5, 2, 8, 9 };
            var itemsControl = new ItemsControl() { ItemsSource = new FilteredCollectionView<int>(source, r => r >= 5) };

            Assert.Equal<object>(new[] { 5, 8, 9 }, itemsControl.Items);

            source.Remove(8);

            Assert.Equal<object>(new[] { 5, 9 }, itemsControl.Items);

            source.Remove(2);

            Assert.Equal<object>(new[] { 5, 9 }, itemsControl.Items);
        }

        [WpfFact]
        public void ObservableCollection_Replace()
        {
            var source = new ObservableCollection<int>() { 5, 2, 8, 9 };
            var itemsControl = new ItemsControl() { ItemsSource = new FilteredCollectionView<int>(source, r => r >= 5) };

            Assert.Equal<object>(new[] { 5, 8, 9 }, itemsControl.Items);

            source[1] = 7;

            Assert.Equal<object>(new[] { 5, 7, 8, 9 }, itemsControl.Items);

            source[2] = 1;

            Assert.Equal<object>(new[] { 5, 7, 9 }, itemsControl.Items);
        }

        [WpfFact]
        public void ObservableCollection_Clear()
        {
            var source = new ObservableCollection<int>() { 5, 2, 8, 9 };
            var itemsControl = new ItemsControl() { ItemsSource = new FilteredCollectionView<int>(source, r => r >= 5) };

            Assert.Equal<object>(new[] { 5, 8, 9 }, itemsControl.Items);

            source.Clear();

            Assert.Empty(itemsControl.Items);
        }
    }
}
