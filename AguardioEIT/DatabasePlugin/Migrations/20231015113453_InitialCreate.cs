using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DatabasePlugin.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeakSensorData",
                columns: table => new
                {
                    DataRawId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DReported = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DLifeTimeUseCount = table.Column<int>(type: "integer", nullable: false),
                    LeakLevelId = table.Column<int>(type: "integer", nullable: false),
                    SensorId = table.Column<int>(type: "integer", nullable: false),
                    DTemperatureOut = table.Column<double>(type: "double precision", nullable: false),
                    DTemperatureIn = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeakSensorData", x => x.DataRawId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeakSensorData");
        }
    }
}
