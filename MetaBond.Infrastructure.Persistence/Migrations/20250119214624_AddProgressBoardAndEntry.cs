using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetaBond.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProgressBoardAndEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgressBoard",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommunitiesId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkProgressBoard", x => x.Id);
                    table.ForeignKey(
                        name: "FkCommunitiesId",
                        column: x => x.CommunitiesId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgressEntry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProgressBoardId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkProgressEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FkProgressBoardId",
                        column: x => x.ProgressBoardId,
                        principalTable: "ProgressBoard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgressBoard_CommunitiesId",
                table: "ProgressBoard",
                column: "CommunitiesId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgressEntry_ProgressBoardId",
                table: "ProgressEntry",
                column: "ProgressBoardId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FkCommunitiesId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "ProgressEntry");

            migrationBuilder.DropTable(
                name: "ProgressBoard");
        }
    }
}
