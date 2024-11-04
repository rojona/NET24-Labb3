using System.Windows;
using NET24_Labb3.ViewModel;

namespace NET24_Labb3.View;

public partial class QuestionEditorDialog : Window
{
    public QuestionEditorDialog()
    {
        InitializeComponent();
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (ValidateInputs())
        {
            DialogResult = true;
            (Owner?.DataContext as MainWindowVM)?.UpdateCommandStates();
        }
    }

    private bool ValidateInputs()
    {
        if (DataContext is not Model.Question question) return false;

        if (string.IsNullOrWhiteSpace(question.Query))
        {
            MessageBox.Show("Question cannot be empty.", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(question.CorrectAnswer))
        {
            MessageBox.Show("Correct answer cannot be empty.", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        foreach (var answer in question.IncorrectAnswers)
        {
            if (string.IsNullOrWhiteSpace(answer))
            {
                MessageBox.Show("Wrong answers cannot be empty.", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        return true;
    }
}