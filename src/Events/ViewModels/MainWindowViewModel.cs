// using AvaloniaControls.Models;

using System;
using System.Collections.ObjectModel;
using Events.Models;

namespace Events.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<Event> Events { get; set; }

    public MainWindowViewModel()
    {
        Events = new ObservableCollection<Event>()
        {
            new Event(
                "Event 1",
                DateTime.Now,
                TimeSpan.FromHours(1),
                "Location 1",
                Category.Business,
                "Description 1"),
            new Event(
                "Event 2",
                DateTime.Now,
                TimeSpan.FromHours(2),
                "Location 2",
                Category.Business,
                "Description 2"),
            new Event(
                "Event 3",
                DateTime.Now,
                TimeSpan.FromHours(3),
                "Location 3",
                Category.Business,
                "Description 3"),
            new Event(
                "Event 4",
                DateTime.Now,
                TimeSpan.FromHours(4),
                "Location 4",
                Category.Business,
                "Description 4"),
            new Event(
                "Event 5",
                DateTime.Now,
                TimeSpan.FromHours(5),
                "Location 5",
                Category.Business,
                "Description 5"),
            new Event(
                "Event 6",
                DateTime.Now,
                TimeSpan.FromHours(6),
                "Location 6",
                Category.Business,
                "Description 6"),
            new Event(
                "Event 7",
                DateTime.Now,
                TimeSpan.FromHours(7),
                "Location 7",
                Category.Business,
                "Description 7"),
            new Event(
                "Event 8",
                DateTime.Now,
                TimeSpan.FromHours(8),
                "Location 8",
                Category.Business,
                "Description 8"),
            new Event(
                "Event 9",
                DateTime.Now,
                TimeSpan.FromHours(9),
                "Location 9",
                Category.Business,
                "Description 9"),
            new Event(
                "Event 10",
                DateTime.Now,
                TimeSpan.FromHours(10),
                "Location 10",
                Category.Business,
                "Description 10"),
            new Event(
                "Event 11",
                DateTime.Now,
                TimeSpan.FromHours(11),
                "Location 11",
                Category.Business,
                "Description 11"),
            new Event(
                "Event 12",
                DateTime.Now,
                TimeSpan.FromHours(12),
                "Location 12",
                Category.Business,
                "Description 12"),
            new Event(
                "Event 13",
                DateTime.Now,
                TimeSpan.FromHours(13),
                "Location 13",
                Category.Business,
                "Description 13"),
        };
    }

}