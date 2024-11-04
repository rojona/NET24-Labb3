using System.IO;
using System.Text.Json;
using NET24_Labb3.Model;

namespace NET24_Labb3.Services;

internal class JsonFileHandler
{
    private readonly string _appDataPath;
    private readonly string _jsonFilePath;

    public JsonFileHandler()
    {
        _appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "QuizGame");
        _jsonFilePath = Path.Combine(_appDataPath, "questionpacks.json");
        
        // Ensure directory exists
        if (!Directory.Exists(_appDataPath))
        {
            Directory.CreateDirectory(_appDataPath);
        }
    }

    public async Task SavePacksAsync(IEnumerable<QuestionPack> packs)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var jsonString = JsonSerializer.Serialize(packs, options);
        await File.WriteAllTextAsync(_jsonFilePath, jsonString);
    }

    public async Task<List<QuestionPack>> LoadPacksAsync()
    {
        if (!File.Exists(_jsonFilePath))
        {
            return new List<QuestionPack>();
        }

        var jsonString = await File.ReadAllTextAsync(_jsonFilePath);
        var packs = JsonSerializer.Deserialize<List<QuestionPack>>(jsonString);
        return packs ?? new List<QuestionPack>();
    }
}