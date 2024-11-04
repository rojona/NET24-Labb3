using System.CodeDom;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;
using NET24_Labb3.Command;
using NET24_Labb3.Model;

namespace NET24_Labb3.ViewModel;

internal class PlayerVM : VMBase, IDisposable
{
    private readonly MainWindowVM? _mainWindowVm;
    private readonly DispatcherTimer _timer;
    private Random _random = new Random();
    private Random _startRandom = new Random();

    private List<Question>? _questions;
    private int _currentQuestionIndex;
    private int _timeLeft;
    private bool _canAnswer = true;
    private bool _showResults;
    private int _correctAnswers;
    private bool _disposed;

    public void Dispose()
    {
        if (!_disposed)
        {
            _timer.Stop();
            _timer.Tick -= Timer_Tick;
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    public void Cleanup()
    {
        _timer.Stop();
        ShowResults = false;
        GameInProgress = false;
        ShuffledAnswers.Clear();
        _currentQuestionIndex = 0;
        CorrectAnswers = 0;
    
        RaisePropertyChanged(nameof(ShowResults));
        RaisePropertyChanged(nameof(GameInProgress));
        RaisePropertyChanged(nameof(CurrentQuestionNumber));
        RaisePropertyChanged(nameof(TotalQuestions));
    }
    
    public PlayerVM(MainWindowVM mainWindowVm)
    {
        _mainWindowVm = mainWindowVm;
        _random = new Random(DateTime.Now.Millisecond);
        
        _timer = new DispatcherTimer();
        _timer.Tick += Timer_Tick;
        _timer.Interval = TimeSpan.FromSeconds(1);

        AnswerCommand = new DelCom(ExecuteAnswer, CanExecuteAnswer);
        ExitCommand = new DelCom(ExecuteExit, CanExecuteExit);
        
        ShuffledAnswers = new ObservableCollection<AnswerViewModel>();
    }

    public Question? CurrentQuestion => _questions?[_currentQuestionIndex];
    
    public int CurrentQuestionNumber => _currentQuestionIndex + 1;
    
    public int TotalQuestions => _questions?.Count ?? 0;
    
    public int TimeLeft
    {
        get => _timeLeft;
        private set
        {
            _timeLeft = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(TimeLeftColor));
        }
    }

    public Brush TimeLeftColor => TimeLeft <= 3 ? Brushes.Red : Brushes.White;

    public bool CanAnswer
    {
        get => _canAnswer;
        private set
        {
            _canAnswer = value;
            RaisePropertyChanged();
        }
    }

    public bool ShowResults
    {
        get => _showResults;
        private set
        {
            _showResults = value;
            RaisePropertyChanged();
        }
    }

    public int CorrectAnswers
    {
        get => _correctAnswers;
        private set
        {
            _correctAnswers = value;
            RaisePropertyChanged();
        }
    }

    public ObservableCollection<AnswerViewModel> ShuffledAnswers { get; }
    
    public DelCom AnswerCommand { get; }
    public DelCom ExitCommand { get; }
    
    public bool GameInProgress { get; private set; }

    public void StartQuiz()
    {
        _startRandom = new Random();
        
        if (_mainWindowVm?.ActivePack == null || !_mainWindowVm.ActivePack.Questions.Any())
        {
            MessageBox.Show("No questions available to start the quiz.",
                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            _timer.Stop();
            ShuffledAnswers.Clear();
            _questions = new List<Question>(_mainWindowVm.ActivePack.Questions)
                .OrderBy(x => _startRandom.Next()).ToList();
            _currentQuestionIndex = 0;
            CorrectAnswers = 0;
            ShowResults = false;
            GameInProgress = true;

            RaisePropertyChanged(nameof(GameInProgress));
            LoadCurrentQuestion();
        }
        catch (Exception ex)
        {
            GameInProgress = false;
            RaisePropertyChanged(nameof(GameInProgress));
            MessageBox.Show($"Error starting quiz: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadCurrentQuestion()
    {
        if (_questions == null || _currentQuestionIndex >= _questions.Count)
        {
            ShowResults = true;
            return;
        }

        TimeLeft = _mainWindowVm.ActivePack?.TimeLimitInSeconds ?? 10;
        CanAnswer = true;

        var currentQuestion = _questions[_currentQuestionIndex];
        ShuffledAnswers.Clear();

        var allAnswers = new List<AnswerViewModel>
        {
            new AnswerViewModel { Text = currentQuestion.CorrectAnswer },
            new AnswerViewModel { Text = currentQuestion.IncorrectAnswers[0] },
            new AnswerViewModel { Text = currentQuestion.IncorrectAnswers[1] },
            new AnswerViewModel { Text = currentQuestion.IncorrectAnswers[2] }
        };

        for (int i = allAnswers.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (allAnswers[i], allAnswers[j]) = (allAnswers[j], allAnswers[i]);
        }

        foreach (var answer in allAnswers)
        {
            ShuffledAnswers.Add(answer);
        }

        _timer.Start();
        
        RaisePropertyChanged(nameof(CurrentQuestion));
        RaisePropertyChanged(nameof(CurrentQuestionNumber));
        RaisePropertyChanged(nameof(TotalQuestions));
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        TimeLeft--;
        
        if (TimeLeft <= 0)
        {
            HandleTimeout();
        }
    }

    private void HandleTimeout()
    {
        _timer.Stop();
        CanAnswer = false;
        
        foreach (var answer in ShuffledAnswers)
        {
            answer.ShowIcon = true;
            answer.Icon = answer.Text == CurrentQuestion?.CorrectAnswer ? 
                IconType.Check : IconType.Times;
            answer.IconColor = answer.Text == CurrentQuestion?.CorrectAnswer ? 
                Brushes.Green : Brushes.Red;
        }

        Task.Delay(2000).ContinueWith(_ => Application.Current.Dispatcher.Invoke(NextQuestion));
    }

    private void ExecuteAnswer(object parameter)
    {
        if (parameter is not AnswerViewModel answer || CurrentQuestion == null) return;
        
        _timer.Stop();
        CanAnswer = false;

        var isCorrect = answer.Text == CurrentQuestion.CorrectAnswer;
        if (isCorrect) CorrectAnswers++;

        foreach (var ans in ShuffledAnswers)
        {
            ans.ShowIcon = true;

            if (ans == answer)
            {
                ans.IsSelected = (ans == answer);
                ans.Icon = ans.Text == CurrentQuestion.CorrectAnswer ? 
                    IconType.Check : IconType.Times;
                ans.IconColor = ans.Text == CurrentQuestion.CorrectAnswer ? 
                    Brushes.Green : Brushes.Red;
            }

            if (ans.Text == CurrentQuestion.CorrectAnswer)
            {
                ans.IsCorrectAnswer = (ans.Text == CurrentQuestion.CorrectAnswer);
                ans.Icon = IconType.Check;
                ans.IconColor = Brushes.Green;
            }
        }

        Task.Delay(2000).ContinueWith(_ => Application.Current.Dispatcher.Invoke(NextQuestion));
    }

    private bool CanExecuteAnswer(object parameter) => CanAnswer;

    private void NextQuestion()
    {
        _currentQuestionIndex++;
        
        if (_currentQuestionIndex >= (_questions?.Count ?? 0))
        {
            ShowResults = true;
            return;
        }
        
        LoadCurrentQuestion();
    }

    private void ExecuteExit(object parameter)
    {
        Cleanup();
        if (_mainWindowVm != null)
        {
            _mainWindowVm.IsInConfigMode = true;
        }
    }

    private bool CanExecuteExit(object parameter) => true;
}