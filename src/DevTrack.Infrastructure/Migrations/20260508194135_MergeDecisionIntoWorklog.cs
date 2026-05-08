using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DevTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MergeDecisionIntoWorklog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add new columns to Worklogs first.
            migrationBuilder.AddColumn<string>(
                name: "Alternatives",
                table: "Worklogs",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reasoning",
                table: "Worklogs",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            // 2. Migrate every Decision row into a Worklog row, preserving owner FKs,
            //    soft-delete state, and timestamps. Title becomes the worklog headline.
            migrationBuilder.Sql(@"
                INSERT INTO ""Worklogs""
                    (""UserId"", ""ProjectId"", ""ComponentId"", ""LearningTrackId"", ""LearningModuleId"",
                     ""WhatIDid"", ""Reasoning"", ""Alternatives"", ""LoggedAt"",
                     ""CreatedAt"", ""UpdatedAt"", ""IsDeleted"", ""DeletedAt"")
                SELECT
                    d.""UserId"", d.""ProjectId"", d.""ComponentId"", d.""LearningTrackId"", d.""LearningModuleId"",
                    d.""Title"",
                    NULLIF(d.""Reasoning"", ''),
                    d.""Alternatives"",
                    d.""DecidedAt"",
                    d.""CreatedAt"", d.""UpdatedAt"", d.""IsDeleted"", d.""DeletedAt""
                FROM ""Decisions"" d;
            ");

            // 3. Drop the Decisions table — its data now lives in Worklogs.
            migrationBuilder.DropTable(
                name: "Decisions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alternatives",
                table: "Worklogs");

            migrationBuilder.DropColumn(
                name: "Reasoning",
                table: "Worklogs");

            migrationBuilder.CreateTable(
                name: "Decisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ComponentId = table.Column<int>(type: "integer", nullable: true),
                    LearningModuleId = table.Column<int>(type: "integer", nullable: true),
                    LearningTrackId = table.Column<int>(type: "integer", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Alternatives = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DecidedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Reasoning = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decisions", x => x.Id);
                    table.CheckConstraint("CK_Decisions_Owner_ExactlyOne", "(CASE WHEN \"ProjectId\" IS NULL THEN 0 ELSE 1 END + CASE WHEN \"ComponentId\" IS NULL THEN 0 ELSE 1 END + CASE WHEN \"LearningTrackId\" IS NULL THEN 0 ELSE 1 END + CASE WHEN \"LearningModuleId\" IS NULL THEN 0 ELSE 1 END) = 1");
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
        }
    }
}
