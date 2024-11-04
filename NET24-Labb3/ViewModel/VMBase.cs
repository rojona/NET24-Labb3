using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NET24_Labb3.ViewModel;

public class VMBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public void RaisePropertyChanged([CallerMemberName]string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}