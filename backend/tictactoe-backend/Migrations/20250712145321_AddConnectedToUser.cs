using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tictactoe_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddConnectedToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Connected",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Connected",
                table: "Users");
        }
    }
}
