using DevTrack.Repository.Common;
using DevTrack.Repository.Implementations;
using DevTrack.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DevTrack.Repository;

public static class RepositoryServiceCollectionExtensions
{
    public static IServiceCollection AddDevTrackRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IComponentRepository, ComponentRepository>();
        services.AddScoped<ILearningTrackRepository, LearningTrackRepository>();
        services.AddScoped<ILearningModuleRepository, LearningModuleRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IWorklogRepository, WorklogRepository>();
        services.AddScoped<INextStepRepository, NextStepRepository>();
        services.AddScoped<IIdeaRepository, IdeaRepository>();
        services.AddScoped<IResourceRepository, ResourceRepository>();
        services.AddScoped<IReminderRepository, ReminderRepository>();
        services.AddScoped<ITransactionFactory, TransactionFactory>();
        return services;
    }
}
