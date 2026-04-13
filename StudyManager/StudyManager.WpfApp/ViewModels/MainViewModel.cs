using System.ComponentModel;
using StudyManager.WpfApp.Infrastructure;
using StudyManager.WpfApp.Navigation;

namespace StudyManager.WpfApp.ViewModels;

public sealed class MainViewModel : BaseViewModel
{
    private readonly INavigationService _nav;

    public object CurrentViewModel => _nav.CurrentViewModel;
    public RelayCommand BackCommand { get; }

    public bool CanGoBack => _nav.CanGoBack;

    public MainViewModel(INavigationService nav)
    {
        _nav = nav;

        BackCommand = new RelayCommand(
            execute: () => _nav.GoBack(),
            canExecute: () => _nav.CanGoBack);

        _nav.PropertyChanged += Nav_PropertyChanged;
    }

    private void Nav_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(INavigationService.CurrentViewModel))
            OnPropertyChanged(nameof(CurrentViewModel));

        if (e.PropertyName == nameof(INavigationService.CanGoBack))
        {
            OnPropertyChanged(nameof(CanGoBack));
            BackCommand.RaiseCanExecuteChanged();
        }
    }
}