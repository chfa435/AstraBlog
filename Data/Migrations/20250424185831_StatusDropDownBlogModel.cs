using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AstraBlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class StatusDropDownBlogModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "BlogPosts");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "BlogPosts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "BlogPosts");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "BlogPosts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
