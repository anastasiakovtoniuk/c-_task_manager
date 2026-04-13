using System.ComponentModel;

namespace StudyManager.WpfApp.Navigation;

public interface INavigationService : INotifyPropertyChanged
{
    object CurrentViewModel { get; }
    bool CanGoBack { get; }

    void NavigateTo<TViewModel>(object? parameter = null) where TViewModel : class;
    void GoBack();
}