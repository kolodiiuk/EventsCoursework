﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Events.Models;

public class Event : INotifyPropertyChanged
{
    private bool _isSelected;
    private string _name;
    private DateTime _dateTime;
    private TimeSpan _duration;
    private string _location;
    private string _category;
    private string _description;

    public Event(
        string name,
        DateTime dateTime,
        TimeSpan duration,
        string location,
        string category,
        string description)
    {
        Name = name;
        DateTime = dateTime;
        Duration = duration;
        Location = location;
        Category = category;
        Description = description;
    }

    public Guid Id { get; private set; }

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

    public DateTime DateTime
    {
        get => _dateTime;
        set
        {
            if (value.Equals(_dateTime)) return;
            _dateTime = value;
            OnPropertyChanged();
        }
    }

    public TimeSpan Duration
    {
        get => _duration;
        set
        {
            if (value.Equals(_duration)) return;
            _duration = value;
            OnPropertyChanged();
        }
    }

    public string Location
    {
        get => _location;
        set
        {
            if (value == _location) return;
            _location = value;
            OnPropertyChanged();
        }
    }

    public string Category
    {
        get => _category;
        set
        {
            if (value == _category) return;
            _category = value;
            OnPropertyChanged();
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (value == _description) return;
            _description = value;
            OnPropertyChanged();
        }
    }

    public bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }

    public bool AreIntersected(Event other)
    {
        return DateTime.Date == other.DateTime.Date &&
               DateTime.TimeOfDay < other.DateTime.TimeOfDay + other.Duration &&
               DateTime.TimeOfDay + Duration > other.DateTime.TimeOfDay;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}