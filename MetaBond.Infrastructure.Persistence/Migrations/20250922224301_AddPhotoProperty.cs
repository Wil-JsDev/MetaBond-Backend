using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetaBond.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "Communities",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "Communities",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Communities");
        }
    }
}
