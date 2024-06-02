using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Events.Models;

public class EventJsonRepository : IRepository //todo: adjust result
{
    private readonly string _filePath;

    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };

    public EventJsonRepository(string filePath = null)
    {
        _filePath = filePath ?? Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, $"Events.json");

        EnsureFileExists();
    }

    private void EnsureFileExists()
    {
        if (!File.Exists(_filePath))
        {
            WriteToFileAsync(new List<Event>()).GetAwaiter().GetResult();
        }
    }
    
    public async Task<Result> AddAsync(Event @event)
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

    public async Task<Result<Event>> GetSingleByConditionAsync(Func<Event, bool> condition)
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
    
    public async Task<Result<IEnumerable<Event>>> GetListByConditionAsync(
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

    public async Task<Result> UpdateAsync(Event @event, Func<Event, bool> condition)
    {
        try
        {
            var events = await ReadFromFileAsync();
            var eventToUpdate = events.Where(condition).FirstOrDefault();

            if (eventToUpdate == null)
            {
                return Result.Fail("Event not found.");
            }
            
            events.Remove(eventToUpdate);
            events.Add(@event);
            await WriteToFileAsync(events);
            
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            return Result.Fail("Error updating event.");
        }
    }

    public async Task<Result> DeleteAsync(Func<Event, bool> condition)
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
        var items = await JsonSerializer.DeserializeAsync<List<Event>>(stream, _options) ?? new List<Event>();
            
        return items;
    }

    private async Task WriteToFileAsync(List<Event> items) //todo: make it return Result?
    {
        using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, items, _options);
    }
}