using StudyManager.Repositories; 
using StudyManager.Services.Dtos.Lessons;
using StudyManager.Services.Dtos.Subjects;
using StudyManager.Services.Interfaces;

namespace StudyManager.Services.Implementation;

public sealed class StudyService : IStudyService
{
    private readonly IStudyRepository _repo;

    public StudyService(IStudyRepository repo)
    {
        _repo = repo;
    }

    public IReadOnlyList<SubjectListDto> GetSubjects()
    {
        var subjects = _repo.GetAllSubjects(); // повертає DB models (SubjectRecord)
        return subjects
            .Select(s => new SubjectListDto(
                s.Id,
                s.Name,
                s.EctsCredits,
                s.Domain.ToString()))
            .ToList();
    }

    public SubjectDetailsDto? GetSubjectDetails(Guid subjectId)
    {
        var subject = _repo.GetSubject(subjectId);
        if (subject is null) return null;

        var lessons = _repo.GetLessonsForSubject(subjectId);

        var lessonDtos = lessons.Select(l =>
        {
            var duration = l.EndTime.ToTimeSpan() - l.StartTime.ToTimeSpan();
            return new LessonListDto(
                l.Id,
                l.Date,
                l.StartTime,
                l.EndTime,
                l.Topic,
                l.Type.ToString(),
                duration);
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

    public LessonDetailsDto? GetLessonDetails(Guid lessonId)
    {
        var lesson = _repo.GetLesson(lessonId);
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
}