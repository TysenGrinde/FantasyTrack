using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FantasySports.Migrations
{
    /// <inheritdoc />
    public partial class AddTradeSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TradeDeadlineDays",
                table: "Leagues",
                newName: "TradeReviewPeriodHours");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Trades",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "LeagueId",
                table: "Trades",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Leagues",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TradeLimit",
                table: "Leagues",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "LeagueMembers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SlotNumber",
                table: "Athletes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MeetSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LeagueId = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeetSchedules_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trades_LeagueId",
                table: "Trades",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_LeagueMembers_TeamId",
                table: "LeagueMembers",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetSchedules_LeagueId",
                table: "MeetSchedules",
                column: "LeagueId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueMembers_Teams_TeamId",
                table: "LeagueMembers",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trades_Leagues_LeagueId",
                table: "Trades",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeagueMembers_Teams_TeamId",
                table: "LeagueMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Trades_Leagues_LeagueId",
                table: "Trades");

            migrationBuilder.DropTable(
                name: "MeetSchedules");

            migrationBuilder.DropIndex(
                name: "IX_Trades_LeagueId",
                table: "Trades");

            migrationBuilder.DropIndex(
                name: "IX_LeagueMembers_TeamId",
                table: "LeagueMembers");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "LeagueId",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "TradeLimit",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "LeagueMembers");

            migrationBuilder.DropColumn(
                name: "SlotNumber",
                table: "Athletes");

            migrationBuilder.RenameColumn(
                name: "TradeReviewPeriodHours",
                table: "Leagues",
                newName: "TradeDeadlineDays");
        }
    }
}
