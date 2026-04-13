using StudyManager.Storage;

namespace StudyManager.Repositories;

public interface IStudyStore
{
    IReadOnlyList<SubjectRecord> Subjects { get; }
    IReadOnlyList<LessonRecord> Lessons { get; }
}