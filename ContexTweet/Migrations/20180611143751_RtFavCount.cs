using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ContexTweet.Migrations
{
    public partial class RtFavCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FavoriteCount",
                table: "Tweets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RetweetCount",
                table: "Tweets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FavoriteCount",
                table: "Tweets");

            migrationBuilder.DropColumn(
                name: "RetweetCount",
                table: "Tweets");
        }
    }
}
