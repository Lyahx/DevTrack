using AutoMapper;
using DevTrack.Domain.Common;
using DevTrack.Domain.DTOs.Components;
using DevTrack.Domain.DTOs.Decisions;
using DevTrack.Domain.DTOs.Ideas;
using DevTrack.Domain.DTOs.LearningTracks;
using DevTrack.Domain.DTOs.NextSteps;
using DevTrack.Domain.DTOs.Projects;
using DevTrack.Domain.DTOs.Reminders;
using DevTrack.Domain.DTOs.Resources;
using DevTrack.Domain.DTOs.Tags;
using DevTrack.Domain.DTOs.Worklogs;
using DevTrack.Domain.Entities;

namespace DevTrack.Infrastructure.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Project, ProjectResponse>()
            .ForMember(d => d.Tags, o => o.MapFrom(s => s.ProjectTags.Select(pt => pt.Tag)));
        CreateMap<ProjectCreateRequest, Project>();
        CreateMap<ProjectUpdateRequest, Project>();

        CreateMap<Component, ComponentResponse>()
            .ForMember(d => d.Tags, o => o.MapFrom(s => s.ComponentTags.Select(ct => ct.Tag)));
        CreateMap<ComponentCreateRequest, Component>();
        CreateMap<ComponentUpdateRequest, Component>();

        CreateMap<LearningTrack, LearningTrackResponse>()
            .ForMember(d => d.Tags, o => o.MapFrom(s => s.LearningTrackTags.Select(lt => lt.Tag)));
        CreateMap<LearningTrackCreateRequest, LearningTrack>();
        CreateMap<LearningTrackUpdateRequest, LearningTrack>();

        CreateMap<LearningModule, LearningModuleResponse>();
        CreateMap<LearningModuleCreateRequest, LearningModule>();
        CreateMap<LearningModuleUpdateRequest, LearningModule>();

        CreateMap<Tag, TagResponse>();
        CreateMap<TagCreateRequest, Tag>();
        CreateMap<TagUpdateRequest, Tag>();

        CreateMap<Worklog, WorklogResponse>()
            .ForMember(d => d.Owner, o => o.MapFrom(s =>
                OwnerReference.FromColumns(s.ProjectId, s.ComponentId, s.LearningTrackId, s.LearningModuleId)));

        CreateMap<Decision, DecisionResponse>()
            .ForMember(d => d.Owner, o => o.MapFrom(s =>
                OwnerReference.FromColumns(s.ProjectId, s.ComponentId, s.LearningTrackId, s.LearningModuleId)));

        CreateMap<NextStep, NextStepResponse>()
            .ForMember(d => d.Owner, o => o.MapFrom(s =>
                OwnerReference.FromColumns(s.ProjectId, s.ComponentId, s.LearningTrackId, s.LearningModuleId)));

        CreateMap<Idea, IdeaResponse>()
            .ForMember(d => d.Owner, o => o.MapFrom(s =>
                OwnerReference.FromColumns(s.ProjectId, s.ComponentId, s.LearningTrackId, s.LearningModuleId)));

        CreateMap<Resource, ResourceResponse>()
            .ForMember(d => d.Owner, o => o.MapFrom(s =>
                OwnerReference.FromColumns(s.ProjectId, s.ComponentId, s.LearningTrackId, s.LearningModuleId)));

        CreateMap<Reminder, ReminderResponse>();
    }
}
