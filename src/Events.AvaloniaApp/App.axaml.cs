using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Events.AvaloniaApp.ViewModels;
using Events.AvaloniaApp.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Events.AvaloniaApp;
// a composition root
public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() //todo: remove usage of microsoft.extensions.dependencyinjection
    {
        BindingPlugins.DataValidators.RemoveAt(0); //todo: add pure di switch
        
        var collection = new ServiceCollection();
        collection.AddCommonServices();
        
        var services = collection.BuildServiceProvider();

        var vm = services.GetRequiredService<MainWindowViewModel>();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = vm,
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainWindow
            {
                DataContext = vm,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}