using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NET24_Labb3.Model;

public class Question : INotifyPropertyChanged
{
    private string _query = string.Empty;
    private string _correctAnswer = string.Empty;
    private string[] _incorrectAnswers;

    public Question()
    {
        _incorrectAnswers = new string[3];
    }

    public Question(string query, string correctAnswer, 
        string incorrectAnswer1, string incorrectAnswer2, string incorrectAnswer3)
    {
        _query = query;
        _correctAnswer = correctAnswer;
        _incorrectAnswers = new string[3] { incorrectAnswer1, incorrectAnswer2, incorrectAnswer3 };
    }
    
    public string Query 
    { 
        get => _query;
        set
        {
            _query = value;
            OnPropertyChanged();
        }
    }
    
    public string CorrectAnswer 
    { 
        get => _correctAnswer;
        set
        {
            _correctAnswer = value;
            OnPropertyChanged();
        }
    }
    
    public string[] IncorrectAnswers 
    { 
        get => _incorrectAnswers;
        set
        {
            _incorrectAnswers = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}