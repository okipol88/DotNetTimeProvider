using Microsoft.Extensions.Time.Testing;

namespace DotNetTimeProvider.Tests;

[TestClass]
public class TestCollectingData
{
    [TestMethod]
    public void OneObjectCollectedInFirstAndSecondInterval_TriggersTwoCallbacksWithTwoOneElementLists()
    {
        // Arrange
        var firstItem = 42;
        var secondItem = 43;
        var timeSpan = TimeSpan.FromMicroseconds(1);
        var fakeTimeProvider = new FakeTimeProvider();

        var dataCollector = new DataCollector<int>(fakeTimeProvider);
        var cancellationTokenSource = new CancellationTokenSource();
        var itemCollections = new Queue<IList<int>>();
        dataCollector.DataCollected += itemCollections.Enqueue;

        // Act
        dataCollector.StartCollecting(timeSpan, cancellationTokenSource.Token);

        dataCollector.Collectdata(firstItem);
        fakeTimeProvider.Advance(timeSpan);

        dataCollector.Collectdata(secondItem);
        fakeTimeProvider.Advance(timeSpan);

        cancellationTokenSource.Cancel();

        // Assert
        Assert.AreEqual(2, itemCollections.Count);

        Assert.AreEqual(1, itemCollections.ElementAt(0).Count);
        Assert.AreEqual(1, itemCollections.ElementAt(1).Count);

        Assert.AreEqual(firstItem, itemCollections.ElementAt(0).Single());
        Assert.AreEqual(secondItem, itemCollections.ElementAt(1).Single());
    }
}