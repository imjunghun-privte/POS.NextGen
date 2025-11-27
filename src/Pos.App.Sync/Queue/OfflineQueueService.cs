using Pos.App.Sync.Queue;

namespace Pos.App.Sync;

public class OfflineQueueService
{
    private readonly List<OfflineQueueItem> _items = new();

    public void Enqueue(OfflineQueueItem item)
    {
        _items.Add(item);
    }

    public async Task SyncAsync(Func<OfflineQueueItem, Task<bool>> sender, CancellationToken ct = default)
    {
        foreach (var item in _items.Where(x => !x.Sent))
        {
            if (ct.IsCancellationRequested)
                break;

            var ok = await sender(item);
            if (ok)
                item.Sent = true;
        }
    }
}
