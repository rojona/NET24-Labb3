using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using NET24_Labb3.Command;
using NET24_Labb3.Model;
using NET24_Labb3.Services;
using NET24_Labb3.View;

namespace NET24_Labb3.ViewModel;

internal class MainWindowVM : VMBase
{
    private readonly JsonFileHandler _jsonFileHandler;
    private QuestionPackVM? _activePack;
    private bool _isInConfigMode = true;
    
    public ObservableCollection<QuestionPackVM> Packs { get; set; }
    public ConfigVM ConfigVm { get; }
    public PlayerVM PlayerVm { get; }
    
    public QuestionPackVM? ActivePack
    {
        get => _activePack;
        set
        {
            _activePack = value;
            RaisePropertyChanged(); 
            ConfigVm.RaisePropertyChanged(nameof(ConfigVm.ActivePack));
        }
    }

    public bool IsInConfigMode
    {
        get => _isInConfigMode;
        set
        {
            _isInConfigMode = value;
            RaisePropertyChanged();
            UpdateCommandStates();
        }
    }

    public DelCom NewPackCommand { get; }
    public DelCom OpenPackOptionsCommand { get; }
    public DelCom SaveCommand { get; }
    public DelCom ImportQuestionsCommand { get; }
    public DelCom ToggleFullscreenCommand { get; }
    public DelCom ExitCommand { get; }
    public DelCom SwitchToConfigCommand { get; }
    public DelCom SwitchToPlayCommand { get; }
    
    public MainWindowVM()
    {
        _jsonFileHandler = new JsonFileHandler();
        ConfigVm = new ConfigVM(this);
        PlayerVm = new PlayerVM(this);
        Packs = new ObservableCollection<QuestionPackVM>();
        
        NewPackCommand = new DelCom(ExecuteNewPack, CanExecuteNewPack);
        OpenPackOptionsCommand = new DelCom(ExecuteOpenPackOptions, CanExecuteOpenPackOptions);
        SaveCommand = new DelCom(ExecuteSave, CanExecuteSave);
        ImportQuestionsCommand = new DelCom(ExecuteImportQuestions, CanExecuteImportQuestions);
        ToggleFullscreenCommand = new DelCom(ExecuteToggleFullscreen, CanExecuteToggleFullscreen);
        ExitCommand = new DelCom(ExecuteExit, CanExecuteExit);
        SwitchToConfigCommand = new DelCom(ExecuteSwitchToConfig, CanExecuteSwitchToConfig);
        SwitchToPlayCommand = new DelCom(ExecuteSwitchToPlay, CanExecuteSwitchToPlay);

        LoadPacksAsync();
    }
    
    private async void LoadPacksAsync()
    {
        try
        {
            var loadedPacks = await _jsonFileHandler.LoadPacksAsync();
            Packs.Clear();
            
            foreach (var pack in loadedPacks)
            {
                if (pack.Questions != null && pack.Questions.Any(q => 
                        !string.IsNullOrWhiteSpace(q.Query)
                        && !string.IsNullOrWhiteSpace(q.CorrectAnswer) 
                        && q.IncorrectAnswers?.Length == 3))
                {
                    Packs.Add(new QuestionPackVM(pack));
                }
            }

            if (!Packs.Any())
            {
                CreateNewPack();
            }
            else
            {
                ActivePack = Packs[0];
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading question packs: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            ExecuteNewPack(null);
        }
    }
    
    private void CreateNewPack()
    {
        var newPack = new QuestionPackVM(new QuestionPack
        {
            Name = "New Question Pack",
            Questions = new List<Question>(),
            Difficulty = Difficulty.Medium,
            TimeLimitInSeconds = 30
        });
    
        Packs.Add(newPack);
        ActivePack = newPack;
    }
    
    public async Task SavePacksAsync()
    {
        try
        {
            var packsToSave = Packs.Select(p => new QuestionPack(p.Name, p.Difficulty, p.TimeLimitInSeconds)
            {
                Questions = new List<Question>(p.Questions)
            });
            await _jsonFileHandler.SavePacksAsync(packsToSave);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving question packs: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            throw;
        }
    }
    
    private void ExecuteNewPack(object obj)
    {
        CreateNewPack();
        SavePacksAsync();
    }
    private bool CanExecuteNewPack(object obj) => true;

    private void ExecuteOpenPackOptions(object obj)
    {
        if (ActivePack == null) return;

        var dialog = new PackOptionsDialog
        {
            Owner = Application.Current.MainWindow,
            DataContext = ActivePack
        };

        var result = dialog.ShowDialog();
        if (result == true)
        {
            SavePacksAsync();
        }
    }
    private bool CanExecuteOpenPackOptions(object obj) => ActivePack != null;

    private async void ExecuteSave(object obj)
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            DefaultExt = ".json"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                var pack = ActivePack;
                if (pack != null)
                {
                    await File.WriteAllTextAsync(dialog.FileName, 
                        JsonSerializer.Serialize(new QuestionPack
                        {
                            Name = pack.Name,
                            Difficulty = pack.Difficulty,
                            TimeLimitInSeconds = pack.TimeLimitInSeconds,
                            Questions = pack.Questions.ToList()
                        }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    private bool CanExecuteSave(object obj) => Packs.Count > 0;

    private async void ExecuteImportQuestions(object obj)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            DefaultExt = ".json"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                var jsonString = await File.ReadAllTextAsync(dialog.FileName);
                var importedPack = JsonSerializer.Deserialize<QuestionPack>(jsonString);
            
                if (importedPack != null)
                {
                    var newPack = new QuestionPackVM(importedPack);
                    Packs.Add(newPack);
                    ActivePack = newPack;
                    SavePacksAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing file: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    private bool CanExecuteImportQuestions(object obj) => ActivePack != null;

    private void ExecuteToggleFullscreen(object obj)
    {
        var window = Application.Current.MainWindow;
        if (window != null)
        {
            if (window.WindowState != WindowState.Maximized || window.WindowStyle != WindowStyle.None)
            {
                window.WindowState = WindowState.Maximized;
                window.WindowStyle = WindowStyle.None;
            }
            else
            {
                window.WindowState = WindowState.Normal;
                window.WindowStyle = WindowStyle.SingleBorderWindow;
            }
        }
    }
    private bool CanExecuteToggleFullscreen(object obj) => true;

    private void ExecuteExit(object obj)
    {
        SavePacksAsync();
        Application.Current.Shutdown();
    }
    private bool CanExecuteExit(object obj) => true;

    private void ExecuteSwitchToConfig(object obj)
    {
        PlayerVm.Cleanup();
        IsInConfigMode = true;
        UpdateCommandStates();
    }
    
    private bool CanExecuteSwitchToConfig(object obj) => !IsInConfigMode;

    private async void ExecuteSwitchToPlay(object obj)
    {
        if (ActivePack?.Questions.Count > 0)
        {
            try 
            {
                IsInConfigMode = false;
                PlayerVm.StartQuiz();
                UpdateCommandStates();
            }
            catch (Exception ex)
            {
                IsInConfigMode = true;
                MessageBox.Show($"Error starting play mode: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateCommandStates();
            }
        }
    }
    private bool CanExecuteSwitchToPlay(object obj) => IsInConfigMode && ActivePack?.Questions.Count > 0;

    public void UpdateCommandStates()
    {
        SwitchToPlayCommand.RaiseCanExecuteChanged();
        SwitchToConfigCommand.RaiseCanExecuteChanged();
    }
}