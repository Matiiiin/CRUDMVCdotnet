using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class GetAllPersons_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlStatement = "CREATE PROCEDURE [dbo].[GetAllPersons] AS BEGIN SELECT * FROM [dbo].[Persons] END";
            migrationBuilder.Sql(sqlStatement);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE [dbo].[GetAllPersons]");
        }
    }
}
