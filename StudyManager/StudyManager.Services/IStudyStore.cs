using StudyManager.Storage;

namespace StudyManager.Services;

public interface IStudyStore
{
    IReadOnlyList<SubjectRecord> Subjects { get; }
    IReadOnlyList<LessonRecord> Lessons { get; }
}