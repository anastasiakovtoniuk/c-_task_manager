using Microsoft.Extensions.DependencyInjection;
using StudyManager.Repositories.Storage;

namespace StudyManager.Repositories;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IStudyStore, JsonFileStudyStore>(); 
        services.AddSingleton<IStudyRepository, StudyRepository>();
        return services;
    }
}