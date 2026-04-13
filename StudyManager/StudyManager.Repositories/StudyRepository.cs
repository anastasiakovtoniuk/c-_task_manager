using StudyManager.Storage;

namespace StudyManager.Repositories;

public sealed class StudyRepository : IStudyRepository
{
    private readonly IStudyStore _store;

    public StudyRepository(IStudyStore store) => _store = store;

    public IReadOnlyList<SubjectRecord> GetAllSubjects() => _store.Subjects;

    public SubjectRecord? GetSubject(Guid id)
        => _store.Subjects.FirstOrDefault(s => s.Id == id);

    public IReadOnlyList<LessonRecord> GetLessonsForSubject(Guid subjectId)
        => _store.Lessons.Where(l => l.SubjectId == subjectId).ToList();

    public LessonRecord? GetLesson(Guid lessonId)
        => _store.Lessons.FirstOrDefault(l => l.Id == lessonId);
}