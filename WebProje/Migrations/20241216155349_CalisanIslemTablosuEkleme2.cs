using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebProje.Migrations
{
    public partial class CalisanIslemTablosuEkleme2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalisanIslem_Calisanlar_CalisanId",
                table: "CalisanIslem");

            migrationBuilder.DropForeignKey(
                name: "FK_CalisanIslem_Islemler_IslemId",
                table: "CalisanIslem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CalisanIslem",
                table: "CalisanIslem");

            migrationBuilder.RenameTable(
                name: "CalisanIslem",
                newName: "CalisanIslemler");

            migrationBuilder.RenameIndex(
                name: "IX_CalisanIslem_IslemId",
                table: "CalisanIslemler",
                newName: "IX_CalisanIslemler_IslemId");

            migrationBuilder.RenameIndex(
                name: "IX_CalisanIslem_CalisanId",
                table: "CalisanIslemler",
                newName: "IX_CalisanIslemler_CalisanId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CalisanIslemler",
                table: "CalisanIslemler",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CalisanIslemler_Calisanlar_CalisanId",
                table: "CalisanIslemler",
                column: "CalisanId",
                principalTable: "Calisanlar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CalisanIslemler_Islemler_IslemId",
                table: "CalisanIslemler",
                column: "IslemId",
                principalTable: "Islemler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalisanIslemler_Calisanlar_CalisanId",
                table: "CalisanIslemler");

            migrationBuilder.DropForeignKey(
                name: "FK_CalisanIslemler_Islemler_IslemId",
                table: "CalisanIslemler");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CalisanIslemler",
                table: "CalisanIslemler");

            migrationBuilder.RenameTable(
                name: "CalisanIslemler",
                newName: "CalisanIslem");

            migrationBuilder.RenameIndex(
                name: "IX_CalisanIslemler_IslemId",
                table: "CalisanIslem",
                newName: "IX_CalisanIslem_IslemId");

            migrationBuilder.RenameIndex(
                name: "IX_CalisanIslemler_CalisanId",
                table: "CalisanIslem",
                newName: "IX_CalisanIslem_CalisanId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CalisanIslem",
                table: "CalisanIslem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CalisanIslem_Calisanlar_CalisanId",
                table: "CalisanIslem",
                column: "CalisanId",
                principalTable: "Calisanlar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CalisanIslem_Islemler_IslemId",
                table: "CalisanIslem",
                column: "IslemId",
                principalTable: "Islemler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
