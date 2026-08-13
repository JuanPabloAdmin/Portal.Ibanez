using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portal.Ibanez.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCountryIsoCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsoCode",
                table: "AppCountries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IsoCode",
                table: "AppCountries",
                type: "character varying(3)",
                maxLength: 3,
                nullable: true);
        }
    }
}
