using System.Windows;
using TelemetryViewer.ViewModels;

namespace TelemetryViewer;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;

    public MainWindow(string gamePath)
    {
        InitializeComponent();
        _vm = new MainViewModel(gamePath);
        DataContext = _vm;
    }

    protected override void OnClosed(EventArgs e)
    {
        _vm.Dispose();
        base.OnClosed(e);
    }
}
