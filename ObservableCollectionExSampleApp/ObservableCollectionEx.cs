using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace ObservableCollectionExSampleApp;

public class ObservableCollectionEx<T> : ObservableCollection<T>
{
    private readonly static System.ComponentModel.PropertyChangedEventArgs ItemsINPC = new System.ComponentModel.PropertyChangedEventArgs("Item[]");
    private readonly static System.ComponentModel.PropertyChangedEventArgs CountINPC = new System.ComponentModel.PropertyChangedEventArgs(nameof(Count));

    public void AddRange(IEnumerable<T> collection)
    {
        CheckReentrancy();

        int count = 0;
        foreach (var item in collection)
        {
            count++;
            base.Items.Add(item); // Editing this protected list won't raise events
        }

        if(count > 0)
        {
            OnPropertyChanged(ItemsINPC);
            OnPropertyChanged(CountINPC);
            if (count > 1) // Multi-item change
            {
                OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        action: NotifyCollectionChangedAction.Add,
                        changedItems: collection as System.Collections.IList ?? collection.ToList(),
                        startingIndex: Count - count));
            }
            else // count == 1 - single-item change
            {
                OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                        action: NotifyCollectionChangedAction.Add,
                        changedItem: collection.First(),
                        index: Count - 1));
            }
        }
    }
}
