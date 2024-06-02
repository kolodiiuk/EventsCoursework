using System;

namespace Events.Models;

public class Event
{
    // public int Id { get; private set; }
    public string Name { get; set; }
    public DateTime DateTime { get; set; }
    public TimeSpan Duration { get; set; }
    public string Location { get; set; }
    public Category Category { get; set; }
    public string Description { get; set; }

    public Event(
        string name,
        DateTime dateTime,
        TimeSpan duration,
        string location,
        Category category,
        string description)
    {
        Name = name;
        DateTime = dateTime;
        Duration = duration;
        Location = location;
        Category = category;
        Description = description;
    }
}