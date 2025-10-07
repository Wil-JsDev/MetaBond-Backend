using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetaBond.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEventParticipationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_eventParticipations",
                table: "eventParticipations");

            migrationBuilder.RenameTable(
                name: "ParticiationInEvent",
                newName: "ParticipationInEvent");

            migrationBuilder.RenameTable(
                name: "eventParticipations",
                newName: "EventParticipation");

            migrationBuilder.RenameIndex(
                name: "IX_eventParticipations_ParticipationInEventId",
                table: "EventParticipation",
                newName: "IX_EventParticipation_ParticipationInEventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventParticipation",
                table: "EventParticipation",
                columns: new[] { "EventId", "ParticipationInEventId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EventParticipation",
                table: "EventParticipation");

            migrationBuilder.RenameTable(
                name: "ParticipationInEvent",
                newName: "ParticiationInEvent");

            migrationBuilder.RenameTable(
                name: "EventParticipation",
                newName: "eventParticipations");

            migrationBuilder.RenameIndex(
                name: "IX_EventParticipation_ParticipationInEventId",
                table: "eventParticipations",
                newName: "IX_eventParticipations_ParticipationInEventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_eventParticipations",
                table: "eventParticipations",
                columns: new[] { "EventId", "ParticipationInEventId" });
        }
    }
}
