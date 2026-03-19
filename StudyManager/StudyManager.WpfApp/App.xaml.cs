using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using StudyManager.Services;
using StudyManager.WpfApp.Pages;

namespace StudyManager.WpfApp;

public partial class App : Application
{
    private IServiceProvider _serviceProvider = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        // Реєстрація сервісів даних (IoC/DI)
        services.AddStudyManagerServices();

        // Реєстрація UI
        services.AddSingleton<MainWindow>();
        services.AddTransient<SubjectsPage>();

        services.AddTransient<Func<Guid, SubjectDetailsPage>>(sp =>
            subjectId => new SubjectDetailsPage(
                sp.GetRequiredService<IStudyRepository>(),
                subjectId,
                sp.GetRequiredService<Func<Guid, LessonDetailsPage>>()));

        services.AddTransient<Func<Guid, LessonDetailsPage>>(sp =>
            lessonId => new LessonDetailsPage(
                sp.GetRequiredService<IStudyRepository>(),
                lessonId));

        _serviceProvider = services.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();

        // Стартова навігація на список предметів
        mainWindow.Navigate(_serviceProvider.GetRequiredService<SubjectsPage>());
    }
}