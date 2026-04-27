using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aihrly_api.Migrations
{
    /// <inheritdoc />
    public partial class applicationCoverLetterField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "coverLetter",
                table: "Applications",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "coverLetter",
                table: "Applications");
        }
    }
}
