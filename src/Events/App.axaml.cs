using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Events.FileAccess;
using Events.Models;
using Events.ViewModels;
using Events.Views;

namespace Events;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(new EventsJsonEventDataProvider())
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
