using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADValidation.Migrations
{
    /// <inheritdoc />
    public partial class AddedResourceNameToAuditRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResourceName",
                table: "AuditRecord",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResourceName",
                table: "AuditRecord");
        }
    }
}
