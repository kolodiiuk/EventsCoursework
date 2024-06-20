using System;
using System.Collections.Generic;
using System.Linq;

namespace Events.Models;

public class Statistics
{
    private List<Event> _events;

    public Statistics(List<Event> events)
    {
        _events = events;
    }

    // Total number of events
    public int GetTotalNumberOfEvents()
    {
        return _events.Count;
    }

    // Number of completed events
    public int GetNumberOfCompletedEvents()
    {
        return _events.Count(e => e.Done == true);
    }

    // Number of upcoming events
    public int GetNumberOfUpcomingEvents()
    {
        return _events.Count(e => e.DateTime > DateTime.Now);
    }

    // Number of past events
    public int GetNumberOfPastEvents()
    {
        return _events.Count(e => e.DateTime < DateTime.Now);
    }

    // Event distribution by category
    public Dictionary<string, int> GetEventDistributionByCategory()
    {
        return _events
            .GroupBy(e => e.Category)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    // Event distribution by date
    public Dictionary<DateTime, int> GetEventDistributionByDate()
    {
        return _events
            .GroupBy(e => e.DateTime?.Date)
            .ToDictionary(g => g.Key ?? DateTime.MinValue, g => g.Count());
    }

    // Average event duration
    public TimeSpan GetAverageEventDuration()
    {
        return TimeSpan.FromTicks((long)_events.Average(e => e.Duration?.Ticks ?? 0));
    }

    // Longest and shortest event
    public (Event Longest, Event Shortest) GetLongestAndShortestEvents()
    {
        return (_events.OrderByDescending(e => e.Duration).First(), _events.OrderBy(e => e.Duration).First());
    }

    // Events by location
    public Dictionary<string, int> GetEventsByLocation()
    {
        return _events
            .GroupBy(e => e.Location)
            .ToDictionary(g => g.Key ?? "Unknown", g => g.Count());
    }

    // Number of events per day/week/month
    public (int PerDay, int PerWeek, int PerMonth) GetNumberOfEventsPerDayWeekMonth()
    {
        var today = DateTime.Today;
        var thisWeekStart = today.AddDays(-(int)today.DayOfWeek);
        var thisMonthStart = new DateTime(today.Year, today.Month, 1);

        return (_events.Count(e => e.DateTime?.Date == today),
                _events.Count(e => e.DateTime?.Date >= thisWeekStart),
                _events.Count(e => e.DateTime?.Date >= thisMonthStart));
    }
}
