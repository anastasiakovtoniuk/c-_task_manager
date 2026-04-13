using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using StudyManager.Services;            // AddStudyServices()
using StudyManager.Repositories;        // AddRepositories() (коли зробиш)
using StudyManager.WpfApp.Navigation;
using StudyManager.WpfApp.ViewModels;

namespace StudyManager.WpfApp;

public partial class App : Application
{
    private ServiceProvider _provider = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        // нижні шари
        services.AddRepositories();   // реєстрація repo/store
        services.AddStudyServices();  // реєстрація IStudyService

        // навігація
        services.AddSingleton<INavigationService, NavigationService>();

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddTransient<SubjectsListViewModel>();
        services.AddTransient<SubjectDetailsViewModel>();
        services.AddTransient<LessonDetailsViewModel>();

        // MainWindow
        services.AddSingleton<MainWindow>();

        _provider = services.BuildServiceProvider();

        var nav = _provider.GetRequiredService<INavigationService>();
        nav.NavigateTo<SubjectsListViewModel>();

        var window = _provider.GetRequiredService<MainWindow>();
        window.DataContext = _provider.GetRequiredService<MainViewModel>();
        window.Show();
    }
}