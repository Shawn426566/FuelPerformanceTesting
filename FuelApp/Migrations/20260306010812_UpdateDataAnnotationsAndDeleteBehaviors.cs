using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuelApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDataAnnotationsAndDeleteBehaviors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Players_PlayerId",
                table: "Evaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Associations_AssociationID",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Teams_TeamID",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Associations_AssociationID",
                table: "Teams");

            migrationBuilder.AlterColumn<int>(
                name: "AssociationID",
                table: "Teams",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AssociationID",
                table: "Players",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Players_PlayerId",
                table: "Evaluations",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Associations_AssociationID",
                table: "Players",
                column: "AssociationID",
                principalTable: "Associations",
                principalColumn: "AssociationID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Teams_TeamID",
                table: "Players",
                column: "TeamID",
                principalTable: "Teams",
                principalColumn: "TeamID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Associations_AssociationID",
                table: "Teams",
                column: "AssociationID",
                principalTable: "Associations",
                principalColumn: "AssociationID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Players_PlayerId",
                table: "Evaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Associations_AssociationID",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Teams_TeamID",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Associations_AssociationID",
                table: "Teams");

            migrationBuilder.AlterColumn<int>(
                name: "AssociationID",
                table: "Teams",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AssociationID",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Players_PlayerId",
                table: "Evaluations",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Associations_AssociationID",
                table: "Players",
                column: "AssociationID",
                principalTable: "Associations",
                principalColumn: "AssociationID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Teams_TeamID",
                table: "Players",
                column: "TeamID",
                principalTable: "Teams",
                principalColumn: "TeamID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Associations_AssociationID",
                table: "Teams",
                column: "AssociationID",
                principalTable: "Associations",
                principalColumn: "AssociationID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
