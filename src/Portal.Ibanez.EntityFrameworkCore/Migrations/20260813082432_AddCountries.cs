using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portal.Ibanez.Migrations
{
    /// <inheritdoc />
    public partial class AddCountries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CountryId",
                table: "AppCustomers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppCountries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsoCode = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCountries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppCustomers_CountryId",
                table: "AppCustomers",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCountries_Name",
                table: "AppCountries",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_AppCustomers_AppCountries_CountryId",
                table: "AppCustomers",
                column: "CountryId",
                principalTable: "AppCountries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppCustomers_AppCountries_CountryId",
                table: "AppCustomers");

            migrationBuilder.DropTable(
                name: "AppCountries");

            migrationBuilder.DropIndex(
                name: "IX_AppCustomers_CountryId",
                table: "AppCustomers");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "AppCustomers");
        }
    }
}
