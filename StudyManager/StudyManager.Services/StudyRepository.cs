using System;
using System.Collections.Generic;
using System.Text;

using StudyManager.App;
using StudyManager.Storage;

namespace StudyManager.Services;

public sealed class StudyRepository : IStudyRepository
{
    public IReadOnlyList<SubjectView> GetAllSubjects()
        => FakeStudyStore.Subjects
            .Select(MapSubject)
            .ToList();

    public SubjectView? GetSubject(Guid id)
    {
        var r = FakeStudyStore.Subjects.FirstOrDefault(s => s.Id == id);
        return r is null ? null : MapSubject(r);
    }

    public IReadOnlyList<LessonView> GetLessonsForSubject(Guid subjectId)
        => FakeStudyStore.Lessons
            .Where(l => l.SubjectId == subjectId)
            .OrderBy(l => l.Date)
            .ThenBy(l => l.StartTime)
            .Select(MapLesson)
            .ToList();

    public LessonView? GetLesson(Guid lessonId)
    {
        var r = FakeStudyStore.Lessons.FirstOrDefault(l => l.Id == lessonId);
        return r is null ? null : MapLesson(r);
    }

    public void EnsureLessonsLoaded(SubjectView subject)
    {
        if (subject.LessonsLoaded) return;
        var lessons = GetLessonsForSubject(subject.Id);
        subject.SetLessons(lessons);
    }

    private static SubjectView MapSubject(SubjectRecord r)
        => new(r.Id, r.Name, r.EctsCredits, r.Domain);

    private static LessonView MapLesson(LessonRecord r)
        => new(r.Id, r.SubjectId, r.Date, r.StartTime, r.EndTime, r.Topic, r.Type);
}