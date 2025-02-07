using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomUserLogin.Data.Migrations
{
    /// <inheritdoc />
    public partial class Paymentplan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.CreateTable(
                name: "PaymentPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Installments = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<float>(type: "real", nullable: false),
                    Reasons = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EmployerDefaulterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentPlans_EmployerDefaulters_EmployerDefaulterId",
                        column: x => x.EmployerDefaulterId,
                        principalTable: "EmployerDefaulters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentPlans_EmployerDefaulterId",
                table: "PaymentPlans",
                column: "EmployerDefaulterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentPlans");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EmployerDefaulters");
        }
    }
}
