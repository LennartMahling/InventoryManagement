using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_InventoryManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentToInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Inventory",
                type: "TEXT",
                maxLength: 1000,
                nullable: true,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Inventory");
        }
    }
}
