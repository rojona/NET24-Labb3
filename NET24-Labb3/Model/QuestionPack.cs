namespace NET24_Labb3.Model;

public enum Difficulty { Easy, Medium, Hard }

public class QuestionPack
{
    public QuestionPack()
    {
        Name = "New Question Pack";
        Difficulty = Difficulty.Medium;
        TimeLimitInSeconds = 10;
        Questions = new List<Question>();
    }
    
    public QuestionPack(string name, Difficulty difficulty = Difficulty.Medium,
        int timeLimitInSeconds = 10)
    {
        Name = name;
        Difficulty = difficulty;
        TimeLimitInSeconds = timeLimitInSeconds;
        Questions = new List<Question>();
    }

    public string Name { get; set; }
    public Difficulty Difficulty { get; set; }
    public int TimeLimitInSeconds { get; set; }
    public List<Question> Questions { get; set; }
}