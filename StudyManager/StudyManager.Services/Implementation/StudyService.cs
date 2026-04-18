using StudyManager.Repositories;
using StudyManager.Services.Dtos.Lessons;
using StudyManager.Services.Dtos.Subjects;
using StudyManager.Services.Interfaces;
using StudyManager.Storage;

namespace StudyManager.Services.Implementation;

public sealed class StudyService : IStudyService
{
    private readonly IStudyRepository _repo;

    public StudyService(IStudyRepository repo)
    {
        _repo = repo;
    }

    // ---------- READ ----------
    public async Task<IReadOnlyList<SubjectListDto>> GetSubjectsAsync(CancellationToken ct = default)
    {
        var subjects = await _repo.GetAllSubjectsAsync(ct);
        return subjects
            .Select(s => new SubjectListDto(s.Id, s.Name, s.EctsCredits, s.Domain.ToString()))
            .ToList();
    }

    public async Task<SubjectDetailsDto?> GetSubjectDetailsAsync(Guid subjectId, CancellationToken ct = default)
    {
        var subject = await _repo.GetSubjectAsync(subjectId, ct);
        if (subject is null) return null;

        var lessons = await _repo.GetLessonsForSubjectAsync(subjectId, ct);

        var lessonDtos = lessons.Select(l =>
        {
            var duration = l.EndTime.ToTimeSpan() - l.StartTime.ToTimeSpan();
            return new LessonListDto(l.Id, l.Date, l.StartTime, l.EndTime, l.Topic, l.Type.ToString(), duration);
        }).ToList();

        var total = lessonDtos.Aggregate(TimeSpan.Zero, (sum, x) => sum + x.Duration);

        return new SubjectDetailsDto(
            subject.Id,
            subject.Name,
            subject.EctsCredits,
            subject.Domain.ToString(),
            total,
            lessonDtos);
    }
    public async Task UpdateSubjectAsync(Guid subjectId, string name, int ectsCredits, string domain, CancellationToken ct = default)
    {
        var parsedDomain = ParseDomain(domain);

        await _repo.UpdateSubjectAsync(
            new SubjectRecord(subjectId, name.Trim(), ectsCredits, parsedDomain),
            ct);
    }
    public async Task<LessonDetailsDto?> GetLessonDetailsAsync(Guid lessonId, CancellationToken ct = default)
    {
        var lesson = await _repo.GetLessonAsync(lessonId, ct);
        if (lesson is null) return null;

        var duration = lesson.EndTime.ToTimeSpan() - lesson.StartTime.ToTimeSpan();

        return new LessonDetailsDto(
            lesson.Id,
            lesson.SubjectId,
            lesson.Date,
            lesson.StartTime,
            lesson.EndTime,
            lesson.Topic,
            lesson.Type.ToString(),
            duration);
    }

    // ---------- CREATE ----------
    public async Task<SubjectListDto> CreateSubjectAsync(string name, int ectsCredits, string domain, CancellationToken ct = default)
    {
        var parsedDomain = ParseDomain(domain);

        var created = await _repo.AddSubjectAsync(
            new SubjectRecord(Guid.NewGuid(), name.Trim(), ectsCredits, parsedDomain),
            ct);

        return new SubjectListDto(created.Id, created.Name, created.EctsCredits, created.Domain.ToString());
    }

    public async Task<LessonListDto> CreateLessonAsync(Guid subjectId, DateOnly date, TimeOnly start, TimeOnly end, string topic, string lessonType, CancellationToken ct = default)
    {
        var parsedType = ParseLessonType(lessonType);

        var created = await _repo.AddLessonAsync(
            new LessonRecord(Guid.NewGuid(), subjectId, date, start, end, topic.Trim(), parsedType),
            ct);

        var duration = created.EndTime.ToTimeSpan() - created.StartTime.ToTimeSpan();

        return new LessonListDto(created.Id, created.Date, created.StartTime, created.EndTime, created.Topic, created.Type.ToString(), duration);
    }

    // ---------- DELETE ----------
    public Task DeleteSubjectAsync(Guid subjectId, CancellationToken ct = default)
        => _repo.DeleteSubjectAsync(subjectId, ct);

    public Task DeleteLessonAsync(Guid lessonId, CancellationToken ct = default)
        => _repo.DeleteLessonAsync(lessonId, ct);

    // ---------- Helpers ----------
    private static SubjectDomain ParseDomain(string domain)
        => Enum.TryParse<SubjectDomain>(domain, true, out var d) ? d : SubjectDomain.Programming;

    private static LessonType ParseLessonType(string type)
        => Enum.TryParse<LessonType>(type, true, out var t) ? t : LessonType.Lecture;
}