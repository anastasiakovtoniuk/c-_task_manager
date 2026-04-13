namespace StudyManager.Services.Dtos.Lessons;

public sealed record LessonListDto(
    Guid Id,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string Topic,
    string Type,
    TimeSpan Duration
);