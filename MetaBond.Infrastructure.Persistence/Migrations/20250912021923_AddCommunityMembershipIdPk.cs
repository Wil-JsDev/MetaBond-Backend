using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetaBond.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCommunityMembershipIdPk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CommunityMemberships",
                table: "CommunityMemberships");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "InterestCategories",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(25)",
                oldMaxLength: 25);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "CommunityMemberships",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CommunityCategories",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(25)",
                oldMaxLength: 25);

            migrationBuilder.AddPrimaryKey(
                name: "PkCommunityMemberships",
                table: "CommunityMemberships",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityMemberships_UserId",
                table: "CommunityMemberships",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PkCommunityMemberships",
                table: "CommunityMemberships");

            migrationBuilder.DropIndex(
                name: "IX_CommunityMemberships_UserId",
                table: "CommunityMemberships");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CommunityMemberships");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "InterestCategories",
                type: "character varying(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CommunityCategories",
                type: "character varying(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommunityMemberships",
                table: "CommunityMemberships",
                columns: new[] { "UserId", "CommunityId" });
        }
    }
}
