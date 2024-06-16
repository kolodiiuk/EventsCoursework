using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Events.Models;
using Events.Utilities;

namespace Events.FileAccess;

public class EventRepository : IEventRepository
{
    private readonly string _filePath;

    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public EventRepository(string filePath = null)
    {
        var pathToEventStorage = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "events-storage");
        
        if (!Directory.Exists(pathToEventStorage))
        {
            Directory.CreateDirectory(pathToEventStorage);
        }
        
        _filePath = filePath ?? Path.Combine(pathToEventStorage, $"Events.json");

        EnsureFileExists();
    }

    private void EnsureFileExists()
    {
        if (!File.Exists(_filePath))
        {
            WriteToFileAsync(new List<Event>()).GetAwaiter().GetResult();
        }
    }

    public async Task<Result> AddEventAsync(Event @event)
    {
        try
        {
            var events = await ReadFromFileAsync();
            events.Add(@event);

            await WriteToFileAsync(events);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Fail("Error adding event.");
        }
    }

    public async Task<Result<Event>> GetSingleEventByConditionAsync(
        Func<Event, bool> condition)
    {
        try
        {
            var events = await ReadFromFileAsync();
            var @event = events.Where(condition).FirstOrDefault();

            if (@event != null)
            {
                return Result.Success<Event>(@event);
            }
            else
            {
                return Result.Fail<Event>("Event not found.");
            }
        }
        catch (Exception ex)
        {
            return Result.Fail<Event>("Error retrieving event.");
        }
    }

    public async Task<Result<IEnumerable<Event>>> GetEventListByConditionAsync(
        Func<Event, bool> condition)
    {
        try
        {
            var events = await ReadFromFileAsync();
            var filteredEvents = events.Where(condition);

            if (filteredEvents == null)
            {
                return Result.Fail<IEnumerable<Event>>("Events not found.");
            }
            else
            {
                return Result.Success<IEnumerable<Event>>(filteredEvents);
            }
        }
        catch (Exception ex)
        {
            return Result.Fail<IEnumerable<Event>>("Error retrieving events.");
        }
    }

    public async Task<Result> UpdateEventAsync(
        Event @event, Func<Event, bool> condition)
    {
        try
        {
            var events = await ReadFromFileAsync();
            var eventToUpdate = events.Where(condition).FirstOrDefault();

            if (eventToUpdate == null)
            {
                return Result.Fail("Event not found.");
            }

            eventToUpdate.Id = @event.Id;
            eventToUpdate.Name = @event.Name;
            eventToUpdate.DateTime = @event.DateTime;
            eventToUpdate.Duration = @event.Duration;
            eventToUpdate.Location = @event.Location;
            eventToUpdate.Category = @event.Category;
            eventToUpdate.Description = @event.Description;

            await WriteToFileAsync(events);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error updating event.");
            return Result.Fail("Error updating event.");
        }
    }

    public async Task<Result> DeleteEventAsync(Func<Event, bool> condition)
    {
        try
        {
            var events = await ReadFromFileAsync();
            var eventsToDelete = events.Where(condition).ToList();

            if (!eventsToDelete.Any())
            {
                return Result.Fail("Item not found.");
            }

            foreach (var @event in eventsToDelete)
            {
                events.Remove(@event);
            }

            await WriteToFileAsync(events);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Fail("Error deleting event.");
        }
    }

    private async Task<List<Event>> ReadFromFileAsync()
    {
        using var stream = File.OpenRead(_filePath);
        if (stream.Length == 0)
        {
            return new List<Event>();
        }
        else
        {
            var items = await JsonSerializer.DeserializeAsync<List<Event>>(stream, _options) ?? new List<Event>();
            return items;
        }
    }

    public async Task WriteToFileAsync(List<Event> items) 
    {
        using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, items, _options);
    }
}