using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portal.Ibanez.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentFolderToQrCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DocumentFolderId",
                table: "AppQrCodes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppQrCodes_DocumentFolderId",
                table: "AppQrCodes",
                column: "DocumentFolderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppQrCodes_DocumentFolderId",
                table: "AppQrCodes");

            migrationBuilder.DropColumn(
                name: "DocumentFolderId",
                table: "AppQrCodes");
        }
    }
}
