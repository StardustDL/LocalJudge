using Microsoft.EntityFrameworkCore.Migrations;

namespace StarOJ.Data.Provider.SqlServer.Migrations
{
    public partial class UseMaxTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalTime",
                table: "Submissions",
                newName: "MaximumTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaximumTime",
                table: "Submissions",
                newName: "TotalTime");
        }
    }
}
