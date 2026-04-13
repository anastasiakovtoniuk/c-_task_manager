using StudyManager.Services.Dtos.Lessons;
using StudyManager.Services.Interfaces;
using StudyManager.WpfApp.Infrastructure;
using StudyManager.WpfApp.Navigation;

namespace StudyManager.WpfApp.ViewModels;

public sealed class LessonDetailsViewModel : BaseViewModel, IParameterReceiver
{
    private readonly IStudyService _service;

    private LessonDetailsDto? _lesson;
    public LessonDetailsDto? Lesson
    {
        get => _lesson;
        private set { _lesson = value; OnPropertyChanged(); }
    }

    public LessonDetailsViewModel(IStudyService service)
    {
        _service = service;
    }

    public void Receive(object? parameter)
    {
        if (parameter is not Guid lessonId) return;
        Lesson = _service.GetLessonDetails(lessonId);
    }
}