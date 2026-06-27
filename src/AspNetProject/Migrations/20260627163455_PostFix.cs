using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetProject.Migrations
{
    /// <inheritdoc />
    public partial class PostFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isDraft",
                table: "Posts",
                newName: "IsDraft");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDraft",
                table: "Posts",
                newName: "isDraft");
        }
    }
}
