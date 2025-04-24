using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class AddPerson_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE AddPerson 
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
                INSERT INTO [dbo].[Persons] (PersonID, PersonName, Email, DateOfBirth, Gender, Address, CountryID,RecievesNewsLetters)
                VALUES (@PersonID, @PersonName, @Email, @DateOfBirth, @Gender,@Address, @CountryID, @RecievesNewsLetters)
                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE AddPerson");
        }
    }
}
