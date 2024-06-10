using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Events.Models;
using Events.ViewModels;
using Events.Views;

namespace Events;

public partial class App : Application
{
    // private readonly EventJsonEventRepository _repo = new EventJsonEventRepository();
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var repository = new EventRepository();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(repository),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}