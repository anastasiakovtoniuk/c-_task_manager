using Microsoft.Extensions.DependencyInjection;

namespace StudyManager.Repositories;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IStudyStore, FakeStudyStore>();
        services.AddSingleton<IStudyRepository, StudyRepository>();
        return services;
    }
}