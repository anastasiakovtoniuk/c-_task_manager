using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StudyManager.WpfApp.Navigation;

public sealed class NavigationService : INavigationService
{
    private readonly IServiceProvider _provider;
    private readonly Stack<object> _history = new();

    private object _currentViewModel;
    public object CurrentViewModel
    {
        get => _currentViewModel;
        private set
        {
            _currentViewModel = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanGoBack));
        }
    }

    public bool CanGoBack => _history.Count > 0;

    public NavigationService(IServiceProvider provider)
    {
        _provider = provider;
        _currentViewModel = new object(); // тимчасово
    }

    public void NavigateTo<TViewModel>(object? parameter = null) where TViewModel : class
    {
        if (CurrentViewModel is not null && CurrentViewModel.GetType() != typeof(object))
            _history.Push(CurrentViewModel);

        var vm = _provider.GetRequiredService(typeof(TViewModel));

        if (vm is IParameterReceiver receiver)
            receiver.Receive(parameter);

        CurrentViewModel = vm!;
    }

    public void GoBack()
    {
        if (!CanGoBack) return;
        CurrentViewModel = _history.Pop();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}