using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomUserLogin.Data.Migrations
{
    /// <inheritdoc />
    public partial class Paymentdetailsboolispaid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFullPaid",
                table: "PaymentDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFullPaid",
                table: "PaymentDetails");
        }
    }
}
