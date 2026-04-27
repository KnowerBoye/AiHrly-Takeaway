using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aihrly_api.Migrations
{
    /// <inheritdoc />
    public partial class removeDBEnumType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:application_note_type", "general,screening,interview,reference_check,red_flag")
                .OldAnnotation("Npgsql:Enum:application_stages", "applied,screening,interview,offer,hired,rejected")
                .OldAnnotation("Npgsql:Enum:score_dimension", "culture_fit,interview,assessment")
                .OldAnnotation("Npgsql:Enum:status", "open,closed")
                .OldAnnotation("Npgsql:Enum:team_member_role", "recruiter,hiring_manager");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:application_note_type", "general,screening,interview,reference_check,red_flag")
                .Annotation("Npgsql:Enum:application_stages", "applied,screening,interview,offer,hired,rejected")
                .Annotation("Npgsql:Enum:score_dimension", "culture_fit,interview,assessment")
                .Annotation("Npgsql:Enum:status", "open,closed")
                .Annotation("Npgsql:Enum:team_member_role", "recruiter,hiring_manager");
        }
    }
}
