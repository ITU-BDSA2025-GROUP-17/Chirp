using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddedSavedCheepsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SavedCheeps",
                columns: table => new
                {
                    SaverId = table.Column<int>(type: "INTEGER", nullable: false),
                    CheepId = table.Column<long>(type: "INTEGER", nullable: false),
                    time_stamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedCheeps", x => new { x.SaverId, x.CheepId });
                    table.ForeignKey(
                        name: "FK_SavedCheeps_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SavedCheeps_AspNetUsers_SaverId",
                        column: x => x.SaverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SavedCheeps_Cheeps_CheepId",
                        column: x => x.CheepId,
                        principalTable: "Cheeps",
                        principalColumn: "message_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedCheeps_AuthorId",
                table: "SavedCheeps",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedCheeps_CheepId",
                table: "SavedCheeps",
                column: "CheepId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedCheeps");
        }
    }
}
