using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevTrack.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Decisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Reasoning = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Alternatives = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DecidedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    ComponentId = table.Column<int>(type: "int", nullable: true),
                    LearningTrackId = table.Column<int>(type: "int", nullable: true),
                    LearningModuleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decisions", x => x.Id);
                    table.CheckConstraint("CK_Decisions_Owner_ExactlyOne", "(CASE WHEN [ProjectId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [ComponentId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [LearningTrackId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [LearningModuleId] IS NULL THEN 0 ELSE 1 END) = 1");
                    table.ForeignKey(
                        name: "FK_Decisions_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Decisions_LearningModules_LearningModuleId",
                        column: x => x.LearningModuleId,
                        principalTable: "LearningModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Decisions_LearningTracks_LearningTrackId",
                        column: x => x.LearningTrackId,
                        principalTable: "LearningTracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Decisions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Decisions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NextSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    ComponentId = table.Column<int>(type: "int", nullable: true),
                    LearningTrackId = table.Column<int>(type: "int", nullable: true),
                    LearningModuleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NextSteps", x => x.Id);
                    table.CheckConstraint("CK_NextSteps_Owner_ExactlyOne", "(CASE WHEN [ProjectId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [ComponentId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [LearningTrackId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [LearningModuleId] IS NULL THEN 0 ELSE 1 END) = 1");
                    table.ForeignKey(
                        name: "FK_NextSteps_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NextSteps_LearningModules_LearningModuleId",
                        column: x => x.LearningModuleId,
                        principalTable: "LearningModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NextSteps_LearningTracks_LearningTrackId",
                        column: x => x.LearningTrackId,
                        principalTable: "LearningTracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NextSteps_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NextSteps_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reminders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RelatedProjectId = table.Column<int>(type: "int", nullable: true),
                    RelatedLearningTrackId = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    IsDismissed = table.Column<bool>(type: "bit", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reminders_LearningTracks_RelatedLearningTrackId",
                        column: x => x.RelatedLearningTrackId,
                        principalTable: "LearningTracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reminders_Projects_RelatedProjectId",
                        column: x => x.RelatedProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reminders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    ComponentId = table.Column<int>(type: "int", nullable: true),
                    LearningTrackId = table.Column<int>(type: "int", nullable: true),
                    LearningModuleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                    table.CheckConstraint("CK_Resources_Owner_ExactlyOne", "(CASE WHEN [ProjectId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [ComponentId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [LearningTrackId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [LearningModuleId] IS NULL THEN 0 ELSE 1 END) = 1");
                    table.ForeignKey(
                        name: "FK_Resources_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Resources_LearningModules_LearningModuleId",
                        column: x => x.LearningModuleId,
                        principalTable: "LearningModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Resources_LearningTracks_LearningTrackId",
                        column: x => x.LearningTrackId,
                        principalTable: "LearningTracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Resources_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Resources_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Worklogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WhatIDid = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    WhatsLeft = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    LoggedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    ComponentId = table.Column<int>(type: "int", nullable: true),
                    LearningTrackId = table.Column<int>(type: "int", nullable: true),
                    LearningModuleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worklogs", x => x.Id);
                    table.CheckConstraint("CK_Worklogs_Owner_ExactlyOne", "(CASE WHEN [ProjectId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [ComponentId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [LearningTrackId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [LearningModuleId] IS NULL THEN 0 ELSE 1 END) = 1");
                    table.ForeignKey(
                        name: "FK_Worklogs_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Worklogs_LearningModules_LearningModuleId",
                        column: x => x.LearningModuleId,
                        principalTable: "LearningModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Worklogs_LearningTracks_LearningTrackId",
                        column: x => x.LearningTrackId,
                        principalTable: "LearningTracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Worklogs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Worklogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ideas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsConvertedToNextStep = table.Column<bool>(type: "bit", nullable: false),
                    ConvertedNextStepId = table.Column<int>(type: "int", nullable: true),
                    CapturedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    ComponentId = table.Column<int>(type: "int", nullable: true),
                    LearningTrackId = table.Column<int>(type: "int", nullable: true),
                    LearningModuleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ideas", x => x.Id);
                    table.CheckConstraint("CK_Ideas_Owner_ExactlyOne", "(CASE WHEN [ProjectId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [ComponentId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [LearningTrackId] IS NULL THEN 0 ELSE 1 END + CASE WHEN [LearningModuleId] IS NULL THEN 0 ELSE 1 END) = 1");
                    table.ForeignKey(
                        name: "FK_Ideas_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ideas_LearningModules_LearningModuleId",
                        column: x => x.LearningModuleId,
                        principalTable: "LearningModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ideas_LearningTracks_LearningTrackId",
                        column: x => x.LearningTrackId,
                        principalTable: "LearningTracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ideas_NextSteps_ConvertedNextStepId",
                        column: x => x.ConvertedNextStepId,
                        principalTable: "NextSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ideas_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ideas_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Decisions_ComponentId",
                table: "Decisions",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_Decisions_DecidedAt",
                table: "Decisions",
                column: "DecidedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Decisions_LearningModuleId",
                table: "Decisions",
                column: "LearningModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Decisions_LearningTrackId",
                table: "Decisions",
                column: "LearningTrackId");

            migrationBuilder.CreateIndex(
                name: "IX_Decisions_ProjectId",
                table: "Decisions",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Decisions_UserId",
                table: "Decisions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Ideas_ComponentId",
                table: "Ideas",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_Ideas_ConvertedNextStepId",
                table: "Ideas",
                column: "ConvertedNextStepId");

            migrationBuilder.CreateIndex(
                name: "IX_Ideas_IsConvertedToNextStep",
                table: "Ideas",
                column: "IsConvertedToNextStep");

            migrationBuilder.CreateIndex(
                name: "IX_Ideas_LearningModuleId",
                table: "Ideas",
                column: "LearningModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Ideas_LearningTrackId",
                table: "Ideas",
                column: "LearningTrackId");

            migrationBuilder.CreateIndex(
                name: "IX_Ideas_ProjectId",
                table: "Ideas",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Ideas_UserId",
                table: "Ideas",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NextSteps_ComponentId",
                table: "NextSteps",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_NextSteps_IsCompleted",
                table: "NextSteps",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_NextSteps_LearningModuleId",
                table: "NextSteps",
                column: "LearningModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_NextSteps_LearningTrackId",
                table: "NextSteps",
                column: "LearningTrackId");

            migrationBuilder.CreateIndex(
                name: "IX_NextSteps_Priority",
                table: "NextSteps",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_NextSteps_ProjectId",
                table: "NextSteps",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_NextSteps_UserId",
                table: "NextSteps",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_GeneratedAt",
                table: "Reminders",
                column: "GeneratedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_IsDismissed",
                table: "Reminders",
                column: "IsDismissed");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_IsRead",
                table: "Reminders",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_RelatedLearningTrackId",
                table: "Reminders",
                column: "RelatedLearningTrackId");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_RelatedProjectId",
                table: "Reminders",
                column: "RelatedProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_UserId",
                table: "Reminders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_ComponentId",
                table: "Resources",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_LearningModuleId",
                table: "Resources",
                column: "LearningModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_LearningTrackId",
                table: "Resources",
                column: "LearningTrackId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_ProjectId",
                table: "Resources",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_Type",
                table: "Resources",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_UserId",
                table: "Resources",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Worklogs_ComponentId",
                table: "Worklogs",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_Worklogs_LearningModuleId",
                table: "Worklogs",
                column: "LearningModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Worklogs_LearningTrackId",
                table: "Worklogs",
                column: "LearningTrackId");

            migrationBuilder.CreateIndex(
                name: "IX_Worklogs_LoggedAt",
                table: "Worklogs",
                column: "LoggedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Worklogs_ProjectId",
                table: "Worklogs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Worklogs_UserId",
                table: "Worklogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Decisions");

            migrationBuilder.DropTable(
                name: "Ideas");

            migrationBuilder.DropTable(
                name: "Reminders");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "Worklogs");

            migrationBuilder.DropTable(
                name: "NextSteps");
        }
    }
}
