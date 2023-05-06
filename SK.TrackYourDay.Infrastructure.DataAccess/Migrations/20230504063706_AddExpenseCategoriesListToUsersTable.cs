using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SK.TrackYourDay.Infrastructure.DataAccess.Migrations
{
    public partial class AddExpenseCategoriesListToUsersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ExpenseCategories",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_UserId",
                table: "ExpenseCategories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseCategories_AspNetUsers_UserId",
                table: "ExpenseCategories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseCategories_AspNetUsers_UserId",
                table: "ExpenseCategories");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseCategories_UserId",
                table: "ExpenseCategories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ExpenseCategories");
        }
    }
}
