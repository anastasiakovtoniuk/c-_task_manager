using StudyManager.Storage;

namespace StudyManager.Repositories;

public interface IStudyRepository
{
    // READ
    Task<IReadOnlyList<SubjectRecord>> GetAllSubjectsAsync(CancellationToken ct = default);
    Task<SubjectRecord?> GetSubjectAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<LessonRecord>> GetLessonsForSubjectAsync(Guid subjectId, CancellationToken ct = default);
    Task<LessonRecord?> GetLessonAsync(Guid lessonId, CancellationToken ct = default);

    // CREATE
    Task<SubjectRecord> AddSubjectAsync(SubjectRecord subject, CancellationToken ct = default);
    Task<LessonRecord> AddLessonAsync(LessonRecord lesson, CancellationToken ct = default);

    // UPDATE 
    Task<SubjectRecord> UpdateSubjectAsync(SubjectRecord subject, CancellationToken ct = default);

    // DELETE
    Task DeleteSubjectAsync(Guid subjectId, CancellationToken ct = default); // каскадно видаляє уроки
    Task DeleteLessonAsync(Guid lessonId, CancellationToken ct = default);
}