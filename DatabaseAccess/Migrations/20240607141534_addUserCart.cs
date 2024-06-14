using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseAccess.Migrations
{
    /// <inheritdoc />
    public partial class addUserCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "userCarts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    userId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userCarts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_userCarts_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_userCarts_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_userCarts_ApplicationUserId",
                table: "userCarts",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_userCarts_ProductId",
                table: "userCarts",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "userCarts");
        }
    }
}
