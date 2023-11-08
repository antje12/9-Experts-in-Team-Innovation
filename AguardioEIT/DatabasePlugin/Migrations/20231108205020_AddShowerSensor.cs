using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DatabasePlugin.Migrations
{
    /// <inheritdoc />
    public partial class AddShowerSensor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShowerSensorData",
                columns: table => new
                {
                    DataRawId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DReported = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DShowerState = table.Column<int>(type: "integer", nullable: false),
                    DTemperature = table.Column<float>(type: "real", nullable: false),
                    DHumidity = table.Column<int>(type: "integer", nullable: false),
                    DBattery = table.Column<int>(type: "integer", nullable: false),
                    SensorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowerSensorData", x => x.DataRawId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShowerSensorData");
        }
    }
}
