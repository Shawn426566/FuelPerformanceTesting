using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuelApp.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceEvaluationDateIndexWithComposite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Evaluation_Date_EvaluationId",
                table: "Evaluations",
                columns: new[] { "Date", "EvaluationId" },
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Evaluation_Date_EvaluationId",
                table: "Evaluations");
        }
    }
}
