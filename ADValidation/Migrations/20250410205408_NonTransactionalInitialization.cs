using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADValidation.Migrations
{
    /// <inheritdoc />
    public partial class NonTransactionalInitialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = 0;", suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
