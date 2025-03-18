using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppWithHighSecurity.Data.Migrations
{
    /// <inheritdoc />
    public partial class JohnsLangeFinger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCprNumbers_ApplicationUser_UserId1",
                table: "UserCprNumbers");

            migrationBuilder.DropIndex(
                name: "IX_UserCprNumbers_UserId1",
                table: "UserCprNumbers");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserCprNumbers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "UserCprNumbers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserCprNumbers_UserId1",
                table: "UserCprNumbers",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCprNumbers_ApplicationUser_UserId1",
                table: "UserCprNumbers",
                column: "UserId1",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
