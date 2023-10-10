using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace ObservableCollectionExSampleApp;

public class ObservableCollectionEx<T> : ObservableCollection<T>
{
    public void AddRange(IEnumerable<T> collection)
    {
        CheckReentrancy();

        if (collection.Any() is false)
        {
            return;
        }

        List<T> itemsList = (List<T>)Items;
        itemsList.AddRange(collection);

        OnCollectionChanged(
            new NotifyCollectionChangedEventArgs(
                action: NotifyCollectionChangedAction.Add,
                changedItem: itemsList.Last(),
                index: itemsList.Count - 1));
    }
}
