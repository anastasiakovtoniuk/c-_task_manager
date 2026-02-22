using System;
using System.Collections.Generic;
using System.Text;

using StudyManager.App;

namespace StudyManager.Services;

public interface IStudyRepository
{
    IReadOnlyList<SubjectView> GetAllSubjects();
    SubjectView? GetSubject(Guid id);

    IReadOnlyList<LessonView> GetLessonsForSubject(Guid subjectId);
    LessonView? GetLesson(Guid lessonId);

    void EnsureLessonsLoaded(SubjectView subject);
}