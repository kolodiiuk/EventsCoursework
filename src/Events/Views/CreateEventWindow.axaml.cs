using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Events.Models;
using Events.ViewModels;

namespace Events.Views;

public partial class CreateEventWindow : Window
{
    public CreateEventWindow(MainWindowViewModel mainWindowViewModel, IEventRepository repository)
    {
        InitializeComponent();
        DataContext = new CreateEventWindowViewModel(mainWindowViewModel, repository);
    }

    public void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
