using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FantasySports.Migrations
{
    /// <inheritdoc />
    public partial class NewMigrationStandards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Athletes_Leagues_LeagueId",
                table: "Athletes");

            migrationBuilder.DropForeignKey(
                name: "FK_Athletes_Teams_TeamId",
                table: "Athletes");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_AspNetUsers_UserId",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Leagues_LeagueId",
                table: "Teams");

            migrationBuilder.DropTable(
                name: "Picks");

            migrationBuilder.DropIndex(
                name: "IX_Athletes_LeagueId",
                table: "Athletes");

            migrationBuilder.DropColumn(
                name: "DraftStatus",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "LeagueId",
                table: "Athletes");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Teams",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Teams_UserId",
                table: "Teams",
                newName: "IX_Teams_OwnerId");

            migrationBuilder.RenameColumn(
                name: "Event",
                table: "Athletes",
                newName: "Sport");

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
                name: "CreatorId",
                table: "Leagues",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "LeagueMembers",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LeagueId = table.Column<int>(type: "INTEGER", nullable: false),
                    JoinedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueMembers", x => new { x.UserId, x.LeagueId });
                    table.ForeignKey(
                        name: "FK_LeagueMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeagueMembers_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Leagues_CreatorId",
                table: "Leagues",
                column: "CreatorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeagueMembers_LeagueId",
                table: "LeagueMembers",
                column: "LeagueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Athletes_Teams_TeamId",
                table: "Athletes",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Leagues_AspNetUsers_CreatorId",
                table: "Leagues",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_AspNetUsers_OwnerId",
                table: "Teams",
                column: "OwnerId",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Athletes_Teams_TeamId",
                table: "Athletes");

            migrationBuilder.DropForeignKey(
                name: "FK_Leagues_AspNetUsers_CreatorId",
                table: "Leagues");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_AspNetUsers_OwnerId",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Leagues_LeagueId",
                table: "Teams");

            migrationBuilder.DropTable(
                name: "LeagueMembers");

            migrationBuilder.DropIndex(
                name: "IX_Leagues_CreatorId",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Teams",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Teams_OwnerId",
                table: "Teams",
                newName: "IX_Teams_UserId");

            migrationBuilder.RenameColumn(
                name: "Sport",
                table: "Athletes",
                newName: "Event");

            migrationBuilder.AlterColumn<int>(
                name: "LeagueId",
                table: "Teams",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "DraftStatus",
                table: "Leagues",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LeagueId",
                table: "Athletes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Picks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AthleteId = table.Column<int>(type: "INTEGER", nullable: false),
                    LeagueId = table.Column<int>(type: "INTEGER", nullable: false),
                    PickNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Picks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Athletes_LeagueId",
                table: "Athletes",
                column: "LeagueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Athletes_Leagues_LeagueId",
                table: "Athletes",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Athletes_Teams_TeamId",
                table: "Athletes",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_AspNetUsers_UserId",
                table: "Teams",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Leagues_LeagueId",
                table: "Teams",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id");
        }
    }
}
