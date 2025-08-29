using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PublicUtilities.Migrations
{
    /// <inheritdoc />
    public partial class AddUniquePersonalAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_PersonalAccount",
                table: "Users",
                column: "PersonalAccount",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_PersonalAccount",
                table: "Users");
        }
    }
}
