using Avalonia.Controls;
using Events.AvaloniaApp.ViewModels;

namespace Events.AvaloniaApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(new EventJsonRepository());
    }
}