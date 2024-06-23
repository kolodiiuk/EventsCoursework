using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Events.Models;

public class Statistics
{
    private List<Event> _events;
    private string? _stringRepresentation;
    
    public Statistics(List<Event> events) => _events = events;
    
    public int TotalNumberOfEvents => _events.Count;

    public int NumberOfCompletedEvents => _events.Count(e => e.Done == true);

    public int NumberOfUpcomingEvents => _events.Count(e => e.DateTime > DateTime.Now);

    public int NumberOfPastEvents => _events.Count(e => e.DateTime < DateTime.Now);

    public Dictionary<string, int> EventDistributionByCategory =>
        _events
            .GroupBy(e => e.Category)
            .ToDictionary(g => g.Key ?? "Unknown", g => g.Count());

    public Dictionary<string, int> EventsByLocation =>
        _events
            .GroupBy(e => e.Location)
            .ToDictionary(g => g.Key ?? "Unknown", g => g.Count());

    public Dictionary<DayOfWeek, int> EventDistributionByDayOfWeek
    {
        get
        {
            Dictionary<DayOfWeek, int> map = new Dictionary<DayOfWeek, int>();
            foreach (var e in _events)
            {
                if (e.DateTime.HasValue)
                {
                    if (!map.ContainsKey(e.DateTime.Value.DayOfWeek))
                    {
                        map[e.DateTime.Value.DayOfWeek] = 0;
                    }

                    map[e.DateTime.Value.DayOfWeek]++;
                }
            }

            return map;
        }
    }
    
    public string? StringRepresentation
    {
        get => new StringBuilder()
            .AppendLine($"Total number of events: {TotalNumberOfEvents}")
            .AppendLine($"Number of completed events: {NumberOfCompletedEvents}")
            .AppendLine($"Number of upcoming events: {NumberOfUpcomingEvents}")
            .AppendLine($"Number of past events: {NumberOfPastEvents} {Environment.NewLine}")
            .AppendLine(
                $"Event distribution by category: {Environment.NewLine}{EventDistributionToString<string>(EventDistributionByCategory)}")
            .AppendLine(
                $"Events by location: {Environment.NewLine}{EventDistributionToString<string>(EventsByLocation)}")
            .AppendLine(
                $"Event distribution by day of week: {Environment.NewLine}{EventDistributionToString<DayOfWeek>(EventDistributionByDayOfWeek)}")
            .ToString();
    }
    
    private string EventDistributionToString<TKey>(Dictionary<TKey, int> eventDistribution)
    {
        var sb = new StringBuilder();
        foreach (var kvp in eventDistribution)
        {
            sb.AppendLine($"{kvp.Key}: {kvp.Value}");
        }
        
        return sb.ToString();
    }
}
