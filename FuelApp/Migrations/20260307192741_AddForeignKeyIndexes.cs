using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuelApp.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeyIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Teams_AssociationID",
                table: "Teams",
                newName: "IX_Team_AssociationID");

            migrationBuilder.RenameIndex(
                name: "IX_StaffMembers_TeamID",
                table: "StaffMembers",
                newName: "IX_StaffMember_TeamID");

            migrationBuilder.RenameIndex(
                name: "IX_Players_TeamID",
                table: "Players",
                newName: "IX_Player_TeamID");

            migrationBuilder.RenameIndex(
                name: "IX_Players_AssociationID",
                table: "Players",
                newName: "IX_Player_AssociationID");

            migrationBuilder.RenameIndex(
                name: "IX_Evaluations_StaffMemberId",
                table: "Evaluations",
                newName: "IX_Evaluation_StaffMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Evaluations_PlayerId",
                table: "Evaluations",
                newName: "IX_Evaluation_PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Team_AssociationID",
                table: "Teams",
                newName: "IX_Teams_AssociationID");

            migrationBuilder.RenameIndex(
                name: "IX_StaffMember_TeamID",
                table: "StaffMembers",
                newName: "IX_StaffMembers_TeamID");

            migrationBuilder.RenameIndex(
                name: "IX_Player_TeamID",
                table: "Players",
                newName: "IX_Players_TeamID");

            migrationBuilder.RenameIndex(
                name: "IX_Player_AssociationID",
                table: "Players",
                newName: "IX_Players_AssociationID");

            migrationBuilder.RenameIndex(
                name: "IX_Evaluation_StaffMemberId",
                table: "Evaluations",
                newName: "IX_Evaluations_StaffMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Evaluation_PlayerId",
                table: "Evaluations",
                newName: "IX_Evaluations_PlayerId");
        }
    }
}
