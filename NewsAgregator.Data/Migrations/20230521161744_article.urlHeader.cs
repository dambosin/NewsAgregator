using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsAgregator.Data.Migrations
{
    /// <inheritdoc />
    public partial class articleurlHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlHeader",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlHeader",
                table: "Articles");
        }
    }
}
