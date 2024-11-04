using System.Windows.Media;

namespace NET24_Labb3.ViewModel;

public enum IconType
{
    None,
    Check,
    Times
}

internal class AnswerViewModel : VMBase
{
    private string _text = "";
    private IconType _icon;
    private bool _showIcon;
    private Brush _iconColor = Brushes.Black;
    private bool _isSelected;
    private bool _isCorrectAnswer;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            RaisePropertyChanged();
        }
    }

    public bool IsCorrectAnswer
    {
        get => _isCorrectAnswer;
        set
        {
            _isCorrectAnswer = value;
            RaisePropertyChanged();
        }
    }

    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            RaisePropertyChanged();
        }
    }

    public IconType Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            RaisePropertyChanged();
        }
    }

    public bool ShowIcon
    {
        get => _showIcon;
        set
        {
            _showIcon = value;
            RaisePropertyChanged();
        }
    }

    public Brush IconColor
    {
        get => _iconColor;
        set
        {
            _iconColor = value;
            RaisePropertyChanged();
        }
    }
}