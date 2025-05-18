using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADValidation.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAccessPolicyAddedDatetimes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValidationTypes",
                table: "AccessPolicies",
                newName: "PolicyStartDatetime");

            migrationBuilder.RenameColumn(
                name: "Resource",
                table: "AccessPolicies",
                newName: "PolicyEndDatetime");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "AccessPolicies",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "AccessPolicies");

            migrationBuilder.RenameColumn(
                name: "PolicyStartDatetime",
                table: "AccessPolicies",
                newName: "ValidationTypes");

            migrationBuilder.RenameColumn(
                name: "PolicyEndDatetime",
                table: "AccessPolicies",
                newName: "Resource");
        }
    }
}
