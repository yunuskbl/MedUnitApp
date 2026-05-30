using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedUnit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Faz1Guncelleme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Uzmanlik",
                table: "Kullanicilar",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SifreSifirlamaToken",
                table: "Kullanicilar",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SifreSifirlamaTokenSon",
                table: "Kullanicilar",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Uzmanlik", table: "Kullanicilar");
            migrationBuilder.DropColumn(name: "SifreSifirlamaToken", table: "Kullanicilar");
            migrationBuilder.DropColumn(name: "SifreSifirlamaTokenSon", table: "Kullanicilar");
        }
    }
}
