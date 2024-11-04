using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using NET24_Labb3.Command;
using NET24_Labb3.Model;
using NET24_Labb3.View;

namespace NET24_Labb3.ViewModel;

internal class QuestionPackVM : VMBase
{
    private readonly QuestionPack _model;
    private Question? _selectedQuestion;
    
    public QuestionPackVM(QuestionPack model)
    {
        _model = model;
        Questions = new ObservableCollection<Question>(model.Questions);
        Questions.CollectionChanged += Questions_CollectionChanged;

        AddQuestionCommand = new DelCom(ExecuteAddQuestion, CanExecuteAddQuestion);
        EditQuestionCommand = new DelCom(ExecuteEditQuestion, CanExecuteEditQuestion);
        RemoveQuestionCommand = new DelCom(ExecuteRemoveQuestion, CanExecuteRemoveQuestion);
    }

    private void Questions_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateModel();
        RaisePropertyChanged(nameof(Questions));
    }

    public string Name
    {
        get => _model.Name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                value = "New Question Pack";
            
            _model.Name = value;
            RaisePropertyChanged();
        } 
    }
    
    public Difficulty Difficulty
    {
        get => _model.Difficulty;
        set
        {
            _model.Difficulty = value;
            RaisePropertyChanged();
        }
    }

    public int TimeLimitInSeconds
    {
        get => _model.TimeLimitInSeconds;
        set
        {
            if (value < 5)
                value = 5;
            else if (value > 300)
                value = 300;
            
            _model.TimeLimitInSeconds = value;
            RaisePropertyChanged();
        }
    }
    
    public ObservableCollection<Question> Questions { get; }

    public Question? SelectedQuestion
    {
        get => _selectedQuestion;
        set
        {
            _selectedQuestion = value;
            RaisePropertyChanged();
            RemoveQuestionCommand.RaiseCanExecuteChanged();
            EditQuestionCommand.RaiseCanExecuteChanged();
        }
    }
    
    public DelCom AddQuestionCommand { get; }
    public DelCom RemoveQuestionCommand { get; }
    public DelCom EditQuestionCommand { get; }

    private void ExecuteAddQuestion(object obj)
    {
        var newQuestion = new Question
        {
            Query = "New Question",
            CorrectAnswer = "Correct Answer",
            IncorrectAnswers = new[] { "Wrong Answer 1", "Wrong Answer 2", "Wrong Answer 3" }
        };
        
        var dialog = new QuestionEditorDialog
        {
            Owner = Application.Current.MainWindow,
            DataContext = newQuestion
        };

        if (dialog.ShowDialog() == true)
        {
            Questions.Add(newQuestion);
            SelectedQuestion = newQuestion;
        }
    }

    private bool CanExecuteAddQuestion(object obj) => true;

    private void ExecuteEditQuestion(object obj)
    {
        if (SelectedQuestion == null) return;

        var questionCopy = new Question
        {
            Query = SelectedQuestion.Query,
            CorrectAnswer = SelectedQuestion.CorrectAnswer,
            IncorrectAnswers = SelectedQuestion.IncorrectAnswers.ToArray()
        };

        var dialog = new QuestionEditorDialog
        {
            Owner = Application.Current.MainWindow,
            DataContext = questionCopy 
        };

        if (dialog.ShowDialog() == true)
        {
            var index = Questions.IndexOf(SelectedQuestion);
            Questions.RemoveAt(index);
            Questions.Insert(index, questionCopy);
            SelectedQuestion = questionCopy;
        }
    }

    private bool CanExecuteEditQuestion(object obj) => SelectedQuestion != null;

    private void ExecuteRemoveQuestion(object obj)
    {
        if (SelectedQuestion == null) return;

        var result = MessageBox.Show(
            "Are you sure you want to delete this question?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            Questions.Remove(SelectedQuestion);
            SelectedQuestion = null;
        }
    }

    private bool CanExecuteRemoveQuestion(object obj) => SelectedQuestion != null;
    
    private void UpdateModel()
    {
        _model.Questions = Questions.ToList();
        _model.Name = Name;
        _model.Difficulty = Difficulty;
        _model.TimeLimitInSeconds = TimeLimitInSeconds;
    }
}