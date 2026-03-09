using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuelApp.Migrations
{
    /// <inheritdoc />
    public partial class AddEvaluationDateIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Evaluation_Date",
                table: "Evaluations",
                column: "Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Evaluation_Date",
                table: "Evaluations");
        }
    }
}
