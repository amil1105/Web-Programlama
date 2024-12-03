using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebProje.Migrations
{
    public partial class AddKullaniciTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Randevular_Kullanici_KullaniciId",
                table: "Randevular");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Kullanici",
                table: "Kullanici");

            migrationBuilder.RenameTable(
                name: "Kullanici",
                newName: "Kullanicilar");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kullanicilar",
                table: "Kullanicilar",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Randevular_Kullanicilar_KullaniciId",
                table: "Randevular",
                column: "KullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Randevular_Kullanicilar_KullaniciId",
                table: "Randevular");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Kullanicilar",
                table: "Kullanicilar");

            migrationBuilder.RenameTable(
                name: "Kullanicilar",
                newName: "Kullanici");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kullanici",
                table: "Kullanici",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Randevular_Kullanici_KullaniciId",
                table: "Randevular",
                column: "KullaniciId",
                principalTable: "Kullanici",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
