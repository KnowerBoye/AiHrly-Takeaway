using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aihrly_api.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationScoreTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationScore_Applications_applicationId",
                table: "ApplicationScore");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationScore_TeamMembers_updatedBy",
                table: "ApplicationScore");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationScore",
                table: "ApplicationScore");

            migrationBuilder.RenameTable(
                name: "ApplicationScore",
                newName: "ApplicationScores");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationScore_updatedBy",
                table: "ApplicationScores",
                newName: "IX_ApplicationScores_updatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationScore_applicationId_dimension",
                table: "ApplicationScores",
                newName: "IX_ApplicationScores_applicationId_dimension");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationScores",
                table: "ApplicationScores",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationScores_Applications_applicationId",
                table: "ApplicationScores",
                column: "applicationId",
                principalTable: "Applications",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationScores_TeamMembers_updatedBy",
                table: "ApplicationScores",
                column: "updatedBy",
                principalTable: "TeamMembers",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationScores_Applications_applicationId",
                table: "ApplicationScores");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationScores_TeamMembers_updatedBy",
                table: "ApplicationScores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationScores",
                table: "ApplicationScores");

            migrationBuilder.RenameTable(
                name: "ApplicationScores",
                newName: "ApplicationScore");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationScores_updatedBy",
                table: "ApplicationScore",
                newName: "IX_ApplicationScore_updatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationScores_applicationId_dimension",
                table: "ApplicationScore",
                newName: "IX_ApplicationScore_applicationId_dimension");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationScore",
                table: "ApplicationScore",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationScore_Applications_applicationId",
                table: "ApplicationScore",
                column: "applicationId",
                principalTable: "Applications",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationScore_TeamMembers_updatedBy",
                table: "ApplicationScore",
                column: "updatedBy",
                principalTable: "TeamMembers",
                principalColumn: "id");
        }
    }
}
