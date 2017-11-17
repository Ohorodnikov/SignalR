using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SignalR.Data.Migrations
{
    public partial class VoteInfoSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VoteInfo",
                table: "VoteInfo");

            migrationBuilder.DropColumn(
                name: "SessionCode",
                table: "VoteInfo");

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "VoteInfo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VoteInfo",
                table: "VoteInfo",
                columns: new[] { "UserId", "ImageId", "SessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_VoteInfo_SessionId",
                table: "VoteInfo",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_VoteInfo_Session_SessionId",
                table: "VoteInfo",
                column: "SessionId",
                principalTable: "Session",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoteInfo_Session_SessionId",
                table: "VoteInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VoteInfo",
                table: "VoteInfo");

            migrationBuilder.DropIndex(
                name: "IX_VoteInfo_SessionId",
                table: "VoteInfo");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "VoteInfo");

            migrationBuilder.AddColumn<string>(
                name: "SessionCode",
                table: "VoteInfo",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VoteInfo",
                table: "VoteInfo",
                columns: new[] { "UserId", "ImageId" });
        }
    }
}
