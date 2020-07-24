using System.Collections.Specialized;
using System.ComponentModel;

namespace Sakuno.Collections.BindableViews
{
    static class KnownEventArgs
    {
        public static readonly PropertyChangedEventArgs CountPropertyChanged = new PropertyChangedEventArgs("Count");
        public static readonly PropertyChangedEventArgs IndexerPropertyChanged = new PropertyChangedEventArgs("Item[]");

        public static readonly NotifyCollectionChangedEventArgs CollectionReset =
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
    }
}
