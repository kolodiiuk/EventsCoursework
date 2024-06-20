using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Events.Models;
using Events.Utilities;

namespace Events.FileAccess;

public class EventsJsonEventDataProvider : IEventDataProvider
{
    private string _filePath;
    private List<Event> _events;
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public EventsJsonEventDataProvider(string filePath = null)
    {
        if (filePath != null && !Path.IsPathRooted(filePath))
        {
            throw new ArgumentException("File path must be absolute.", nameof(filePath));
        }

        var pathToEventStorage = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
            "events-storage");

        if (!Directory.Exists(pathToEventStorage))
        {
            Directory.CreateDirectory(pathToEventStorage);
        }

        _filePath = filePath ?? Path.Combine(pathToEventStorage, $"Events.json");
        EnsureFileExists();
        _events = ReadAllEventsFromFile().Value;
    }

    private void EnsureFileExists()
    {
        if (!File.Exists(_filePath))
        {
            WriteToFileAsync(new List<Event>()).GetAwaiter().GetResult();
        }
    }

    public Result AddEvent(Event @event)
    {
        if (@event == null) return Result.Fail("Event is null.");
        _events.Add(@event);
        
        return Result.Success();
    }

    public Result<IEnumerable<Event>> GetEventListByCondition(Func<Event, bool> condition)
    {
        var filteredEvents = _events.Where(condition);

        return (filteredEvents == null)
            ? Result.Fail<IEnumerable<Event>>("Events not found.")
            : Result.Success<IEnumerable<Event>>(filteredEvents);
    }

    public Result UpdateEvent(Event @event)
    {
        var eventToUpdate = _events.SingleOrDefault(e => e.Id == @event.Id);

        if (eventToUpdate == null) return Result.Fail("Event not found.");

        eventToUpdate.Id = @event.Id;
        eventToUpdate.Name = @event.Name;
        eventToUpdate.DateTime = @event.DateTime;
        eventToUpdate.Duration = @event.Duration;
        eventToUpdate.Location = @event.Location;
        eventToUpdate.Category = @event.Category;
        eventToUpdate.Description = @event.Description;
        eventToUpdate.Done = @event.Done;

        return Result.Success();
    }

    public Result DeleteEvent(Guid id)
    {
        var eventToDelete = _events.SingleOrDefault(e => e.Id == id);
        if (eventToDelete == null) return Result.Fail("Event not found.");
        _events.Remove(eventToDelete);

        return Result.Success();
    }
    
    public Result<List<Event>> GetAllEvents()
    {
        return Result.Success(_events);
    }

    public async Task<Result> SubmitChanges()
    {
        try
        {
            using var stream = File.Create(_filePath);
            await JsonSerializer.SerializeAsync(stream, _events, _options);
            
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }
    
    private Result<List<Event>> ReadAllEventsFromFile()
    {
        try
        {
            using var stream = File.OpenRead(_filePath);
            if (stream.Length == 0)
            {
                return Result.Success<List<Event>>(new List<Event>());
            }
            else
            {
                var events = JsonSerializer.Deserialize<List<Event>>(stream, _options) 
                             ?? new List<Event>();
                return Result.Success<List<Event>>(events);
            }
        }
        catch (Exception ex)
        {
            return Result.Fail<List<Event>>(ex.Message);
        }
    }
    
    public async Task WriteToFileAsync(List<Event> items) 
    {
        using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, items, _options);
    }
}