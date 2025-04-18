using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FantasySports.Migrations
{
    /// <inheritdoc />
    public partial class FixTeamRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_AspNetUsers_UserId1",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Leagues_LeagueId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_UserId1",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Teams");

            migrationBuilder.AlterColumn<int>(
                name: "LeagueId",
                table: "Teams",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Leagues_LeagueId",
                table: "Teams",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Leagues_LeagueId",
                table: "Teams");

            migrationBuilder.AlterColumn<int>(
                name: "LeagueId",
                table: "Teams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Teams",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_UserId1",
                table: "Teams",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_AspNetUsers_UserId1",
                table: "Teams",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Leagues_LeagueId",
                table: "Teams",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
