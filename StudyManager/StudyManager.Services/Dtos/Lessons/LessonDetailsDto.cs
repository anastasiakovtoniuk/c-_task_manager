namespace StudyManager.Services.Dtos.Lessons;

public sealed record LessonDetailsDto(
    Guid Id,
    Guid SubjectId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string Topic,
    string Type,
    TimeSpan Duration
);