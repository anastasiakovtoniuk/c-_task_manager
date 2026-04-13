using Microsoft.Extensions.DependencyInjection;
using StudyManager.Services.Implementation;
using StudyManager.Services.Interfaces;

namespace StudyManager.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStudyServices(this IServiceCollection services)
    {
        services.AddSingleton<IStudyService, StudyService>();
        return services;
    }
}