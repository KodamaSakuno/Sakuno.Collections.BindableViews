﻿using System.Collections.ObjectModel;
using System.Windows.Controls;
using Xunit;

namespace Sakuno.Collections.BindableViews.Tests
{
    public class MappedCollectionViewWpfTests
    {
        [WpfFact]
        public void SimpleCollection()
        {
            var source = new[] { 2, 1, 5 };
            var itemsControl = new ItemsControl() { ItemsSource = new MappedCollectionView<int, int>(source, r => r * 2) };

            Assert.Equal<object>(new[] { 4, 2, 10 }, itemsControl.Items);
        }

        [WpfFact]
        public void ObservableCollection_Add()
        {
            var source = new ObservableCollection<int>() { 5 };
            var itemsControl = new ItemsControl() { ItemsSource = new MappedCollectionView<int, int>(source, r => r * 2) };

            Assert.Equal<object>(new[] { 10 }, itemsControl.Items);

            source.Add(1);

            Assert.Equal<object>(new[] { 10, 2 }, itemsControl.Items);

            source.Insert(0, 9);

            Assert.Equal<object>(new[] { 18, 10, 2 }, itemsControl.Items);
        }

        [WpfFact]
        public void ObservableCollection_Remove()
        {
            var source = new ObservableCollection<int>() { 5, 2, 8, 9 };
            var itemsControl = new ItemsControl() { ItemsSource = new MappedCollectionView<int, int>(source, r => r * 2) };

            Assert.Equal<object>(new[] { 10, 4, 16, 18 }, itemsControl.Items);

            source.Remove(8);

            Assert.Equal<object>(new[] { 10, 4, 18 }, itemsControl.Items);
        }

        [WpfFact]
        public void ObservableCollection_Replace()
        {
            var source = new ObservableCollection<int>() { 5, 2, 8, 9 };
            var itemsControl = new ItemsControl() { ItemsSource = new MappedCollectionView<int, int>(source, r => r * 2) };

            Assert.Equal<object>(new[] { 10, 4, 16, 18 }, itemsControl.Items);

            source[1] = 7;

            Assert.Equal<object>(new[] { 10, 14, 16, 18 }, itemsControl.Items);
        }

        [WpfFact]
        public void ObservableCollection_Clear()
        {
            var source = new ObservableCollection<int>() { 5, 2, 8, 9 };
            var itemsControl = new ItemsControl() { ItemsSource = new MappedCollectionView<int, int>(source, r => r * 2) };

            Assert.Equal<object>(new[] { 10, 4, 16, 18 }, itemsControl.Items);

            source.Clear();

            Assert.Empty(itemsControl.Items);
        }
    }
}
