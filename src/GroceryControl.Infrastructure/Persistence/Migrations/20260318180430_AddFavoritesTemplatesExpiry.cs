using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroceryControl.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoritesTemplatesExpiry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDateUtc",
                table: "InventoryEntries",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ShoppingTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingTemplates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFavoriteProducts",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoriteProducts", x => new { x.UserId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_UserFavoriteProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavoriteProducts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingTemplateItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingTemplateItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingTemplateItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShoppingTemplateItems_ShoppingTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "ShoppingTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShoppingTemplateItems_UnitTypes_UnitTypeId",
                        column: x => x.UnitTypeId,
                        principalTable: "UnitTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingTemplateItems_ProductId",
                table: "ShoppingTemplateItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingTemplateItems_TemplateId",
                table: "ShoppingTemplateItems",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingTemplateItems_UnitTypeId",
                table: "ShoppingTemplateItems",
                column: "UnitTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingTemplates_UserId",
                table: "ShoppingTemplates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteProducts_ProductId",
                table: "UserFavoriteProducts",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShoppingTemplateItems");

            migrationBuilder.DropTable(
                name: "UserFavoriteProducts");

            migrationBuilder.DropTable(
                name: "ShoppingTemplates");

            migrationBuilder.DropColumn(
                name: "ExpirationDateUtc",
                table: "InventoryEntries");
        }
    }
}
