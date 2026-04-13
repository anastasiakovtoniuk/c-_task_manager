using StudyManager.Storage;

namespace StudyManager.Repositories;

public interface IStudyRepository
{
    IReadOnlyList<SubjectRecord> GetAllSubjects();
    SubjectRecord? GetSubject(Guid id);

    IReadOnlyList<LessonRecord> GetLessonsForSubject(Guid subjectId);
    LessonRecord? GetLesson(Guid lessonId);
}