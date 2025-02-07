using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomUserLogin.Data.Migrations
{
    /// <inheritdoc />
    public partial class addfktoemployersTAble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployerDefaulters_Employers_employersId",
                table: "EmployerDefaulters");

            migrationBuilder.DropIndex(
                name: "IX_EmployerDefaulters_employersId",
                table: "EmployerDefaulters");

            migrationBuilder.DropColumn(
                name: "employersId",
                table: "EmployerDefaulters");

            migrationBuilder.CreateIndex(
                name: "IX_EmployerDefaulters_EmployerId",
                table: "EmployerDefaulters",
                column: "EmployerId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployerDefaulters_Employers_EmployerId",
                table: "EmployerDefaulters",
                column: "EmployerId",
                principalTable: "Employers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployerDefaulters_Employers_EmployerId",
                table: "EmployerDefaulters");

            migrationBuilder.DropIndex(
                name: "IX_EmployerDefaulters_EmployerId",
                table: "EmployerDefaulters");

            migrationBuilder.AddColumn<int>(
                name: "employersId",
                table: "EmployerDefaulters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EmployerDefaulters_employersId",
                table: "EmployerDefaulters",
                column: "employersId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployerDefaulters_Employers_employersId",
                table: "EmployerDefaulters",
                column: "employersId",
                principalTable: "Employers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
