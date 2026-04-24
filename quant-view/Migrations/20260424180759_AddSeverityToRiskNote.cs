using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quant_view.Migrations
{
    /// <inheritdoc />
    public partial class AddSeverityToRiskNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Severity",
                table: "RiskNotes",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Severity",
                table: "RiskNotes");
        }
    }
}
