using StudyManager.Services.Dtos.Lessons;
using StudyManager.Services.Dtos.Subjects;

namespace StudyManager.Services.Interfaces;

public interface IStudyService
{
    // READ
    Task<IReadOnlyList<SubjectListDto>> GetSubjectsAsync(CancellationToken ct = default);
    Task<SubjectDetailsDto?> GetSubjectDetailsAsync(Guid subjectId, CancellationToken ct = default);
    Task<LessonDetailsDto?> GetLessonDetailsAsync(Guid lessonId, CancellationToken ct = default);

    // CREATE
    Task<SubjectListDto> CreateSubjectAsync(string name, int ectsCredits, string domain, CancellationToken ct = default);
    Task<LessonListDto> CreateLessonAsync(Guid subjectId, DateOnly date, TimeOnly start, TimeOnly end, string topic, string lessonType, CancellationToken ct = default);

    // UPDATE
    Task UpdateSubjectAsync(Guid subjectId, string name, int ectsCredits, string domain, CancellationToken ct = default);

    // DELETE
    Task DeleteSubjectAsync(Guid subjectId, CancellationToken ct = default);
    Task DeleteLessonAsync(Guid lessonId, CancellationToken ct = default);
}