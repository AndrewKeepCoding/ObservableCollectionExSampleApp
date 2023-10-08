using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace ObservableCollectionExSampleApp;

public record Item(int Id);

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Item> _items = new();

    [ObservableProperty]
    private bool _useObservableCollectionEx = false;

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task AddItems(int itemsCount, CancellationToken cancellationToken)
    {
        try
        {
            Items = UseObservableCollectionEx is false
                ? new ObservableCollection<Item>()
                : new ObservableCollectionEx<Item>();
            using PeriodicTimer timer = new(period: TimeSpan.FromMilliseconds(100));

            while (await timer.WaitForNextTickAsync(cancellationToken) is true)
            {
                int startId = Items.Count + 1;
                IList<Item> newItems = await GetData(startId, itemsCount, cancellationToken);

                if (UseObservableCollectionEx is false)
                {
                    foreach (Item item in newItems)
                    {
                        Items.Add(item);
                    }
                }
                else
                {
                    ((ObservableCollectionEx<Item>)Items).AddRange(newItems);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private static async Task<List<Item>> GetData(int startId, int itemsCount, CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            List<Item> newItems = new();

            for (int i = 0; i < itemsCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                int id = startId + i;
                newItems.Add(new Item(id));
            }

            return newItems;
        });
    }
}

