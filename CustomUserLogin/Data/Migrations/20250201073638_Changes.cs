using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomUserLogin.Data.Migrations
{
    /// <inheritdoc />
    public partial class Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SSNITEmployerNumber",
                table: "Employers",
                type: "nvarchar(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EnrollmentNumber",
                table: "Employers",
                type: "nvarchar(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Employers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "EmployerDefaulters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployerId = table.Column<int>(type: "int", nullable: false),
                    AffectedEmployees = table.Column<int>(type: "int", nullable: false),
                    ContributionMonth = table.Column<DateOnly>(type: "date", nullable: false),
                    MonthsRun = table.Column<int>(type: "int", nullable: false),
                    SurchargeDate = table.Column<DateOnly>(type: "date", nullable: false),
                    AmountDefaulted = table.Column<decimal>(type: "money", nullable: false),
                    SurchargeDue = table.Column<decimal>(type: "money", nullable: false),
                    TotalAmountDue = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployerDefaulters", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployerDefaulters");

            migrationBuilder.AlterColumn<string>(
                name: "SSNITEmployerNumber",
                table: "Employers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)");

            migrationBuilder.AlterColumn<string>(
                name: "EnrollmentNumber",
                table: "Employers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Employers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
