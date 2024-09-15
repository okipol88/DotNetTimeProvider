namespace DotNetTimeProvider.Tests;

[TestClass]
public class TestCollectingData
{
    [TestMethod]
    public async Task OneObjectCollectedInFirstAndSecondInterval_TriggersTwoCallbacksWithTwoOneElementLists()
    {
        // Arrange
        var firstItem = 42;
        var secondItem = 43;
        var timeSpan = TimeSpan.FromSeconds(1);
        var timeSpanSafetyDelta = TimeSpan.FromMilliseconds(100);

        var dataCollector = new DataCollector<int>();
        var cancellationTokenSource = new CancellationTokenSource();
        var itemCollections = new Queue<IList<int>>();
        dataCollector.DataCollected += itemCollections.Enqueue;

        // Act
        dataCollector.StartCollecting(timeSpan, cancellationTokenSource.Token);

        dataCollector.Collectdata(firstItem);
        await Task.Delay(timeSpan + timeSpanSafetyDelta);

        dataCollector.Collectdata(secondItem);
        await Task.Delay(timeSpan + timeSpanSafetyDelta);

        cancellationTokenSource.Cancel();

        // Assert
        Assert.AreEqual(2, itemCollections.Count);

        Assert.AreEqual(1, itemCollections.ElementAt(0).Count);
        Assert.AreEqual(1, itemCollections.ElementAt(1).Count);

        Assert.AreEqual(firstItem, itemCollections.ElementAt(0).Single());
        Assert.AreEqual(secondItem, itemCollections.ElementAt(1).Single());
    }
}