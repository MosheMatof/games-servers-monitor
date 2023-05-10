using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamesServersMonitor.Infrastructure.Migrations
{
    public partial class GamesServersMointorDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameServers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentPlayers = table.Column<int>(type: "int", nullable: false),
                    PlayersCapacity = table.Column<int>(type: "int", nullable: false),
                    GameMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HighScore = table.Column<float>(type: "real", nullable: false),
                    AvgScore = table.Column<float>(type: "real", nullable: false),
                    IsRunning = table.Column<bool>(type: "bit", nullable: false),
                    CpuTemperature = table.Column<float>(type: "real", nullable: false),
                    CpuSpeed = table.Column<float>(type: "real", nullable: false),
                    MemoryUsage = table.Column<float>(type: "real", nullable: false),
                    MemoryCapacity = table.Column<float>(type: "real", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameServers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerUpdates",
                columns: table => new
                {
                    ServerId = table.Column<int>(type: "int", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentPlayers = table.Column<int>(type: "int", nullable: false),
                    IsRunning = table.Column<bool>(type: "bit", nullable: false),
                    CpuTemperature = table.Column<float>(type: "real", nullable: false),
                    CpuSpeed = table.Column<float>(type: "real", nullable: false),
                    HighScore = table.Column<float>(type: "real", nullable: false),
                    AvgScore = table.Column<float>(type: "real", nullable: false),
                    MemoryUsage = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerUpdates", x => new { x.ServerId, x.TimeStamp });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameServers");

            migrationBuilder.DropTable(
                name: "ServerUpdates");
        }
    }
}
