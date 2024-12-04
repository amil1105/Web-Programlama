using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebProje.Migrations
{
    public partial class AddCalisan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calisanlar_Magazalar_MagazaId",
                table: "Calisanlar");

            migrationBuilder.RenameColumn(
                name: "UygunlukSaatleri",
                table: "Calisanlar",
                newName: "Adres");

            migrationBuilder.AlterColumn<int>(
                name: "MagazaId",
                table: "Calisanlar",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<long>(
                name: "Telefon",
                table: "Calisanlar",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_Calisanlar_Magazalar_MagazaId",
                table: "Calisanlar",
                column: "MagazaId",
                principalTable: "Magazalar",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calisanlar_Magazalar_MagazaId",
                table: "Calisanlar");

            migrationBuilder.DropColumn(
                name: "Telefon",
                table: "Calisanlar");

            migrationBuilder.RenameColumn(
                name: "Adres",
                table: "Calisanlar",
                newName: "UygunlukSaatleri");

            migrationBuilder.AlterColumn<int>(
                name: "MagazaId",
                table: "Calisanlar",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Calisanlar_Magazalar_MagazaId",
                table: "Calisanlar",
                column: "MagazaId",
                principalTable: "Magazalar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
