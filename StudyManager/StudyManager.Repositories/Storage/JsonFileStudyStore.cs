using System.Text.Json;
using StudyManager.Repositories.Models;
using StudyManager.Repositories.Serialization;

namespace StudyManager.Repositories.Storage;

public sealed class JsonFileStudyStore : IStudyStore
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _json;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public JsonFileStudyStore(string? filePath = null)
    {
        _filePath = filePath ?? GetDefaultPath();

        _json = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
        _json.Converters.Add(new DateOnlyJsonConverter());
        _json.Converters.Add(new TimeOnlyJsonConverter());
    }

    public async Task<StudyStoreData> LoadAsync(CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            EnsureDirectory();

            if (!File.Exists(_filePath))
            {
                
                var seeded = SeedData.Create();
                await SaveInternalAsync(seeded, ct);
                return seeded;
            }

            await using var fs = new FileStream(
                _filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 4096,
                useAsync: true);

            var data = await JsonSerializer.DeserializeAsync<StudyStoreData>(fs, _json, ct);

            // якщо файл порожній/зіпсований — пересідимо
            if (data is null)
            {
                var seeded = SeedData.Create();
                await SaveInternalAsync(seeded, ct);
                return seeded;
            }

            data.Subjects ??= new();
            data.Lessons ??= new();
            return data;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task SaveAsync(StudyStoreData data, CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            EnsureDirectory();
            await SaveInternalAsync(data, ct);
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task SaveInternalAsync(StudyStoreData data, CancellationToken ct)
    {
        var temp = _filePath + ".tmp";

        await using (var fs = new FileStream(
            temp,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true))
        {
            await JsonSerializer.SerializeAsync(fs, data, _json, ct);
            await fs.FlushAsync(ct);
        }

        
        File.Copy(temp, _filePath, overwrite: true);
        File.Delete(temp);
    }

    private void EnsureDirectory()
    {
        var dir = Path.GetDirectoryName(_filePath)!;
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    private static string GetDefaultPath()
    {
        var baseDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "StudyManager");

        return Path.Combine(baseDir, "study_store.json");
    }
}