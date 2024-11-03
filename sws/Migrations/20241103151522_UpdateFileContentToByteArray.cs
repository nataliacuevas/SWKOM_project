using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sws.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFileContentToByteArray : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "UploadedDocuments");

            migrationBuilder.AddColumn<byte[]>(
                name: "File",
                table: "UploadedDocuments",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "File",
                table: "UploadedDocuments");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "UploadedDocuments",
                type: "text",
                nullable: true);
        }
    }
}
