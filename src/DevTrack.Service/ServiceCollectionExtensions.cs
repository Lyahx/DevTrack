using DevTrack.Service.Configuration;
using DevTrack.Service.Implementations;
using DevTrack.Service.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DevTrack.Service;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDevTrackServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AiSettings>(configuration.GetSection(AiSettings.SectionName));
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IComponentService, ComponentService>();
        services.AddScoped<ILearningTrackService, LearningTrackService>();
        services.AddScoped<ILearningModuleService, LearningModuleService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IOwnerValidator, OwnerValidator>();
        services.AddScoped<IActivityTrackingService, ActivityTrackingService>();
        services.AddScoped<IWorklogService, WorklogService>();
        services.AddScoped<IDecisionService, DecisionService>();
        services.AddScoped<INextStepService, NextStepService>();
        services.AddScoped<IIdeaService, IdeaService>();
        services.AddScoped<IResourceService, ResourceService>();
        services.AddScoped<IReminderService, ReminderService>();
        services.AddScoped<IResumeService, ResumeService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IQuickCaptureService, QuickCaptureService>();
        services.AddScoped<IReminderGenerator, ReminderGenerator>();
        services.AddScoped<ICommitService, GitHubCommitService>();
        services.AddScoped<IAiExtractionService, AiExtractionService>();
        services.AddScoped<IAiImportService, AiImportService>();
        services.AddMemoryCache();
        services.AddHttpClient(GitHubCommitService.HttpClientName, c =>
        {
            c.Timeout = TimeSpan.FromSeconds(10);
            c.DefaultRequestHeaders.UserAgent.ParseAdd("DevTrack/1.0");
            c.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
        });
        services.AddHttpClient(AiExtractionService.HttpClientName);

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
