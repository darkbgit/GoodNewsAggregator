using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace GoodNewsAggregator.DAL.Core.Migrations
{
    public partial class InitRss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
               table: "RssSources",
               columns: new[] { "Id", "Name", "Url" },
               values: new object[]
               {
                    Guid.NewGuid(),
                    "Onliner",
                    "https://www.onliner.by/feed"
               });
            migrationBuilder.InsertData(
               table: "RssSources",
               columns: new[] { "Id", "Name", "Url" },
               values: new object[]
               {
                    Guid.NewGuid(),
                    "Tut.by",
                    "https://news.tut.by/rss/all.rss"
               });
            migrationBuilder.InsertData(
               table: "RssSources",
               columns: new[] { "Id", "Name", "Url" },
               values: new object[]
               {
                    Guid.NewGuid(),
                    "S13",
                    "http://s13.ru/rss"
               });
            migrationBuilder.InsertData(
               table: "RssSources",
               columns: new[] { "Id", "Name", "Url" },
               values: new object[]
               {
                    Guid.NewGuid(),
                    "TJournal",
                    "https://tjournal.ru/rss"
               });
            migrationBuilder.InsertData(
               table: "RssSources",
               columns: new[] { "Id", "Name", "Url" },
               values: new object[]
               {
                    Guid.NewGuid(),
                    "DTF",
                    "https://dtf.ru/rss"
               });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
