﻿// <auto-generated />
using ContexTweet.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace ContexTweet.Migrations
{
    [DbContext(typeof(ContexTweetDbContext))]
    [Migration("20180629113706_UserIdandText")]
    partial class UserIdandText
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ContexTweet.Models.NamedEntity", b =>
                {
                    b.Property<string>("Text")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Type");

                    b.HasKey("Text");

                    b.ToTable("NamedEntities");
                });

            modelBuilder.Entity("ContexTweet.Models.NamedEntityTweet", b =>
                {
                    b.Property<string>("NamedEntityText");

                    b.Property<string>("TweetId");

                    b.HasKey("NamedEntityText", "TweetId");

                    b.HasIndex("TweetId");

                    b.ToTable("NamedEntitiesTweets");
                });

            modelBuilder.Entity("ContexTweet.Models.Tweet", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("FavoriteCount");

                    b.Property<int>("RetweetCount");

                    b.Property<float>("SentimentScore");

                    b.Property<string>("Text");

                    b.Property<DateTime>("Timestamp");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Tweets");
                });

            modelBuilder.Entity("ContexTweet.Models.UrlTweet", b =>
                {
                    b.Property<string>("Url");

                    b.Property<string>("TweetId");

                    b.HasKey("Url", "TweetId");

                    b.HasIndex("TweetId");

                    b.ToTable("UrlsTweets");
                });

            modelBuilder.Entity("ContexTweet.Models.NamedEntityTweet", b =>
                {
                    b.HasOne("ContexTweet.Models.NamedEntity", "NamedEntity")
                        .WithMany("Tweets")
                        .HasForeignKey("NamedEntityText")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContexTweet.Models.Tweet", "Tweet")
                        .WithMany("NamedEntities")
                        .HasForeignKey("TweetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContexTweet.Models.UrlTweet", b =>
                {
                    b.HasOne("ContexTweet.Models.Tweet", "Tweet")
                        .WithMany("Urls")
                        .HasForeignKey("TweetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
