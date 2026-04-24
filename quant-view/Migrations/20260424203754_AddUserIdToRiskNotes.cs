using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quant_view.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToRiskNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "RiskNotes",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RiskNotes");
        }
    }
}
