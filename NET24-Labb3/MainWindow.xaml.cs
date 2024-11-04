using System.ComponentModel;
using System.Windows;
using NET24_Labb3.Model;
using NET24_Labb3.ViewModel;

namespace NET24_Labb3;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>

public partial class MainWindow : Window
{
    private readonly MainWindowVM _viewModel;
    
    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainWindowVM();
        DataContext = _viewModel;

        Closing += MainWindow_Closing;
    }

    private async void MainWindow_Closing(object sender, CancelEventArgs e)
    {
        try
        {
            await _viewModel.SavePacksAsync();
        }
        catch (Exception exception)
        {
            MessageBox.Show($"Error saving data: {exception.Message}",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}