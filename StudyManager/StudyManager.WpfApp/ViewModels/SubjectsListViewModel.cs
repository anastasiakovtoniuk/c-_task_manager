using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using StudyManager.Services.Dtos.Subjects;
using StudyManager.Services.Interfaces;
using StudyManager.WpfApp.Infrastructure;
using StudyManager.WpfApp.Navigation;

namespace StudyManager.WpfApp.ViewModels;

public sealed class SubjectsListViewModel : BaseViewModel
{
    private readonly IStudyService _service;
    private readonly INavigationService _nav;

    public ObservableCollection<SubjectListDto> AllSubjects { get; } = new();
    public ICollectionView SubjectsView { get; }

    public ObservableCollection<string> DomainOptions { get; } = new();
    public ObservableCollection<string> SortOptions { get; } = new() { "Назва (A→Z)", "Назва (Z→A)", "ECTS (↑)", "ECTS (↓)" };

    
    public ObservableCollection<string> CreateDomainOptions { get; } =
        new() { "Programming", "Mathematics", "Engineering" };

    private string _newName = "";
    public string NewName
    {
        get => _newName;
        set { _newName = value ?? ""; OnPropertyChanged(); AddSubjectCommand.RaiseCanExecuteChanged(); }
    }

    private string _newEcts = "3";
    public string NewEcts
    {
        get => _newEcts;
        set { _newEcts = value ?? ""; OnPropertyChanged(); AddSubjectCommand.RaiseCanExecuteChanged(); }
    }

    private string _newDomain = "Programming";
    public string NewDomain
    {
        get => _newDomain;
        set { _newDomain = value ?? "Programming"; OnPropertyChanged(); AddSubjectCommand.RaiseCanExecuteChanged(); }
    }

    
    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set { _searchText = value ?? ""; OnPropertyChanged(); SubjectsView.Refresh(); }
    }

    private string _selectedDomain = "Усі";
    public string SelectedDomain
    {
        get => _selectedDomain;
        set { _selectedDomain = string.IsNullOrWhiteSpace(value) ? "Усі" : value; OnPropertyChanged(); SubjectsView.Refresh(); }
    }

    private string _selectedSort = "Назва (A→Z)";
    public string SelectedSort
    {
        get => _selectedSort;
        set { _selectedSort = string.IsNullOrWhiteSpace(value) ? "Назва (A→Z)" : value; OnPropertyChanged(); ApplySorting(); }
    }

    
    private SubjectListDto? _selected;
    public SubjectListDto? Selected
    {
        get => _selected;
        set { _selected = value; OnPropertyChanged(); OpenSubjectCommand.RaiseCanExecuteChanged(); DeleteSubjectCommand.RaiseCanExecuteChanged(); }
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            _isBusy = value;
            OnPropertyChanged();
            RefreshCommand.RaiseCanExecuteChanged();
            OpenSubjectCommand.RaiseCanExecuteChanged();
            AddSubjectCommand.RaiseCanExecuteChanged();
            DeleteSubjectCommand.RaiseCanExecuteChanged();
        }
    }

    public RelayCommand RefreshCommand { get; }
    public RelayCommand OpenSubjectCommand { get; }
    public RelayCommand AddSubjectCommand { get; }
    public RelayCommand DeleteSubjectCommand { get; }

    public SubjectsListViewModel(IStudyService service, INavigationService nav)
    {
        _service = service;
        _nav = nav;

        SubjectsView = CollectionViewSource.GetDefaultView(AllSubjects);
        SubjectsView.Filter = FilterSubject;

        RefreshCommand = new RelayCommand(() => _ = LoadAsync(), () => !IsBusy);

        OpenSubjectCommand = new RelayCommand(
            execute: () =>
            {
                if (Selected is null) return;
                _nav.NavigateTo<SubjectDetailsViewModel>(Selected.Id);
            },
            canExecute: () => !IsBusy && Selected is not null);

        AddSubjectCommand = new RelayCommand(
            execute: () => _ = AddSubjectAsync(),
            canExecute: () => !IsBusy && CanAddSubject());

        DeleteSubjectCommand = new RelayCommand(
            execute: () => _ = DeleteSubjectAsync(),
            canExecute: () => !IsBusy && Selected is not null);

        _ = LoadAsync();
    }

    private bool CanAddSubject()
        => !string.IsNullOrWhiteSpace(NewName)
           && int.TryParse(NewEcts, out var ects) && ects > 0
           && !string.IsNullOrWhiteSpace(NewDomain);

    private bool FilterSubject(object obj)
    {
        if (obj is not SubjectListDto s) return false;

        if (SelectedDomain != "Усі" && !string.Equals(s.Domain, SelectedDomain, StringComparison.OrdinalIgnoreCase))
            return false;

        var q = SearchText.Trim();
        if (q.Length == 0) return true;

        return s.Name.Contains(q, StringComparison.OrdinalIgnoreCase)
               || s.Domain.Contains(q, StringComparison.OrdinalIgnoreCase)
               || s.EctsCredits.ToString().Contains(q, StringComparison.OrdinalIgnoreCase);
    }

    private void ApplySorting()
    {
        SubjectsView.SortDescriptions.Clear();

        switch (SelectedSort)
        {
            case "Назва (Z→A)":
                SubjectsView.SortDescriptions.Add(new SortDescription(nameof(SubjectListDto.Name), ListSortDirection.Descending));
                break;
            case "ECTS (↑)":
                SubjectsView.SortDescriptions.Add(new SortDescription(nameof(SubjectListDto.EctsCredits), ListSortDirection.Ascending));
                break;
            case "ECTS (↓)":
                SubjectsView.SortDescriptions.Add(new SortDescription(nameof(SubjectListDto.EctsCredits), ListSortDirection.Descending));
                break;
            default:
                SubjectsView.SortDescriptions.Add(new SortDescription(nameof(SubjectListDto.Name), ListSortDirection.Ascending));
                break;
        }

        SubjectsView.Refresh();
    }

    private void RebuildDomainOptions()
    {
        DomainOptions.Clear();
        DomainOptions.Add("Усі");

        foreach (var d in AllSubjects.Select(s => s.Domain).Distinct().OrderBy(x => x))
            DomainOptions.Add(d);

        if (!DomainOptions.Contains(SelectedDomain))
            SelectedDomain = "Усі";
    }

    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            Selected = null;
            AllSubjects.Clear();

            var items = await _service.GetSubjectsAsync();
            foreach (var s in items) AllSubjects.Add(s);

            RebuildDomainOptions();
            ApplySorting();
            SubjectsView.Refresh();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddSubjectAsync()
    {
        if (!int.TryParse(NewEcts, out var ects) || ects <= 0)
        {
            MessageBox.Show("ECTS має бути додатним числом.");
            return;
        }

        IsBusy = true;
        try
        {
            await _service.CreateSubjectAsync(NewName, ects, NewDomain);

            NewName = "";
            NewEcts = "3";
            NewDomain = "Programming";

            await LoadAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка додавання предмета: " + ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteSubjectAsync()
    {
        if (Selected is null) return;

        var confirm = MessageBox.Show(
            $"Видалити предмет \"{Selected.Name}\"? (заняття цього предмета також буде видалено)",
            "Підтвердження",
            MessageBoxButton.YesNo);

        if (confirm != MessageBoxResult.Yes) return;

        IsBusy = true;
        try
        {
            await _service.DeleteSubjectAsync(Selected.Id);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка видалення: " + ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}