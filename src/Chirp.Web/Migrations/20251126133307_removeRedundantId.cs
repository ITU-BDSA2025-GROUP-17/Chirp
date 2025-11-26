using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Web.Migrations
{
    /// <inheritdoc />
    public partial class removeRedundantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Follows",
                table: "Follows");

            migrationBuilder.DropIndex(
                name: "IX_Follows_FollowingId",
                table: "Follows");

            migrationBuilder.AlterColumn<int>(
                name: "FollowerId",
                table: "Follows",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Follows",
                table: "Follows",
                column: "FollowingId");

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FollowerId",
                table: "Follows",
                column: "FollowerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Follows",
                table: "Follows");

            migrationBuilder.DropIndex(
                name: "IX_Follows_FollowerId",
                table: "Follows");

            migrationBuilder.AlterColumn<int>(
                name: "FollowerId",
                table: "Follows",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Follows",
                table: "Follows",
                columns: new[] { "FollowerId", "FollowingId" });

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FollowingId",
                table: "Follows",
                column: "FollowingId");
        }
    }
}
