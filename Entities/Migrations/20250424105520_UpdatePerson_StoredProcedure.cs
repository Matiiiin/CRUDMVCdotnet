using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePerson_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE UpdatePerson 
                (@PersonID uniqueidentifier,
                @PersonName varchar(100),
                @Email varchar(100),
                @DateOfBirth datetime,
                @Gender varchar(20),
                @Address varchar(1000),
                @CountryID uniqueidentifier,
                @RecievesNewsLetters bit)
                AS
                BEGIN
                UPDATE [dbo].[persons]
                SET PersonName = @PersonName,
                Email = @Email,
                DateOfBirth = @DateOfBirth,
                Gender = @Gender,
                Address = @Address,
                CountryID = @CountryID,
                RecievesNewsLetters = @RecievesNewsLetters
                WHERE PersonID = @PersonID
                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE UpdatePerson");
        }
    }
}
