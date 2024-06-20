   using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Events.Models;
using Events.Utilities;
using Events.FileAccess;

[TestFixture]
public class EventsJsonEventDataProviderTests
{
    private EventsJsonEventDataProvider _dataProvider;
    private string _filePath;

    [SetUp]
    public void SetUp()
    {
        _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "events-storage", "Events-test.json");
        _dataProvider = new EventsJsonEventDataProvider(_filePath);
    }

    [Test]
    public void AddEvent_InvalidEvent_ReturnsFail()
    {
        Event newEvent = null;

        var result = _dataProvider.AddEvent(newEvent);

        Assert.IsFalse(result.IsSuccess);
    }

    [Test]
    public void AddEvent_ValidEvent_AddsEventToList()
    {
        var newEvent = new Event { Name = "Event name", Id = Guid.NewGuid() };

        _dataProvider.AddEvent(newEvent);

        var result = _dataProvider.GetEventListByCondition(e => e.Id == newEvent.Id);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(newEvent.Id, result.Value.First().Id);
    }

    [Test]
    public void UpdateEvent_PassedEventInTheList_UpdatesExistingEvent()
    {
        var newEvent = new Event { Name = "Event name", Id = Guid.NewGuid() };
        _dataProvider.AddEvent(newEvent);
        
        var existingEvent = _dataProvider.GetEventListByCondition(e => true).Value.First();
        existingEvent.Name = "Updated Event";

        var updateResult = _dataProvider.UpdateEvent(existingEvent);

        Assert.IsTrue(updateResult.IsSuccess);

        var updatedEvent = _dataProvider.GetEventListByCondition(e => e.Id == existingEvent.Id).Value.First();

        Assert.AreEqual("Updated Event", updatedEvent.Name);
    }

    [Test]
    public void UpdateEvent_PassedEventNotInTheList_ReturnsFail()
    {
        var nonExistingEvent = new Event { Id = Guid.NewGuid(), Name = "Non-existing Event" };

        var updateResult = _dataProvider.UpdateEvent(nonExistingEvent);

        Assert.IsFalse(updateResult.IsSuccess);
    }

    [Test]
    public void DeleteEvent_EventInTheList_RemovesEventFromList()
    {
        var newEvent = new Event { Name = "Event name", Id = Guid.NewGuid() };
        _dataProvider.AddEvent(newEvent);
        var existingEvent = _dataProvider.GetEventListByCondition(e => true).Value.First();

        var deleteResult = _dataProvider.DeleteEvent(existingEvent.Id);

        Assert.IsTrue(deleteResult.IsSuccess);

        var deletedEvent = _dataProvider.GetEventListByCondition(e => e.Id == existingEvent.Id);

        Assert.IsTrue(deletedEvent.Value.Count() == 0);
    }

    [Test]
    public void DeleteEvent_NoEventWithPassedId_ReturnsFail()
    {
        var nonExistingEventId = Guid.NewGuid();

        var deleteResult = _dataProvider.DeleteEvent(nonExistingEventId);

        Assert.IsFalse(deleteResult.IsSuccess);
    }

    [Test]
    public void GetEventListByCondition_NoEventsSatisfyingCondition_ReturnsEmptyList()
    {
        var result = _dataProvider.GetEventListByCondition(e => e.Name == "Non-existing Event");

        Assert.IsFalse(result.IsSuccess || result.Value.Any());
    }

    [Test]
    public void GetEventListByCondition_EventsSatisfyingConditionAreInTheList_ReturnsEvents()
    {
        var newEvent = new Event { Id = Guid.NewGuid(), Name = "Test Event" };
        _dataProvider.AddEvent(newEvent);

        var result = _dataProvider.GetEventListByCondition(e => e.Name == "Test Event");

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(newEvent.Id, result.Value.First().Id);
    }
} 