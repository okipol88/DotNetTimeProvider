using System;
using System.Collections.Concurrent;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DotNetTimeProvider;

public class DataCollector<T>
{
    private ITimer _timer;

    private ConcurrentStack<T> _items = new();

    public DataCollector(TimeProvider timerProvider)
    {
        _timer = timerProvider.CreateTimer(OnTimerTick, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }

    public Action<IList<T>>? DataCollected { get; set; }

    public void StartCollecting(TimeSpan interval, CancellationToken cancellationToken)
    {
        _timer.Change(interval, interval);

        cancellationToken.Register(() => _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan));
    }

    private void OnTimerTick(object? state)
    {
        var collectedItems = new T[_items.Count];

        _items.TryPopRange(collectedItems);

        System.Diagnostics.Debug.WriteLine($"Timer tick with {collectedItems.Length} item(s)");

        DataCollected?.Invoke(collectedItems);
    }

    public void Collectdata(T data)
    {
        System.Diagnostics.Debug.WriteLine($"Pushing data {data}");

        _items.Push(data);
    }
}