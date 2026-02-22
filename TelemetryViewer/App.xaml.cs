using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TelemetryViewer.Services;

namespace TelemetryViewer;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var gamePath = SettingsService.LoadGamePath();

        if (gamePath is null)
        {
            gamePath = PromptForPath(null);
            if (gamePath is null)
            {
                Shutdown();
                return;
            }
        }

        SettingsService.SaveGamePath(gamePath);

        var mainWindow = new MainWindow(gamePath);
        mainWindow.Show();
    }

    private static string? PromptForPath(string? previousPath)
    {
        var dialog = new Window
        {
            Title = "TD Sim Dashboard — Setup",
            Width = 520,
            Height = 210,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize,
            Background = new SolidColorBrush(Color.FromRgb(13, 17, 23)),
            FontFamily = new FontFamily("Segoe UI"),
            FontSize = 13
        };

        var panel = new StackPanel { Margin = new Thickness(24, 20, 24, 20) };

        var header = new TextBlock
        {
            Text = "Game Folder Path",
            FontSize = 15,
            FontWeight = FontWeights.SemiBold,
            Foreground = new SolidColorBrush(Color.FromRgb(230, 237, 243)),
            Margin = new Thickness(0, 0, 0, 4)
        };

        var subtitle = new TextBlock
        {
            Text = "Enter the root path to your TD-Game folder (e.g. C:\\Users\\you\\TD-Game)",
            Foreground = new SolidColorBrush(Color.FromRgb(139, 148, 158)),
            FontSize = 11,
            Margin = new Thickness(0, 0, 0, 12)
        };

        var inputBox = new TextBox
        {
            Text = previousPath ?? "",
            Height = 32,
            FontSize = 13,
            Padding = new Thickness(8, 4, 8, 4),
            Background = new SolidColorBrush(Color.FromRgb(33, 38, 45)),
            Foreground = new SolidColorBrush(Color.FromRgb(201, 209, 217)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(48, 54, 61)),
            CaretBrush = new SolidColorBrush(Color.FromRgb(201, 209, 217)),
        };

        var errorLabel = new TextBlock
        {
            Text = "",
            Foreground = new SolidColorBrush(Color.FromRgb(248, 81, 73)),
            FontSize = 11,
            Margin = new Thickness(0, 6, 0, 0),
            Visibility = Visibility.Collapsed
        };

        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 14, 0, 0)
        };

        var okButton = new Button
        {
            Content = "Continue",
            Width = 90,
            Height = 30,
            FontWeight = FontWeights.SemiBold,
            Background = new SolidColorBrush(Color.FromRgb(35, 134, 54)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderThickness = new Thickness(0),
            Cursor = System.Windows.Input.Cursors.Hand,
            IsDefault = true
        };

        var cancelButton = new Button
        {
            Content = "Exit",
            Width = 70,
            Height = 30,
            Margin = new Thickness(0, 0, 8, 0),
            Background = new SolidColorBrush(Color.FromRgb(33, 38, 45)),
            Foreground = new SolidColorBrush(Color.FromRgb(201, 209, 217)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(48, 54, 61)),
            Cursor = System.Windows.Input.Cursors.Hand,
            IsCancel = true
        };

        string? result = null;

        okButton.Click += (_, _) =>
        {
            var path = inputBox.Text.Trim();
            if (!Directory.Exists(path))
            {
                errorLabel.Text = "Directory not found. Please enter a valid path.";
                errorLabel.Visibility = Visibility.Visible;
                return;
            }

            result = path;
            dialog.DialogResult = true;
            dialog.Close();
        };

        cancelButton.Click += (_, _) =>
        {
            dialog.DialogResult = false;
            dialog.Close();
        };

        buttonPanel.Children.Add(cancelButton);
        buttonPanel.Children.Add(okButton);

        panel.Children.Add(header);
        panel.Children.Add(subtitle);
        panel.Children.Add(inputBox);
        panel.Children.Add(errorLabel);
        panel.Children.Add(buttonPanel);

        dialog.Content = panel;
        dialog.ShowDialog();

        return result;
    }
}
