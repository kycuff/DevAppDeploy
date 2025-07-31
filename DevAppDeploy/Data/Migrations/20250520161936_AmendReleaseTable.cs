using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevAppDeploy.Migrations
{
    /// <inheritdoc />
    public partial class AmendReleaseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileExtension",
                table: "Releases",
                newName: "FileUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileUrl",
                table: "Releases",
                newName: "FileExtension");
        }
    }
}
