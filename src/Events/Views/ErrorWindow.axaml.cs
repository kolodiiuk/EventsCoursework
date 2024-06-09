using Avalonia.Controls;

namespace Events.Views;

public partial class ErrorWindow : Window
{
    public ErrorWindow()
    {
        InitializeComponent();
        this.FindControl<Button>("CloseButton").Click += delegate { this.Close(); };
    }
}
