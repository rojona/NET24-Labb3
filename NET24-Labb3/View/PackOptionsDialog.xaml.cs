// View/PackOptionsDialog.xaml.cs
using System.Windows;

namespace NET24_Labb3.View;

public partial class PackOptionsDialog : Window
{
    public PackOptionsDialog()
    {
        InitializeComponent();
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}