using Microsoft.EntityFrameworkCore.Migrations;

namespace BlogAspNet.Migrations
{
    public partial class AddAppUserForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppUserFK",
                table: "Posts",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppUserFK",
                table: "Posts");
        }
    }
}
