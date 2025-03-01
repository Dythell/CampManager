using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGroupSessionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Season",
                table: "Sessions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Sessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Groups",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SessionCounselor_Id",
                table: "Groups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_SessionCounselor_Id",
                table: "Groups",
                column: "SessionCounselor_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventTemplateId",
                table: "Events",
                column: "EventTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventTemplates_EventTemplateId",
                table: "Events",
                column: "EventTemplateId",
                principalTable: "EventTemplates",
                principalColumn: "EventTemplate_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_SessionCounselors_SessionCounselor_Id",
                table: "Groups",
                column: "SessionCounselor_Id",
                principalTable: "SessionCounselors",
                principalColumn: "SessionCounselor_Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventTemplates_EventTemplateId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_SessionCounselors_SessionCounselor_Id",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_SessionCounselor_Id",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Events_EventTemplateId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "SessionCounselor_Id",
                table: "Groups");
        }
    }
}
