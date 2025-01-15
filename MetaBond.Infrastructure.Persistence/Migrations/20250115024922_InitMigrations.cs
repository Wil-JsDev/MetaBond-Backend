using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetaBond.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:status", "pending,accepted,blocked");

            migrationBuilder.CreateTable(
                name: "Communities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Category = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkCommunities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Friendship",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkFriendship", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParticiationInEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticiationInEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rewards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    PointAwarded = table.Column<int>(type: "integer", nullable: false),
                    DateAwarded = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkRewards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DateAndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CommunitiesId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipationInEventId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FkCommunitiesId",
                        column: x => x.CommunitiesId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Image = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    CommunitiesId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FkCommunities",
                        column: x => x.CommunitiesId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventsParticipationInEvent",
                columns: table => new
                {
                    EventsId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipationInEventId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventsParticipationInEvent", x => new { x.EventsId, x.ParticipationInEventId });
                    table.ForeignKey(
                        name: "FK_EventsParticipationInEvent_Events_EventsId",
                        column: x => x.EventsId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventsParticipationInEvent_ParticiationInEvent_Participatio~",
                        column: x => x.ParticipationInEventId,
                        principalTable: "ParticiationInEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_CommunitiesId",
                table: "Events",
                column: "CommunitiesId");

            migrationBuilder.CreateIndex(
                name: "IX_EventsParticipationInEvent_ParticipationInEventId",
                table: "EventsParticipationInEvent",
                column: "ParticipationInEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CommunitiesId",
                table: "Posts",
                column: "CommunitiesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventsParticipationInEvent");

            migrationBuilder.DropTable(
                name: "Friendship");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Rewards");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "ParticiationInEvent");

            migrationBuilder.DropTable(
                name: "Communities");
        }
    }
}
