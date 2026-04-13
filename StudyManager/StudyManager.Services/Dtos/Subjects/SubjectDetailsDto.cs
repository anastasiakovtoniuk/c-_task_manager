using StudyManager.Services.Dtos.Lessons;

namespace StudyManager.Services.Dtos.Subjects;

public sealed record SubjectDetailsDto(
    Guid Id,
    string Name,
    int EctsCredits,
    string Domain,
    TimeSpan TotalDuration,
    IReadOnlyList<LessonListDto> Lessons
);