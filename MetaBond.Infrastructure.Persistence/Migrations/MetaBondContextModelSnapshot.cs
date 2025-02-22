﻿// <auto-generated />
using System;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MetaBond.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(MetaBondContext))]
    partial class MetaBondContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "status", new[] { "pending", "accepted", "blocked" });
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MetaBond.Domain.Models.Communities", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("character varying(25)");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id")
                        .HasName("PkCommunities");

                    b.ToTable("Communities", (string)null);
                });

            modelBuilder.Entity("MetaBond.Domain.Models.EventParticipation", b =>
                {
                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ParticipationInEventId")
                        .HasColumnType("uuid");

                    b.HasKey("EventId", "ParticipationInEventId");

                    b.HasIndex("ParticipationInEventId");

                    b.ToTable("eventParticipations");
                });

            modelBuilder.Entity("MetaBond.Domain.Models.Events", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CommunitiesId")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreateAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DateAndTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id")
                        .HasName("PkEvents");

                    b.HasIndex("CommunitiesId");

                    b.ToTable("Events", (string)null);
                });

            modelBuilder.Entity("MetaBond.Domain.Models.Friendship", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreateAdt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Id")
                        .HasName("PkFriendship");

                    b.ToTable("Friendship", (string)null);
                });

            modelBuilder.Entity("MetaBond.Domain.Models.ParticipationInEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("EventId")
                        .HasColumnType("uuid");

                    b.HasKey("Id")
                        .HasName("PkParticipationInEvent");

                    b.ToTable("ParticiationInEvent", (string)null);
                });

            modelBuilder.Entity("MetaBond.Domain.Models.Posts", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CommunitiesId")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id")
                        .HasName("PkPosts");

                    b.HasIndex("CommunitiesId");

                    b.ToTable("Posts", (string)null);
                });

            modelBuilder.Entity("MetaBond.Domain.Models.ProgressBoard", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CommunitiesId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id")
                        .HasName("PkProgressBoard");

                    b.HasIndex("CommunitiesId")
                        .IsUnique();

                    b.ToTable("ProgressBoard", (string)null);
                });

            modelBuilder.Entity("MetaBond.Domain.Models.ProgressEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<Guid>("ProgressBoardId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("UpdateAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id")
                        .HasName("PkProgressEntry");

                    b.HasIndex("ProgressBoardId");

                    b.ToTable("ProgressEntry", (string)null);
                });

            modelBuilder.Entity("MetaBond.Domain.Models.Rewards", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("DateAwarded")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<int?>("PointAwarded")
                        .IsRequired()
                        .HasColumnType("integer");

                    b.HasKey("Id")
                        .HasName("PkRewards");

                    b.ToTable("Rewards", (string)null);
                });

            modelBuilder.Entity("MetaBond.Domain.Models.EventParticipation", b =>
                {
                    b.HasOne("MetaBond.Domain.Models.Events", "Event")
                        .WithMany("EventParticipations")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FkEvent");

                    b.HasOne("MetaBond.Domain.Models.ParticipationInEvent", "ParticipationInEvent")
                        .WithMany("EventParticipations")
                        .HasForeignKey("ParticipationInEventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FkParticipationInEvent");

                    b.Navigation("Event");

                    b.Navigation("ParticipationInEvent");
                });

            modelBuilder.Entity("MetaBond.Domain.Models.Events", b =>
                {
                    b.HasOne("MetaBond.Domain.Models.Communities", "Communities")
                        .WithMany("Events")
                        .HasForeignKey("CommunitiesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FkCommunitiesId");

                    b.Navigation("Communities");
                });

            modelBuilder.Entity("MetaBond.Domain.Models.Posts", b =>
                {
                    b.HasOne("MetaBond.Domain.Models.Communities", "Communities")
                        .WithMany("Posts")
                        .HasForeignKey("CommunitiesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FkCommunities");

                    b.Navigation("Communities");
                });

            modelBuilder.Entity("MetaBond.Domain.Models.ProgressBoard", b =>
                {
                    b.HasOne("MetaBond.Domain.Models.Communities", "Communities")
                        .WithOne("ProgressBoard")
                        .HasForeignKey("MetaBond.Domain.Models.ProgressBoard", "CommunitiesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FkCommunitiesId");

                    b.Navigation("Communities");
                });

            modelBuilder.Entity("MetaBond.Domain.Models.ProgressEntry", b =>
                {
                    b.HasOne("MetaBond.Domain.Models.ProgressBoard", "ProgressBoard")
                        .WithMany("ProgressEntries")
                        .HasForeignKey("ProgressBoardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FkProgressBoardId");

                    b.Navigation("ProgressBoard");
                });

            modelBuilder.Entity("MetaBond.Domain.Models.Communities", b =>
                {
                    b.Navigation("Events");

                    b.Navigation("Posts");

                    b.Navigation("ProgressBoard");
                });

            modelBuilder.Entity("MetaBond.Domain.Models.Events", b =>
                {
                    b.Navigation("EventParticipations");
                });

            modelBuilder.Entity("MetaBond.Domain.Models.ParticipationInEvent", b =>
                {
                    b.Navigation("EventParticipations");
                });

            modelBuilder.Entity("MetaBond.Domain.Models.ProgressBoard", b =>
                {
                    b.Navigation("ProgressEntries");
                });
#pragma warning restore 612, 618
        }
    }
}
