using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ContexTweet.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NamedEntities",
                columns: table => new
                {
                    Text = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NamedEntities", x => x.Text);
                });

            migrationBuilder.CreateTable(
                name: "Tweets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SentimentScore = table.Column<float>(type: "real", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tweets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NamedEntitiesTweets",
                columns: table => new
                {
                    NamedEntityText = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TweetId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NamedEntitiesTweets", x => new { x.NamedEntityText, x.TweetId });
                    table.ForeignKey(
                        name: "FK_NamedEntitiesTweets_Tweets_NamedEntityText",
                        column: x => x.NamedEntityText,
                        principalTable: "Tweets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NamedEntitiesTweets_NamedEntities_TweetId",
                        column: x => x.TweetId,
                        principalTable: "NamedEntities",
                        principalColumn: "Text",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UrlsTweets",
                columns: table => new
                {
                    Url = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TweetId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlsTweets", x => new { x.Url, x.TweetId });
                    table.ForeignKey(
                        name: "FK_UrlsTweets_Tweets_TweetId",
                        column: x => x.TweetId,
                        principalTable: "Tweets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NamedEntitiesTweets_TweetId",
                table: "NamedEntitiesTweets",
                column: "TweetId");

            migrationBuilder.CreateIndex(
                name: "IX_UrlsTweets_TweetId",
                table: "UrlsTweets",
                column: "TweetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NamedEntitiesTweets");

            migrationBuilder.DropTable(
                name: "UrlsTweets");

            migrationBuilder.DropTable(
                name: "NamedEntities");

            migrationBuilder.DropTable(
                name: "Tweets");
        }
    }
}
