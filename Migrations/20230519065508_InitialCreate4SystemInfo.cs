using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class InitialCreate4SystemInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "NoOfTimesAssigned",
                table: "Schedules",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "NoOfTimesDropped",
                table: "Schedules",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateTable(
                name: "SystemInformation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NoOfEmailsSentDayily = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemInformation", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemInformation");

            migrationBuilder.DropColumn(
                name: "NoOfTimesAssigned",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "NoOfTimesDropped",
                table: "Schedules");
        }
    }
}
