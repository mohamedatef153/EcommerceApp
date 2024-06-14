using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseAccess.Migrations
{
    /// <inheritdoc />
    public partial class addUserCart1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userCarts_AspNetUsers_ApplicationUserId",
                table: "userCarts");

            migrationBuilder.DropIndex(
                name: "IX_userCarts_ApplicationUserId",
                table: "userCarts");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "userCarts");

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "userCarts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_userCarts_userId",
                table: "userCarts",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_userCarts_AspNetUsers_userId",
                table: "userCarts",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userCarts_AspNetUsers_userId",
                table: "userCarts");

            migrationBuilder.DropIndex(
                name: "IX_userCarts_userId",
                table: "userCarts");

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "userCarts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "userCarts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_userCarts_ApplicationUserId",
                table: "userCarts",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_userCarts_AspNetUsers_ApplicationUserId",
                table: "userCarts",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
