using System.Collections.ObjectModel;
using StudyManager.Services.Interfaces;
using StudyManager.Services.Dtos.Lessons;
using StudyManager.Services.Dtos.Subjects;
using StudyManager.WpfApp.Infrastructure;
using StudyManager.WpfApp.Navigation;

namespace StudyManager.WpfApp.ViewModels;

public sealed class SubjectDetailsViewModel : BaseViewModel, IParameterReceiver
{
    private readonly IStudyService _service;
    private readonly INavigationService _nav;

    private SubjectDetailsDto? _subject;
    public SubjectDetailsDto? Subject
    {
        get => _subject;
        private set { _subject = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasNoLessons)); }
    }

    public ObservableCollection<LessonListDto> Lessons { get; } = new();

    public bool HasNoLessons => Lessons.Count == 0;

    private LessonListDto? _selectedLesson;
    public LessonListDto? SelectedLesson
    {
        get => _selectedLesson;
        set { _selectedLesson = value; OnPropertyChanged(); OpenLessonCommand.RaiseCanExecuteChanged(); }
    }

    public RelayCommand OpenLessonCommand { get; }

    public SubjectDetailsViewModel(IStudyService service, INavigationService nav)
    {
        _service = service;
        _nav = nav;

        OpenLessonCommand = new RelayCommand(
            execute: () => _nav.NavigateTo<LessonDetailsViewModel>(SelectedLesson!.Id),
            canExecute: () => SelectedLesson is not null);
    }

    public void Receive(object? parameter)
    {
        if (parameter is not Guid subjectId) return;

        var details = _service.GetSubjectDetails(subjectId);
        Subject = details;

        Lessons.Clear();
        if (details is null) return;

        foreach (var l in details.Lessons)
            Lessons.Add(l);

        OnPropertyChanged(nameof(HasNoLessons));
    }
}