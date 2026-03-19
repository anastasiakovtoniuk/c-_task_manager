using StudyManager.App;
using StudyManager.Storage;

namespace StudyManager.Services;

public sealed class StudyRepository : IStudyRepository
{
    private readonly IStudyStore _store;

    public StudyRepository(IStudyStore store)
    {
        _store = store;
    }

    public IReadOnlyList<SubjectView> GetAllSubjects()
        => _store.Subjects.Select(MapSubject).ToList();

    public SubjectView? GetSubject(Guid id)
    {
        var r = _store.Subjects.FirstOrDefault(s => s.Id == id);
        return r is null ? null : MapSubject(r);
    }

    public IReadOnlyList<LessonView> GetLessonsForSubject(Guid subjectId)
        => _store.Lessons
            .Where(l => l.SubjectId == subjectId)
            .OrderBy(l => l.Date)
            .ThenBy(l => l.StartTime)
            .Select(MapLesson)
            .ToList();

    public LessonView? GetLesson(Guid lessonId)
    {
        var r = _store.Lessons.FirstOrDefault(l => l.Id == lessonId);
        return r is null ? null : MapLesson(r);
    }

    public void EnsureLessonsLoaded(SubjectView subject)
    {
        if (subject.LessonsLoaded) return;
        subject.SetLessons(GetLessonsForSubject(subject.Id));
    }

    private static SubjectView MapSubject(SubjectRecord r)
        => new(r.Id, r.Name, r.EctsCredits, r.Domain);

    private static LessonView MapLesson(LessonRecord r)
        => new(r.Id, r.SubjectId, r.Date, r.StartTime, r.EndTime, r.Topic, r.Type);
}