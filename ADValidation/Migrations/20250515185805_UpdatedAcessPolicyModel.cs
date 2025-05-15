using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADValidation.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAcessPolicyModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Validators",
                table: "AccessPolicies",
                newName: "ValidationTypes");

            migrationBuilder.AddColumn<long>(
                name: "Order",
                table: "AccessPolicies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "AccessPolicies");

            migrationBuilder.RenameColumn(
                name: "ValidationTypes",
                table: "AccessPolicies",
                newName: "Validators");
        }
    }
}
