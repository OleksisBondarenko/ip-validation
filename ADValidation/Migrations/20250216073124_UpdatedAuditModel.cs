using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADValidation.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAuditModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "AuditRecords");

            migrationBuilder.CreateTable(
                name: "AuditData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Hostname = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Domain = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AuditRecordId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditData_AuditRecords_AuditRecordId",
                        column: x => x.AuditRecordId,
                        principalTable: "AuditRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditData_AuditRecordId",
                table: "AuditData",
                column: "AuditRecordId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditData");

            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "AuditRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
