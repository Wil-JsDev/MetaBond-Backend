using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetaBond.Infrastructure.Persistence.Migrations
{
    public partial class AddInterestCategoryAndCommunityCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1️⃣ Eliminamos la columna antigua
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Communities");

            // 2️⃣ Creamos las tablas de categorías con max length 30
            migrationBuilder.CreateTable(
                name: "CommunityCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkCommunityCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InterestCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkInterestCategories", x => x.Id);
                });

            // 3️⃣ Insertamos registros "default" para evitar problemas con FK
            var defaultCommunityCategoryId = Guid.NewGuid();
            var defaultInterestCategoryId = Guid.NewGuid();

            migrationBuilder.InsertData(
                table: "CommunityCategories",
                columns: new[] { "Id", "CreateAt", "Name" },
                values: new object[] { defaultCommunityCategoryId, DateTime.UtcNow, "Default Community Cat" } // 23 chars
            );

            migrationBuilder.InsertData(
                table: "InterestCategories",
                columns: new[] { "Id", "CreateAt", "Name" },
                values: new object[] { defaultInterestCategoryId, DateTime.UtcNow, "Default Interest Cat" } // 22 chars
            );

            // 4️⃣ Creamos columnas con FK, usando IDs default
            migrationBuilder.AddColumn<Guid>(
                name: "CommunityCategoryId",
                table: "Communities",
                type: "uuid",
                nullable: false,
                defaultValue: defaultCommunityCategoryId);

            migrationBuilder.AddColumn<Guid>(
                name: "InterestCategoryId",
                table: "Interests",
                type: "uuid",
                nullable: false,
                defaultValue: defaultInterestCategoryId);

            // 5️⃣ Creamos índices
            migrationBuilder.CreateIndex(
                name: "IX_Communities_CommunityCategoryId",
                table: "Communities",
                column: "CommunityCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Interests_InterestCategoryId",
                table: "Interests",
                column: "InterestCategoryId");

            // 6️⃣ Agregamos las FK
            migrationBuilder.AddForeignKey(
                name: "FkCommunityCategoryId",
                table: "Communities",
                column: "CommunityCategoryId",
                principalTable: "CommunityCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FkInterestCategoryId",
                table: "Interests",
                column: "InterestCategoryId",
                principalTable: "InterestCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FkCommunityCategoryId",
                table: "Communities");

            migrationBuilder.DropForeignKey(
                name: "FkInterestCategoryId",
                table: "Interests");

            migrationBuilder.DropIndex(
                name: "IX_Communities_CommunityCategoryId",
                table: "Communities");

            migrationBuilder.DropIndex(
                name: "IX_Interests_InterestCategoryId",
                table: "Interests");

            migrationBuilder.DropColumn(
                name: "CommunityCategoryId",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "InterestCategoryId",
                table: "Interests");

            migrationBuilder.DropTable(
                name: "CommunityCategories");

            migrationBuilder.DropTable(
                name: "InterestCategories");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Communities",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }
    }
}
