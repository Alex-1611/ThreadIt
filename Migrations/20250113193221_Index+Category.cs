using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreadIt.Migrations
{
    /// <inheritdoc />
    public partial class IndexCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Comments_ThreadId",
                table: "Comments",
                newName: "Index_Comment_ThreadId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ParentId",
                table: "Comments",
                newName: "Index_Comment_ParentID");

            migrationBuilder.CreateIndex(
                name: "Index_Thread_Date",
                table: "Threads",
                column: "CreateTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "Index_Thread_Date",
                table: "Threads");

            migrationBuilder.RenameIndex(
                name: "Index_Comment_ThreadId",
                table: "Comments",
                newName: "IX_Comments_ThreadId");

            migrationBuilder.RenameIndex(
                name: "Index_Comment_ParentID",
                table: "Comments",
                newName: "IX_Comments_ParentId");
        }
    }
}
