using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ContexTweet.Migrations
{
    public partial class FixNamedEntityTweet_manytomany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NamedEntitiesTweets_Tweets_NamedEntityText",
                table: "NamedEntitiesTweets");

            migrationBuilder.DropForeignKey(
                name: "FK_NamedEntitiesTweets_NamedEntities_TweetId",
                table: "NamedEntitiesTweets");

            migrationBuilder.AddForeignKey(
                name: "FK_NamedEntitiesTweets_NamedEntities_NamedEntityText",
                table: "NamedEntitiesTweets",
                column: "NamedEntityText",
                principalTable: "NamedEntities",
                principalColumn: "Text",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NamedEntitiesTweets_Tweets_TweetId",
                table: "NamedEntitiesTweets",
                column: "TweetId",
                principalTable: "Tweets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NamedEntitiesTweets_NamedEntities_NamedEntityText",
                table: "NamedEntitiesTweets");

            migrationBuilder.DropForeignKey(
                name: "FK_NamedEntitiesTweets_Tweets_TweetId",
                table: "NamedEntitiesTweets");

            migrationBuilder.AddForeignKey(
                name: "FK_NamedEntitiesTweets_Tweets_NamedEntityText",
                table: "NamedEntitiesTweets",
                column: "NamedEntityText",
                principalTable: "Tweets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NamedEntitiesTweets_NamedEntities_TweetId",
                table: "NamedEntitiesTweets",
                column: "TweetId",
                principalTable: "NamedEntities",
                principalColumn: "Text",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
