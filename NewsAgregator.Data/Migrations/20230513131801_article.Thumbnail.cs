using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsAgregator.Data.Migrations
{
    /// <inheritdoc />
    public partial class articleThumbnail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlThumbnail",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlThumbnail",
                table: "Articles");
        }
    }
}
