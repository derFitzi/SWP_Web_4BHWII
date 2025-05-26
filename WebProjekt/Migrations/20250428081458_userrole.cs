using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebProjekt.Migrations
{
    /// <inheritdoc />
    public partial class userrole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Userrole",
                table: "Users",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Userrole",
                table: "Users");
        }
    }
}
