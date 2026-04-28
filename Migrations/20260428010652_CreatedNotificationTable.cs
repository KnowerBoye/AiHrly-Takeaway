using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aihrly_api.Migrations
{
    /// <inheritdoc />
    public partial class CreatedNotificationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_Notifications_Applications_application_id",
                        column: x => x.application_id,
                        principalTable: "Applications",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_application_id",
                table: "Notifications",
                column: "application_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");
        }
    }
}
