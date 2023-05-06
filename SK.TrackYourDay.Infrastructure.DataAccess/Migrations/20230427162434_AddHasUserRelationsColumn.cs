using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SK.TrackYourDay.Infrastructure.DataAccess.Migrations
{
    public partial class AddHasUserRelationsColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasUserRelations",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasUserRelations",
                table: "AspNetUsers");
        }
    }
}
