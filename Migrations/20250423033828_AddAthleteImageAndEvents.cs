using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FantasySports.Migrations
{
    /// <inheritdoc />
    public partial class AddAthleteImageAndEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "DraftDate",
                table: "Leagues",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DraftOrder",
                table: "Leagues",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDraftComplete",
                table: "Leagues",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDraftStarted",
                table: "Leagues",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TradeDeadlineDays",
                table: "Leagues",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "LeagueMembers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "Athletes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "Athletes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AthleteEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Stats = table.Column<string>(type: "TEXT", nullable: false),
                    AthleteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AthleteEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AthleteEvent_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProposingTeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReceivingTeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    OfferedAthletes = table.Column<string>(type: "TEXT", nullable: false),
                    RequestedAthletes = table.Column<string>(type: "TEXT", nullable: false),
                    CounterOffer = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trades_Teams_ProposingTeamId",
                        column: x => x.ProposingTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trades_Teams_ReceivingTeamId",
                        column: x => x.ReceivingTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AthleteEvent_AthleteId",
                table: "AthleteEvent",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_ProposingTeamId",
                table: "Trades",
                column: "ProposingTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_ReceivingTeamId",
                table: "Trades",
                column: "ReceivingTeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AthleteEvent");

            migrationBuilder.DropTable(
                name: "Trades");

            migrationBuilder.DropColumn(
                name: "DraftDate",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "DraftOrder",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "IsDraftComplete",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "IsDraftStarted",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "TradeDeadlineDays",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "LeagueMembers");

            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "Athletes");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Athletes");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
