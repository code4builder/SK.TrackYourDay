using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SK.TrackYourDay.Infrastructure.DataAccess.Migrations
{
    public partial class AddExpenseCategoriesListToUsersTableNoCascadeDel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseCategories_AspNetUsers_UserId",
                table: "ExpenseCategories");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseCategories_AspNetUsers_UserId",
                table: "ExpenseCategories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseCategories_AspNetUsers_UserId",
                table: "ExpenseCategories");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseCategories_AspNetUsers_UserId",
                table: "ExpenseCategories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
