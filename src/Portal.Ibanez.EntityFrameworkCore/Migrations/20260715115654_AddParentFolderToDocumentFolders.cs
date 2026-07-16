using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portal.Ibanez.Migrations
{
    /// <inheritdoc />
    public partial class AddParentFolderToDocumentFolders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentFolderId",
                table: "AppDocumentFolders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppDocumentFolders_ParentFolderId",
                table: "AppDocumentFolders",
                column: "ParentFolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppDocumentFolders_AppDocumentFolders_ParentFolderId",
                table: "AppDocumentFolders",
                column: "ParentFolderId",
                principalTable: "AppDocumentFolders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppDocumentFolders_AppDocumentFolders_ParentFolderId",
                table: "AppDocumentFolders");

            migrationBuilder.DropIndex(
                name: "IX_AppDocumentFolders_ParentFolderId",
                table: "AppDocumentFolders");

            migrationBuilder.DropColumn(
                name: "ParentFolderId",
                table: "AppDocumentFolders");
        }
    }
}
