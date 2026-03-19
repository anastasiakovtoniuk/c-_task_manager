using System.Windows;
using System.Windows.Navigation;

namespace StudyManager.WpfApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainFrame.Navigated += MainFrame_Navigated;
    }

    public void Navigate(object page) => MainFrame.Navigate(page);

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (MainFrame.CanGoBack) MainFrame.GoBack();
    }

    private void MainFrame_Navigated(object? sender, NavigationEventArgs e)
    {
        BackButton.IsEnabled = MainFrame.CanGoBack;
    }
}