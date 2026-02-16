using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CorporateHelpdesk.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPriorityToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Ticket",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Ticket");
        }
    }
}
