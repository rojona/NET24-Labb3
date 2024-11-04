namespace NET24_Labb3.ViewModel;

internal class ConfigVM : VMBase
{
    private readonly MainWindowVM? _mainWindowVm;

    public ConfigVM(MainWindowVM? mainWindowVm)
    {
        _mainWindowVm = mainWindowVm;
    }
    
    public QuestionPackVM? ActivePack => _mainWindowVm.ActivePack;
}