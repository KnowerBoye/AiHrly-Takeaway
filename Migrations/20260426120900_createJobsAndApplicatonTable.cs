using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aihrly_api.Migrations
{
    /// <inheritdoc />
    public partial class createJobsAndApplicatonTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TeamMembers",
                newName: "id");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:application_stages.application_stages", "applied,screening,interview,offer,hired,rejected")
                .Annotation("Npgsql:Enum:job_status.status", "open,closed")
                .Annotation("Npgsql:Enum:team_member_role.team_member_role", "recruiter,hiring_manager");

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    location = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    jobId = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    current_stage = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.id);
                    table.ForeignKey(
                        name: "FK_Applications_Jobs_jobId",
                        column: x => x.jobId,
                        principalTable: "Jobs",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ApplicationNotes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    applicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationNotes", x => x.id);
                    table.ForeignKey(
                        name: "FK_ApplicationNotes_Applications_applicationId",
                        column: x => x.applicationId,
                        principalTable: "Applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationNotes_TeamMembers_created_by",
                        column: x => x.created_by,
                        principalTable: "TeamMembers",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ApplicationStageHistories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    applicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    from_stage = table.Column<int>(type: "integer", nullable: false),
                    to_stage = table.Column<int>(type: "integer", nullable: false),
                    changed_by = table.Column<Guid>(type: "uuid", nullable: false),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationStageHistories", x => x.id);
                    table.ForeignKey(
                        name: "FK_ApplicationStageHistories_Applications_applicationId",
                        column: x => x.applicationId,
                        principalTable: "Applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationStageHistories_TeamMembers_changed_by",
                        column: x => x.changed_by,
                        principalTable: "TeamMembers",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationNotes_applicationId",
                table: "ApplicationNotes",
                column: "applicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationNotes_created_by",
                table: "ApplicationNotes",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_current_stage",
                table: "Applications",
                column: "current_stage");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_email_jobId",
                table: "Applications",
                columns: new[] { "email", "jobId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_jobId",
                table: "Applications",
                column: "jobId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationStageHistories_applicationId",
                table: "ApplicationStageHistories",
                column: "applicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationStageHistories_changed_by",
                table: "ApplicationStageHistories",
                column: "changed_by");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_status",
                table: "Jobs",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationNotes");

            migrationBuilder.DropTable(
                name: "ApplicationStageHistories");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "TeamMembers",
                newName: "Id");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:application_stages.application_stages", "applied,screening,interview,offer,hired,rejected")
                .OldAnnotation("Npgsql:Enum:job_status.status", "open,closed")
                .OldAnnotation("Npgsql:Enum:team_member_role.team_member_role", "recruiter,hiring_manager");
        }
    }
}
