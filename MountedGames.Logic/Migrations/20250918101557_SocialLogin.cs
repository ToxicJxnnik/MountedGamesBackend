using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MountedGames.Logic.Migrations
{
    /// <inheritdoc />
    public partial class SocialLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Auth0UserId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SocialProvider",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Auth0UserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SocialProvider",
                table: "Users");
        }
    }
}
