using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using StudyManager.Services.Dtos.Lessons;
using StudyManager.Services.Dtos.Subjects;
using StudyManager.Services.Interfaces;
using StudyManager.WpfApp.Infrastructure;
using StudyManager.WpfApp.Navigation;

namespace StudyManager.WpfApp.ViewModels;

public sealed class SubjectDetailsViewModel : BaseViewModel, IParameterReceiver
{
    private readonly IStudyService _service;
    private readonly INavigationService _nav;

    private Guid _subjectId;

    private SubjectDetailsDto? _subject;
    public SubjectDetailsDto? Subject
    {
        get => _subject;
        private set { _subject = value; OnPropertyChanged(); }
    }

    
    public ObservableCollection<LessonListDto> AllLessons { get; } = new();
    public ICollectionView LessonsView { get; }

    public ObservableCollection<string> LessonSortOptions { get; } =
        new() { "Дата (↑)", "Дата (↓)", "Тривалість (↑)", "Тривалість (↓)" };

    public ObservableCollection<string> LessonTypeOptions { get; } =
        new() { "Lecture", "Seminar", "Practice", "Lab" };

    private string _lessonSearchText = "";
    public string LessonSearchText
    {
        get => _lessonSearchText;
        set
        {
            _lessonSearchText = value ?? "";
            OnPropertyChanged();
            LessonsView.Refresh();
            OnPropertyChanged(nameof(HasNoLessons));
        }
    }

    private string _selectedLessonSort = "Дата (↑)";
    public string SelectedLessonSort
    {
        get => _selectedLessonSort;
        set
        {
            _selectedLessonSort = string.IsNullOrWhiteSpace(value) ? "Дата (↑)" : value;
            OnPropertyChanged();
            ApplyLessonSorting();
        }
    }

    public bool HasNoLessons => LessonsView.IsEmpty;

    private LessonListDto? _selectedLesson;
    public LessonListDto? SelectedLesson
    {
        get => _selectedLesson;
        set
        {
            _selectedLesson = value;
            OnPropertyChanged();
            OpenLessonCommand.RaiseCanExecuteChanged();
            DeleteLessonCommand.RaiseCanExecuteChanged();
        }
    }

    
    private string _newLessonTopic = "";
    public string NewLessonTopic
    {
        get => _newLessonTopic;
        set { _newLessonTopic = value ?? ""; OnPropertyChanged(); AddLessonCommand.RaiseCanExecuteChanged(); }
    }

    private DateTime? _newLessonDate = DateTime.Today;
    public DateTime? NewLessonDate
    {
        get => _newLessonDate;
        set { _newLessonDate = value; OnPropertyChanged(); AddLessonCommand.RaiseCanExecuteChanged(); }
    }

    private string _newStart = "10:00";
    public string NewStart
    {
        get => _newStart;
        set { _newStart = value ?? ""; OnPropertyChanged(); AddLessonCommand.RaiseCanExecuteChanged(); }
    }

    private string _newEnd = "11:30";
    public string NewEnd
    {
        get => _newEnd;
        set { _newEnd = value ?? ""; OnPropertyChanged(); AddLessonCommand.RaiseCanExecuteChanged(); }
    }

    private string _newLessonType = "Lecture";
    public string NewLessonType
    {
        get => _newLessonType;
        set { _newLessonType = value ?? "Lecture"; OnPropertyChanged(); AddLessonCommand.RaiseCanExecuteChanged(); }
    }

   
    public ObservableCollection<string> SubjectDomainOptions { get; } =
        new() { "Programming", "Mathematics", "Engineering" };

    private bool _isEditingSubject;
    public bool IsEditingSubject
    {
        get => _isEditingSubject;
        private set
        {
            _isEditingSubject = value;
            OnPropertyChanged();
            EditSubjectCommand.RaiseCanExecuteChanged();
            SaveSubjectCommand.RaiseCanExecuteChanged();
            CancelEditSubjectCommand.RaiseCanExecuteChanged();
        }
    }

    private string _editName = "";
    public string EditName
    {
        get => _editName;
        set { _editName = value ?? ""; OnPropertyChanged(); SaveSubjectCommand.RaiseCanExecuteChanged(); }
    }

    private string _editEcts = "";
    public string EditEcts
    {
        get => _editEcts;
        set { _editEcts = value ?? ""; OnPropertyChanged(); SaveSubjectCommand.RaiseCanExecuteChanged(); }
    }

    private string _editDomain = "Programming";
    public string EditDomain
    {
        get => _editDomain;
        set { _editDomain = value ?? "Programming"; OnPropertyChanged(); SaveSubjectCommand.RaiseCanExecuteChanged(); }
    }

    
    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            _isBusy = value;
            OnPropertyChanged();

            OpenLessonCommand.RaiseCanExecuteChanged();
            AddLessonCommand.RaiseCanExecuteChanged();
            DeleteLessonCommand.RaiseCanExecuteChanged();

            EditSubjectCommand.RaiseCanExecuteChanged();
            SaveSubjectCommand.RaiseCanExecuteChanged();
            CancelEditSubjectCommand.RaiseCanExecuteChanged();
        }
    }

    
    public RelayCommand OpenLessonCommand { get; }
    public RelayCommand AddLessonCommand { get; }
    public RelayCommand DeleteLessonCommand { get; }

    public RelayCommand EditSubjectCommand { get; }
    public RelayCommand SaveSubjectCommand { get; }
    public RelayCommand CancelEditSubjectCommand { get; }

    public SubjectDetailsViewModel(IStudyService service, INavigationService nav)
    {
        _service = service;
        _nav = nav;

        LessonsView = CollectionViewSource.GetDefaultView(AllLessons);
        LessonsView.Filter = FilterLesson;

        OpenLessonCommand = new RelayCommand(
            execute: () =>
            {
                if (SelectedLesson is null) return;
                _nav.NavigateTo<LessonDetailsViewModel>(SelectedLesson.Id);
            },
            canExecute: () => !IsBusy && SelectedLesson is not null);

        AddLessonCommand = new RelayCommand(
            execute: () => _ = AddLessonAsync(),
            canExecute: () => !IsBusy && CanAddLesson());

        DeleteLessonCommand = new RelayCommand(
            execute: () => _ = DeleteLessonAsync(),
            canExecute: () => !IsBusy && SelectedLesson is not null);

        
        EditSubjectCommand = new RelayCommand(
            execute: EnterEditMode,
            canExecute: () => !IsBusy && Subject is not null && !IsEditingSubject);

        SaveSubjectCommand = new RelayCommand(
            execute: () => _ = SaveSubjectAsync(),
            canExecute: () => !IsBusy && IsEditingSubject && CanSaveSubject());

        CancelEditSubjectCommand = new RelayCommand(
            execute: CancelEdit,
            canExecute: () => !IsBusy && IsEditingSubject);
    }

    public void Receive(object? parameter)
    {
        if (parameter is not Guid subjectId) return;
        _subjectId = subjectId;
        _ = LoadAsync(subjectId);
    }

    private bool FilterLesson(object obj)
    {
        if (obj is not LessonListDto l) return false;
        var q = LessonSearchText.Trim();
        if (q.Length == 0) return true;

        return l.Topic.Contains(q, StringComparison.OrdinalIgnoreCase)
               || l.Type.Contains(q, StringComparison.OrdinalIgnoreCase)
               || l.Date.ToString("yyyy-MM-dd").Contains(q, StringComparison.OrdinalIgnoreCase);
    }

    private void ApplyLessonSorting()
    {
        LessonsView.SortDescriptions.Clear();

        switch (SelectedLessonSort)
        {
            case "Дата (↓)":
                LessonsView.SortDescriptions.Add(new SortDescription(nameof(LessonListDto.Date), ListSortDirection.Descending));
                break;
            case "Тривалість (↑)":
                LessonsView.SortDescriptions.Add(new SortDescription(nameof(LessonListDto.Duration), ListSortDirection.Ascending));
                break;
            case "Тривалість (↓)":
                LessonsView.SortDescriptions.Add(new SortDescription(nameof(LessonListDto.Duration), ListSortDirection.Descending));
                break;
            default:
                LessonsView.SortDescriptions.Add(new SortDescription(nameof(LessonListDto.Date), ListSortDirection.Ascending));
                break;
        }

        LessonsView.Refresh();
        OnPropertyChanged(nameof(HasNoLessons));
    }

    private async Task LoadAsync(Guid subjectId)
    {
        IsBusy = true;
        try
        {
            SelectedLesson = null;
            Subject = await _service.GetSubjectDetailsAsync(subjectId);

            
            SyncEditFieldsFromSubject();

            AllLessons.Clear();
            if (Subject is not null)
            {
                foreach (var l in Subject.Lessons)
                    AllLessons.Add(l);
            }

            ApplyLessonSorting();
            LessonsView.Refresh();
            OnPropertyChanged(nameof(HasNoLessons));
        }
        finally
        {
            IsBusy = false;
        }
    }

    
    private void SyncEditFieldsFromSubject()
    {
        if (Subject is null) return;

        EditName = Subject.Name;
        EditEcts = Subject.EctsCredits.ToString();
        EditDomain = Subject.Domain; 
    }

    private void EnterEditMode()
    {
        SyncEditFieldsFromSubject();
        IsEditingSubject = true;
    }

    private void CancelEdit()
    {
        SyncEditFieldsFromSubject();
        IsEditingSubject = false;
    }

    private bool CanSaveSubject()
    {
        if (string.IsNullOrWhiteSpace(EditName)) return false;
        if (!int.TryParse(EditEcts, out var ects) || ects <= 0) return false;
        if (string.IsNullOrWhiteSpace(EditDomain)) return false;
        return true;
    }

    private async Task SaveSubjectAsync()
    {
        if (!int.TryParse(EditEcts, out var ects) || ects <= 0)
        {
            MessageBox.Show("ECTS має бути додатним числом.");
            return;
        }

        IsBusy = true;
        try
        {
            await _service.UpdateSubjectAsync(_subjectId, EditName, ects, EditDomain);
            IsEditingSubject = false;

            
            await LoadAsync(_subjectId);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка редагування предмета: " + ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    // ===== Add/Delete lesson =====
    private bool CanAddLesson()
        => !string.IsNullOrWhiteSpace(NewLessonTopic)
           && NewLessonDate is not null
           && !string.IsNullOrWhiteSpace(NewStart)
           && !string.IsNullOrWhiteSpace(NewEnd)
           && !string.IsNullOrWhiteSpace(NewLessonType);

    private async Task AddLessonAsync()
    {
        if (NewLessonDate is null)
        {
            MessageBox.Show("Оберіть дату заняття.");
            return;
        }

        if (!TimeOnly.TryParse(NewStart, out var start))
        {
            MessageBox.Show("Невірний час початку. Формат: HH:mm (наприклад 10:00).");
            return;
        }

        if (!TimeOnly.TryParse(NewEnd, out var end))
        {
            MessageBox.Show("Невірний час завершення. Формат: HH:mm (наприклад 11:30).");
            return;
        }

        if (end <= start)
        {
            MessageBox.Show("Час завершення має бути пізніше за час початку.");
            return;
        }

        IsBusy = true;
        try
        {
            var date = DateOnly.FromDateTime(NewLessonDate.Value);

            await _service.CreateLessonAsync(
                _subjectId,
                date,
                start,
                end,
                NewLessonTopic,
                NewLessonType);

            NewLessonTopic = "";
            NewLessonDate = DateTime.Today;
            NewStart = "10:00";
            NewEnd = "11:30";
            NewLessonType = "Lecture";

            await LoadAsync(_subjectId);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка додавання заняття: " + ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteLessonAsync()
    {
        if (SelectedLesson is null) return;

        var confirm = MessageBox.Show(
            $"Видалити заняття \"{SelectedLesson.Topic}\"?",
            "Підтвердження",
            MessageBoxButton.YesNo);

        if (confirm != MessageBoxResult.Yes) return;

        IsBusy = true;
        try
        {
            await _service.DeleteLessonAsync(SelectedLesson.Id);
            await LoadAsync(_subjectId);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка видалення заняття: " + ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}