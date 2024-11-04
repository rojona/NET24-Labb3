using System.Windows.Controls;
using System.Windows.Input;

namespace NET24_Labb3.View;

public partial class ConfigView : UserControl
{
    public ConfigView()
    {
        InitializeComponent();
    }

    private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var vm = DataContext as ViewModel.ConfigVM;
        if (vm?.ActivePack?.EditQuestionCommand.CanExecute(null) == true)
        {
            vm.ActivePack.EditQuestionCommand.Execute(null);
        }
    }
}