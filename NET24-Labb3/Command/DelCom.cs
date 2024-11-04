using System.Windows.Input;

namespace NET24_Labb3.Command;

internal class DelCom : ICommand
{
    private readonly Action<object> _execute;
    private readonly Func<object, bool> _canExecute;
    
    public event EventHandler? CanExecuteChanged;
    
    public DelCom(Action<object> execute, Func<object, bool> canExecute)
    {
        ArgumentNullException.ThrowIfNull(execute);
        _execute = execute;
        _canExecute = canExecute;
    }
    
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public bool CanExecute(object? parameter) => _canExecute is null ? true : _canExecute(parameter);

    public void Execute(object? parameter) => _execute(parameter);
}