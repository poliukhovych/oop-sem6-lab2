using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConstantTalk.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToBill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Bills",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Bills");
        }
    }
}
