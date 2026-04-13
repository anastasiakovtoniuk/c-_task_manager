using StudyManager.Services.Dtos.Lessons;
using StudyManager.Services.Dtos.Subjects;

namespace StudyManager.Services.Interfaces;

public interface IStudyService
{
    IReadOnlyList<SubjectListDto> GetSubjects();
    SubjectDetailsDto? GetSubjectDetails(Guid subjectId);
    LessonDetailsDto? GetLessonDetails(Guid lessonId);
}