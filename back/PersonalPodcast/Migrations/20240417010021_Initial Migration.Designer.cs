﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PersonalPodcast.Data;

#nullable disable

namespace PersonalPodcast.Migrations
{
    [DbContext(typeof(DBContext))]
    [Migration("20240417010021_Initial Migration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("PersonalPodcast.Models.Admin", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("LastLogin")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Admins");
                });

            modelBuilder.Entity("PersonalPodcast.Models.AudioAnalytics", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("EpisodeId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("FirstPlay")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("LastPlay")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("Length")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("EpisodeId")
                        .IsUnique();

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("audioAnalytics");
                });

            modelBuilder.Entity("PersonalPodcast.Models.Category", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("categories");
                });

            modelBuilder.Entity("PersonalPodcast.Models.Comment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("EpisodeId")
                        .HasColumnType("bigint");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("EpisodeId")
                        .IsUnique();

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("comments");
                });

            modelBuilder.Entity("PersonalPodcast.Models.Episode", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("AudioFileUrl")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("Length")
                        .HasColumnType("bigint");

                    b.Property<long>("PodcastId")
                        .HasColumnType("bigint");

                    b.Property<string>("PosterImg")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<long>("PublisherId")
                        .HasColumnType("bigint");

                    b.Property<string>("Tags")
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<long?>("VideoFileUrl")
                        .HasColumnType("bigint");

                    b.Property<long>("Views")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("PodcastId")
                        .IsUnique();

                    b.HasIndex("PublisherId")
                        .IsUnique();

                    b.ToTable("Episodes");
                });

            modelBuilder.Entity("PersonalPodcast.Models.Podcast", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("AudioFileUrl")
                        .HasColumnType("longtext");

                    b.Property<long>("CategoryId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("LastUpdate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("PosterImg")
                        .HasColumnType("longtext");

                    b.Property<long>("PublisherId")
                        .HasColumnType("bigint");

                    b.Property<string>("Tags")
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("VideoFileUrl")
                        .HasColumnType("longtext");

                    b.Property<long>("Views")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId")
                        .IsUnique();

                    b.HasIndex("PublisherId")
                        .IsUnique();

                    b.ToTable("Podcasts");
                });

            modelBuilder.Entity("PersonalPodcast.Models.Rating", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("EpisodeId")
                        .HasColumnType("bigint");

                    b.Property<int>("RatingValue")
                        .HasColumnType("int");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("EpisodeId")
                        .IsUnique();

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("ratings");
                });

            modelBuilder.Entity("PersonalPodcast.Models.Stats", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("Episodes")
                        .HasColumnType("bigint");

                    b.Property<long>("PageViews")
                        .HasColumnType("bigint");

                    b.Property<long>("Podcasts")
                        .HasColumnType("bigint");

                    b.Property<long>("Users")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("stats");
                });

            modelBuilder.Entity("PersonalPodcast.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("Birthdate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ConnectingIp")
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("FirstLogin")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("FullName")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("LastLogin")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Password")
                        .HasColumnType("longtext");

                    b.Property<string>("Role")
                        .HasColumnType("longtext");

                    b.Property<string>("Username")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PersonalPodcast.Models.Admin", b =>
                {
                    b.HasOne("PersonalPodcast.Models.User", "User")
                        .WithOne("Admin")
                        .HasForeignKey("PersonalPodcast.Models.Admin", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PersonalPodcast.Models.AudioAnalytics", b =>
                {
                    b.HasOne("PersonalPodcast.Models.Episode", "Episode")
                        .WithOne("AudioAnalytics")
                        .HasForeignKey("PersonalPodcast.Models.AudioAnalytics", "EpisodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PersonalPodcast.Models.User", "User")
                        .WithOne("AudioAnalytics")
                        .HasForeignKey("PersonalPodcast.Models.AudioAnalytics", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Episode");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PersonalPodcast.Models.Comment", b =>
                {
                    b.HasOne("PersonalPodcast.Models.Episode", "Episode")
                        .WithOne("Comment")
                        .HasForeignKey("PersonalPodcast.Models.Comment", "EpisodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PersonalPodcast.Models.User", "User")
                        .WithOne("Comment")
                        .HasForeignKey("PersonalPodcast.Models.Comment", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Episode");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PersonalPodcast.Models.Episode", b =>
                {
                    b.HasOne("PersonalPodcast.Models.Podcast", null)
                        .WithOne("Episode")
                        .HasForeignKey("PersonalPodcast.Models.Episode", "PodcastId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PersonalPodcast.Models.User", "User")
                        .WithOne("Episode")
                        .HasForeignKey("PersonalPodcast.Models.Episode", "PublisherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PersonalPodcast.Models.Podcast", b =>
                {
                    b.HasOne("PersonalPodcast.Models.Category", "Category")
                        .WithOne("Podcast")
                        .HasForeignKey("PersonalPodcast.Models.Podcast", "CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PersonalPodcast.Models.User", "User")
                        .WithOne("Podcast")
                        .HasForeignKey("PersonalPodcast.Models.Podcast", "PublisherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PersonalPodcast.Models.Rating", b =>
                {
                    b.HasOne("PersonalPodcast.Models.Episode", "Episode")
                        .WithOne("Rating")
                        .HasForeignKey("PersonalPodcast.Models.Rating", "EpisodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PersonalPodcast.Models.User", "User")
                        .WithOne("Rating")
                        .HasForeignKey("PersonalPodcast.Models.Rating", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Episode");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PersonalPodcast.Models.Category", b =>
                {
                    b.Navigation("Podcast")
                        .IsRequired();
                });

            modelBuilder.Entity("PersonalPodcast.Models.Episode", b =>
                {
                    b.Navigation("AudioAnalytics")
                        .IsRequired();

                    b.Navigation("Comment")
                        .IsRequired();

                    b.Navigation("Rating")
                        .IsRequired();
                });

            modelBuilder.Entity("PersonalPodcast.Models.Podcast", b =>
                {
                    b.Navigation("Episode")
                        .IsRequired();
                });

            modelBuilder.Entity("PersonalPodcast.Models.User", b =>
                {
                    b.Navigation("Admin")
                        .IsRequired();

                    b.Navigation("AudioAnalytics")
                        .IsRequired();

                    b.Navigation("Comment")
                        .IsRequired();

                    b.Navigation("Episode")
                        .IsRequired();

                    b.Navigation("Podcast")
                        .IsRequired();

                    b.Navigation("Rating")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
