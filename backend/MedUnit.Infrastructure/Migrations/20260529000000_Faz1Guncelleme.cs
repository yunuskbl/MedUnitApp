using System;
using MedUnit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedUnit.Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260529000000_Faz1Guncelleme")]
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
