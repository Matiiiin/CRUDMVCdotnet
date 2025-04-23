using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryID);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    PersonID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CountryID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RecievesNewsLetters = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.PersonID);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryID", "CountryName" },
                values: new object[,]
                {
                    { new Guid("34ccdd2d-da1d-4b71-9d4d-3963a33fadaf"), "Italy" },
                    { new Guid("3c061c30-c967-4a1a-a2ed-8da4ac4ab918"), "Canada" },
                    { new Guid("4d6681c6-d6d4-4520-8b4b-9ad183ee271c"), "Germany" },
                    { new Guid("54b1e29d-acc5-4a74-914e-51143301af44"), "France" },
                    { new Guid("6b93e03b-24a5-4975-81b0-39cc5832a80c"), "Spain" },
                    { new Guid("aebe6d4e-aa50-4cba-8879-91cecf7b6110"), "Colombia" },
                    { new Guid("ff642272-7ae8-4a19-98fc-c51b6954ec58"), "USA" }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "PersonID", "Address", "CountryID", "DateOfBirth", "Email", "Gender", "PersonName", "RecievesNewsLetters" },
                values: new object[,]
                {
                    { new Guid("0c97d5dd-5984-436a-a1f2-2fe1f3857a59"), "123 Maple Street, New York, NY", new Guid("4d6681c6-d6d4-4520-8b4b-9ad183ee271c"), new DateTime(1985, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "michael.johnson@example.com", "Male", "Michael Johnson", true },
                    { new Guid("2c503e0b-5ae8-4248-a020-30bed949e283"), "987 Spruce Drive, Philadelphia, PA", new Guid("ff642272-7ae8-4a19-98fc-c51b6954ec58"), new DateTime(1988, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "olivia.martinez@example.com", "Female", "Olivia Martinez", true },
                    { new Guid("32cc403b-38a6-41ce-87c4-415aacab9b9d"), "654 Birch Street, Phoenix, AZ", new Guid("34ccdd2d-da1d-4b71-9d4d-3963a33fadaf"), new DateTime(1995, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "william.garcia@example.com", "Male", "William Garcia", true },
                    { new Guid("574ae25f-2d09-4d57-8c76-56913731e0a1"), "951 Redwood Boulevard, San Jose, CA", new Guid("aebe6d4e-aa50-4cba-8879-91cecf7b6110"), new DateTime(1993, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "mia.taylor@example.com", "Female", "Mia Taylor", true },
                    { new Guid("5eda0c41-f885-4ec2-8a1c-68bf060cb9a2"), "159 Elm Court, San Antonio, TX", new Guid("34ccdd2d-da1d-4b71-9d4d-3963a33fadaf"), new DateTime(1990, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "benjamin.wilson@example.com", "Male", "Benjamin Wilson", false },
                    { new Guid("878e4edf-f877-4db5-86fa-ef37dfbe1a2f"), "753 Willow Way, San Diego, CA", new Guid("6b93e03b-24a5-4975-81b0-39cc5832a80c"), new DateTime(1998, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "isabella.anderson@example.com", "Female", "Isabella Anderson", true },
                    { new Guid("be245ea5-9e28-4cb4-97c0-290bc619b082"), "456 Oak Avenue, Los Angeles, CA", new Guid("54b1e29d-acc5-4a74-914e-51143301af44"), new DateTime(1992, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "emily.davis@example.com", "Female", "Emily Davis", false },
                    { new Guid("c7972b4b-c1cb-465e-948b-8c50969d56e8"), "321 Cedar Lane, Houston, TX", new Guid("ff642272-7ae8-4a19-98fc-c51b6954ec58"), new DateTime(2000, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "sophia.brown@example.com", "Female", "Sophia Brown", false },
                    { new Guid("d2f86a9c-8681-4f76-89ab-aa18ea43bbc3"), "852 Aspen Circle, Dallas, TX", new Guid("54b1e29d-acc5-4a74-914e-51143301af44"), new DateTime(1983, 5, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "alexander.thomas@example.com", "Male", "Alexander Thomas", false },
                    { new Guid("e4ae92cb-76ef-4180-af85-e3117a7bf45a"), "789 Pine Road, Chicago, IL", new Guid("3c061c30-c967-4a1a-a2ed-8da4ac4ab918"), new DateTime(1978, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "james.smith@example.com", "Male", "James Smith", true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Persons");
        }
    }
}
