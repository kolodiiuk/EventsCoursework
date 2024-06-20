using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Events.Models;

public class Event : INotifyPropertyChanged
{
    private string _name;
    private DateTime? _dateTime;
    private TimeSpan? _duration;
    private string? _location;
    private string? _category;
    private string? _description;
    private bool? _done;

    public Event() {}
    
    public Event(string name, DateTime? dateTime, 
        TimeSpan? duration, string? location, string? category, 
        string? description, bool? done = false)
    {
        Name = name;
        DateTime = dateTime;
        Duration = duration;
        Location = location;
        Category = category;
        Description = description;
        Done = done;
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }
    
    public string Name
    {
        get => _name;
        set
        {
            if (value == _name) return;
            _name = value;
            OnPropertyChanged();
        }
    }

    public DateTime? DateTime
    {
        get => _dateTime;
        set
        {
            if (value.Equals(_dateTime)) return;
            _dateTime = value;
            OnPropertyChanged();
        }
    }

    public TimeSpan? Duration
    {
        get => _duration;
        set
        {
            if (value.Equals(_duration)) return;
            _duration = value;
            OnPropertyChanged();
        }
    }

    public string? Location
    {
        get => _location;
        set
        {
            if (value == _location) return;
            _location = value;
            OnPropertyChanged();
        }
    }

    public string? Category
    {
        get => _category;
        set
        {
            if (value == _category) return;
            _category = value;
            OnPropertyChanged();
        }
    }

    public string? Description
    {
        get => _description;
        set
        {
            if (value == _description) return;
            _description = value;
            OnPropertyChanged();
        }
    }

    public bool? Done
    {
        get => _done;
        set
        {
            if (value == _done) return;
            _done = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public override string ToString()
    {
        var date = DateTime != null ? DateTime?.ToString("dd/MM/yyyy") : "Not set";
        var time = DateTime != null ? DateTime?.ToString("HH:mm") : "Not set";
        var duration = Duration != null ? Duration?.ToString("g") : "Not set";
        var location = Location ?? "Not set";
        var category = Category ?? "Not set";
        var description = Description ?? "Not set";
        var done = Done != null ? (Done == true ? "Yes" : "No") : "Not set";
        
        return
            $"{nameof(Name)}: {Name} {Environment.NewLine}" +
            $"Date: {date} {Environment.NewLine}" +
            $"Time: {time} {Environment.NewLine}" +
            $"{nameof(Duration)}: {duration} {Environment.NewLine}" +
            $"{nameof(Location)}: {location} {Environment.NewLine}" +
            $"{nameof(Category)}: {category} {Environment.NewLine}" +
            $"{nameof(Description)}: {description} {Environment.NewLine}" +
            $"{nameof(Done)}: {done} {Environment.NewLine} ";
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
