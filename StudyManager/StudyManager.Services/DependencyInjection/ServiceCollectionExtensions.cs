using Microsoft.Extensions.DependencyInjection;

namespace StudyManager.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStudyManagerServices(this IServiceCollection services)
    {
        services.AddSingleton<IStudyStore, FakeStudyStore>();
        services.AddSingleton<IStudyRepository, StudyRepository>();
        return services;
    }
}