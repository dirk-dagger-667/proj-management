using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace proj_mngmt_api.Migrations
{
    /// <inheritdoc />
    public partial class RemovedRequiredFromProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuditType",
                table: "Audits",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuditType",
                table: "Audits");
        }
    }
}
