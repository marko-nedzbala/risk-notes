using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quant_view.Migrations
{
    /// <inheritdoc />
    public partial class AddRiskNoteAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RiskNoteAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RiskNoteId = table.Column<int>(type: "INTEGER", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    StoredFileName = table.Column<string>(type: "TEXT", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskNoteAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RiskNoteAttachments_RiskNotes_RiskNoteId",
                        column: x => x.RiskNoteId,
                        principalTable: "RiskNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RiskNoteAttachments_RiskNoteId",
                table: "RiskNoteAttachments",
                column: "RiskNoteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RiskNoteAttachments");
        }
    }
}
