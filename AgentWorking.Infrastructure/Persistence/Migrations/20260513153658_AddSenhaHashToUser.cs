using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgentWorking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSenhaHashToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SenhaHash",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenhaHash",
                table: "Users");
        }
    }
}
