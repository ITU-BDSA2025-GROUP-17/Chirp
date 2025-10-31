using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Web.Migrations
{
    /// <inheritdoc />
    public partial class MessagesRenameToCheeps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_AuthorId1",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Messages",
                table: "Messages");

            migrationBuilder.RenameTable(
                name: "Messages",
                newName: "Cheeps");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_AuthorId1",
                table: "Cheeps",
                newName: "IX_Cheeps_AuthorId1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cheeps",
                table: "Cheeps",
                column: "message_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cheeps_AspNetUsers_AuthorId1",
                table: "Cheeps",
                column: "AuthorId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cheeps_AspNetUsers_AuthorId1",
                table: "Cheeps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cheeps",
                table: "Cheeps");

            migrationBuilder.RenameTable(
                name: "Cheeps",
                newName: "Messages");

            migrationBuilder.RenameIndex(
                name: "IX_Cheeps_AuthorId1",
                table: "Messages",
                newName: "IX_Messages_AuthorId1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Messages",
                table: "Messages",
                column: "message_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_AuthorId1",
                table: "Messages",
                column: "AuthorId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
