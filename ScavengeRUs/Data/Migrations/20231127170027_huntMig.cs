using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScavengeRUs.Data.Migrations
{
    public partial class huntMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed",
                table: "Location");

            migrationBuilder.AddColumn<string>(
                name: "InvitationBodyText",
                table: "Hunts",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvitationBodyText",
                table: "Hunts");

            migrationBuilder.AddColumn<string>(
                name: "Completed",
                table: "Location",
                type: "TEXT",
                nullable: true);
        }
    }
}
