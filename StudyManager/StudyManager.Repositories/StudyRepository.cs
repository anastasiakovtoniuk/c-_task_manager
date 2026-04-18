using StudyManager.Repositories.Models;
using StudyManager.Repositories.Storage;
using StudyManager.Storage;

namespace StudyManager.Repositories;

public sealed class StudyRepository : IStudyRepository
{
    private readonly IStudyStore _store;

    public StudyRepository(IStudyStore store)
    {
        _store = store;
    }

    // ---------- READ ----------
    public async Task<IReadOnlyList<SubjectRecord>> GetAllSubjectsAsync(CancellationToken ct = default)
    {
        var data = await _store.LoadAsync(ct);
        return data.Subjects.Select(ToRecord).ToList();
    }

    public async Task<SubjectRecord?> GetSubjectAsync(Guid id, CancellationToken ct = default)
    {
        var data = await _store.LoadAsync(ct);
        var s = data.Subjects.FirstOrDefault(x => x.Id == id);
        return s is null ? null : ToRecord(s);
    }

    public async Task<IReadOnlyList<LessonRecord>> GetLessonsForSubjectAsync(Guid subjectId, CancellationToken ct = default)
    {
        var data = await _store.LoadAsync(ct);
        return data.Lessons
            .Where(l => l.SubjectId == subjectId)
            .OrderBy(l => l.Date)
            .ThenBy(l => l.StartTime)
            .Select(ToRecord)
            .ToList();
    }

    public async Task<LessonRecord?> GetLessonAsync(Guid lessonId, CancellationToken ct = default)
    {
        var data = await _store.LoadAsync(ct);
        var l = data.Lessons.FirstOrDefault(x => x.Id == lessonId);
        return l is null ? null : ToRecord(l);
    }

    // ---------- CREATE ----------
    public async Task<SubjectRecord> AddSubjectAsync(SubjectRecord subject, CancellationToken ct = default)
    {
        var data = await _store.LoadAsync(ct);

        var id = subject.Id == Guid.Empty ? Guid.NewGuid() : subject.Id;

        // захист від дубля
        if (data.Subjects.Any(s => s.Id == id))
            id = Guid.NewGuid();

        var fixedSubject = new SubjectRecord(id, subject.Name, subject.EctsCredits, subject.Domain);

        data.Subjects.Add(ToEntity(fixedSubject));
        await _store.SaveAsync(data, ct);

        return fixedSubject;
    }

    public async Task<LessonRecord> AddLessonAsync(LessonRecord lesson, CancellationToken ct = default)
    {
        var data = await _store.LoadAsync(ct);

        // урок не може існувати без предмета
        if (!data.Subjects.Any(s => s.Id == lesson.SubjectId))
            throw new InvalidOperationException("Неможливо додати заняття: предмет не знайдено.");

        var id = lesson.Id == Guid.Empty ? Guid.NewGuid() : lesson.Id;

        if (data.Lessons.Any(l => l.Id == id))
            id = Guid.NewGuid();

        var fixedLesson = new LessonRecord(
            id,
            lesson.SubjectId,
            lesson.Date,
            lesson.StartTime,
            lesson.EndTime,
            lesson.Topic,
            lesson.Type);

        data.Lessons.Add(ToEntity(fixedLesson));
        await _store.SaveAsync(data, ct);

        return fixedLesson;
    }

    // ---------- DELETE ----------
    public async Task DeleteSubjectAsync(Guid subjectId, CancellationToken ct = default)
    {
        var data = await _store.LoadAsync(ct);

        data.Subjects.RemoveAll(s => s.Id == subjectId);

        // каскадне видалення занять предмета
        data.Lessons.RemoveAll(l => l.SubjectId == subjectId);

        await _store.SaveAsync(data, ct);
    }
    public async Task<SubjectRecord> UpdateSubjectAsync(SubjectRecord subject, CancellationToken ct = default)
    {
        var data = await _store.LoadAsync(ct);

        var s = data.Subjects.FirstOrDefault(x => x.Id == subject.Id);
        if (s is null)
            throw new InvalidOperationException("Предмет не знайдено для редагування.");

        s.Name = subject.Name.Trim();
        s.EctsCredits = subject.EctsCredits;
        s.Domain = subject.Domain;

        await _store.SaveAsync(data, ct);
        return subject;
    }

    public async Task DeleteLessonAsync(Guid lessonId, CancellationToken ct = default)
    {
        var data = await _store.LoadAsync(ct);
        data.Lessons.RemoveAll(l => l.Id == lessonId);
        await _store.SaveAsync(data, ct);
    }

    // ---------- Mapping ----------
    private static SubjectRecord ToRecord(SubjectEntity e)
        => new(e.Id, e.Name, e.EctsCredits, e.Domain);

    private static LessonRecord ToRecord(LessonEntity e)
        => new(e.Id, e.SubjectId, e.Date, e.StartTime, e.EndTime, e.Topic, e.Type);

    private static SubjectEntity ToEntity(SubjectRecord r)
        => new()
        {
            Id = r.Id,
            Name = r.Name,
            EctsCredits = r.EctsCredits,
            Domain = r.Domain
        };

    private static LessonEntity ToEntity(LessonRecord r)
        => new()
        {
            Id = r.Id,
            SubjectId = r.SubjectId,
            Date = r.Date,
            StartTime = r.StartTime,
            EndTime = r.EndTime,
            Topic = r.Topic,
            Type = r.Type
        };
}