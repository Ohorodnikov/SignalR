using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SignalR.Data.Migrations
{
    public partial class VoteInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VoteInfo",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ImageId = table.Column<int>(type: "int", nullable: false),
                    SessionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteInfo", x => new { x.UserId, x.ImageId });
                    table.ForeignKey(
                        name: "FK_VoteInfo_ImageModel_ImageId",
                        column: x => x.ImageId,
                        principalTable: "ImageModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoteInfo_SessionUser_UserId",
                        column: x => x.UserId,
                        principalTable: "SessionUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VoteInfo_ImageId",
                table: "VoteInfo",
                column: "ImageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VoteInfo");
        }
    }
}
