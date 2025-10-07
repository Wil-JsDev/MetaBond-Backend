using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetaBond.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationshipsForUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Rewards",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ProgressEntry",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ProgressBoard",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Posts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rewards_UserId",
                table: "Rewards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressEntry_UserId",
                table: "ProgressEntry",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressBoard_UserId",
                table: "ProgressBoard",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CreatedById",
                table: "Posts",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FkPostsCreatedById",
                table: "Posts",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FkProgressBoardsUserId",
                table: "ProgressBoard",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FkProgressEntriesUserId",
                table: "ProgressEntry",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FkRewardsUserId",
                table: "Rewards",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FkPostsCreatedById",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FkProgressBoardsUserId",
                table: "ProgressBoard");

            migrationBuilder.DropForeignKey(
                name: "FkProgressEntriesUserId",
                table: "ProgressEntry");

            migrationBuilder.DropForeignKey(
                name: "FkRewardsUserId",
                table: "Rewards");

            migrationBuilder.DropIndex(
                name: "IX_Rewards_UserId",
                table: "Rewards");

            migrationBuilder.DropIndex(
                name: "IX_ProgressEntry_UserId",
                table: "ProgressEntry");

            migrationBuilder.DropIndex(
                name: "IX_ProgressBoard_UserId",
                table: "ProgressBoard");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CreatedById",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProgressEntry");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProgressBoard");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Posts");
        }
    }
}
