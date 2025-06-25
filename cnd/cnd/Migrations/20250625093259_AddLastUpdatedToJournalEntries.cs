using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cnd.Migrations
{
    /// <inheritdoc />
    public partial class AddLastUpdatedToJournalEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "JournalEntries",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "JournalEntries");
        }
    }
}
