using Events.AvaloniaApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Events.AvaloniaApp;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection services)
    {
        services.AddSingleton<Events.AvaloniaApp.IRepository, Events.AvaloniaApp.EventJsonRepository>();
        // services.AddTransient<IBusinessService, BusinessService>();
        services.AddTransient<MainWindowViewModel>();
    }
}