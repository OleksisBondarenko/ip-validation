using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADValidation.Migrations
{
    /// <inheritdoc />
    public partial class AddedAccessPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessPolicies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IpFilterRules = table.Column<string>(type: "TEXT", nullable: false),
                    Action = table.Column<int>(type: "INTEGER", nullable: false),
                    Resource = table.Column<string>(type: "TEXT", nullable: false),
                    Validators = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditRecordAccessPolicies",
                columns: table => new
                {
                    AccessPoliciesId = table.Column<long>(type: "INTEGER", nullable: false),
                    AuditRecordsId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditRecordAccessPolicies", x => new { x.AccessPoliciesId, x.AuditRecordsId });
                    table.ForeignKey(
                        name: "FK_AuditRecordAccessPolicies_AccessPolicies_AccessPoliciesId",
                        column: x => x.AccessPoliciesId,
                        principalTable: "AccessPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuditRecordAccessPolicies_AuditRecords_AuditRecordsId",
                        column: x => x.AuditRecordsId,
                        principalTable: "AuditRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditRecordAccessPolicies_AuditRecordsId",
                table: "AuditRecordAccessPolicies",
                column: "AuditRecordsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditRecordAccessPolicies");

            migrationBuilder.DropTable(
                name: "AccessPolicies");
        }
    }
}
