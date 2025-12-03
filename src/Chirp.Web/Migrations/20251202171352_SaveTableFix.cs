using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Web.Migrations
{
    /// <inheritdoc />
    public partial class SaveTableFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedCheeps_AspNetUsers_AuthorId",
                table: "SavedCheeps");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedCheeps_AspNetUsers_SaverId",
                table: "SavedCheeps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SavedCheeps",
                table: "SavedCheeps");

            migrationBuilder.DropIndex(
                name: "IX_SavedCheeps_AuthorId",
                table: "SavedCheeps");

            migrationBuilder.DropColumn(
                name: "SaverId",
                table: "SavedCheeps");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "SavedCheeps",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SavedCheeps",
                table: "SavedCheeps",
                columns: new[] { "AuthorId", "CheepId" });

            migrationBuilder.AddForeignKey(
                name: "FK_SavedCheeps_AspNetUsers_AuthorId",
                table: "SavedCheeps",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedCheeps_AspNetUsers_AuthorId",
                table: "SavedCheeps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SavedCheeps",
                table: "SavedCheeps");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "SavedCheeps",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "SaverId",
                table: "SavedCheeps",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SavedCheeps",
                table: "SavedCheeps",
                columns: new[] { "SaverId", "CheepId" });

            migrationBuilder.CreateIndex(
                name: "IX_SavedCheeps_AuthorId",
                table: "SavedCheeps",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedCheeps_AspNetUsers_AuthorId",
                table: "SavedCheeps",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedCheeps_AspNetUsers_SaverId",
                table: "SavedCheeps",
                column: "SaverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
