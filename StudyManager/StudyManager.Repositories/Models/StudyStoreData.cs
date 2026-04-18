using System.Text.Json.Serialization;
using StudyManager.Storage;

namespace StudyManager.Repositories.Models;

public sealed class StudyStoreData
{
    public int SchemaVersion { get; set; } = 1;

    public List<SubjectEntity> Subjects { get; set; } = new();
    public List<LessonEntity> Lessons { get; set; } = new();
}

public sealed class SubjectEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int EctsCredits { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SubjectDomain Domain { get; set; }
}

public sealed class LessonEntity
{
    public Guid Id { get; set; }
    public Guid SubjectId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Topic { get; set; } = "";
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public LessonType Type { get; set; }
}