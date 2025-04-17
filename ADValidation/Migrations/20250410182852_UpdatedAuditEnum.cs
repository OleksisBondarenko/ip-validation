using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADValidation.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAuditEnum : Migration
    {
        
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = 0;", suppressTransaction: true);
            
            migrationBuilder.DropColumn(
                name: "Name",
                table: "AuditRecord");

            migrationBuilder.AddColumn<int>(
                name: "AuditType",
                table: "AuditRecord",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuditType",
                table: "AuditRecord");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AuditRecord",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
