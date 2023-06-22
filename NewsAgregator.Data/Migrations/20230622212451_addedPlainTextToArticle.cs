using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsAgregator.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedPlainTextToArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Comments",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "getdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<double>(
                name: "PositiveIndex",
                table: "Articles",
                type: "float",
                nullable: false,
                defaultValue: -10.0,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "LikesCount",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Articles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "getdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "PlainText",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlainText",
                table: "Articles");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Comments",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "getdate()");

            migrationBuilder.AlterColumn<double>(
                name: "PositiveIndex",
                table: "Articles",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldDefaultValue: -10.0);

            migrationBuilder.AlterColumn<int>(
                name: "LikesCount",
                table: "Articles",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Articles",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "getdate()");
        }
    }
}
