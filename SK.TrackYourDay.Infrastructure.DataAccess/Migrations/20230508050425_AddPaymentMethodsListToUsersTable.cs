using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SK.TrackYourDay.Infrastructure.DataAccess.Migrations
{
    public partial class AddPaymentMethodsListToUsersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "PaymentMethods",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_UserId",
                table: "PaymentMethods",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethods_AspNetUsers_UserId",
                table: "PaymentMethods",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethods_AspNetUsers_UserId",
                table: "PaymentMethods");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMethods_UserId",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PaymentMethods");
        }
    }
}
