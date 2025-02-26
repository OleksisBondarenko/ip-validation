using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADValidation.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAuditDataModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditData_AuditRecords_AuditRecordId",
                table: "AuditData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditRecords",
                table: "AuditRecords");

            migrationBuilder.RenameTable(
                name: "AuditRecords",
                newName: "AuditRecord");

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "AuditData",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditRecord",
                table: "AuditRecord",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditData_AuditRecord_AuditRecordId",
                table: "AuditData",
                column: "AuditRecordId",
                principalTable: "AuditRecord",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditData_AuditRecord_AuditRecordId",
                table: "AuditData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditRecord",
                table: "AuditRecord");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "AuditData");

            migrationBuilder.RenameTable(
                name: "AuditRecord",
                newName: "AuditRecords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditRecords",
                table: "AuditRecords",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditData_AuditRecords_AuditRecordId",
                table: "AuditData",
                column: "AuditRecordId",
                principalTable: "AuditRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
