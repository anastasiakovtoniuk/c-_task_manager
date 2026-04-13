using System.Collections.ObjectModel;
using StudyManager.Services.Interfaces;
using StudyManager.Services.Dtos.Subjects;
using StudyManager.WpfApp.Infrastructure;
using StudyManager.WpfApp.Navigation;

namespace StudyManager.WpfApp.ViewModels;

public sealed class SubjectsListViewModel : BaseViewModel
{
    private readonly IStudyService _service;
    private readonly INavigationService _nav;

    public ObservableCollection<SubjectListDto> Subjects { get; } = new();

    private SubjectListDto? _selected;
    public SubjectListDto? Selected
    {
        get => _selected;
        set { _selected = value; OnPropertyChanged(); OpenSubjectCommand.RaiseCanExecuteChanged(); }
    }

    public RelayCommand OpenSubjectCommand { get; }

    public SubjectsListViewModel(IStudyService service, INavigationService nav)
    {
        _service = service;
        _nav = nav;

        OpenSubjectCommand = new RelayCommand(
            execute: () => _nav.NavigateTo<SubjectDetailsViewModel>(Selected!.Id),
            canExecute: () => Selected is not null);

        Load();
    }

    private void Load()
    {
        Subjects.Clear();
        foreach (var s in _service.GetSubjects())
            Subjects.Add(s);
    }
}