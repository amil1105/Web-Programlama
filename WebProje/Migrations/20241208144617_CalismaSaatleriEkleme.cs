using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebProje.Migrations
{
    public partial class CalismaSaatleriEkleme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calisanlar_Magazalar_MagazaId",
                table: "Calisanlar");

            migrationBuilder.DropIndex(
                name: "IX_Calisanlar_MagazaId",
                table: "Calisanlar");

            migrationBuilder.DropColumn(
                name: "CalismaSaatleri",
                table: "Magazalar");

            migrationBuilder.DropColumn(
                name: "MagazaId",
                table: "Calisanlar");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "AcilisSaati",
                table: "Magazalar",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "KapanisSaati",
                table: "Magazalar",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcilisSaati",
                table: "Magazalar");

            migrationBuilder.DropColumn(
                name: "KapanisSaati",
                table: "Magazalar");

            migrationBuilder.AddColumn<string>(
                name: "CalismaSaatleri",
                table: "Magazalar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MagazaId",
                table: "Calisanlar",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Calisanlar_MagazaId",
                table: "Calisanlar",
                column: "MagazaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Calisanlar_Magazalar_MagazaId",
                table: "Calisanlar",
                column: "MagazaId",
                principalTable: "Magazalar",
                principalColumn: "Id");
        }
    }
}
