using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MedUnit.Infrastructure.Migrations
{
    public partial class Faz2Guncelleme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Kullanicilar — yeni alanlar
            migrationBuilder.AddColumn<string>(
                name: "Telefon",
                table: "Kullanicilar",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            // Klinikler tablosu
            migrationBuilder.CreateTable(
                name: "Klinikler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "text", nullable: false),
                    Adres = table.Column<string>(type: "text", nullable: true),
                    Telefon = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    AbonelikTipi = table.Column<string>(type: "text", nullable: false),
                    AbonelikBitisTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Aktif = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Klinikler", x => x.Id);
                });

            // KlinikId FK
            migrationBuilder.AddColumn<int>(
                name: "KlinikId",
                table: "Kullanicilar",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_KlinikId",
                table: "Kullanicilar",
                column: "KlinikId");

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanicilar_Klinikler_KlinikId",
                table: "Kullanicilar",
                column: "KlinikId",
                principalTable: "Klinikler",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            // DoktorNotlari tablosu
            migrationBuilder.CreateTable(
                name: "DoktorNotlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RandevuId = table.Column<int>(type: "integer", nullable: false),
                    DoktorId = table.Column<int>(type: "integer", nullable: false),
                    HastaId = table.Column<int>(type: "integer", nullable: false),
                    Not = table.Column<string>(type: "text", nullable: false),
                    Tani = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoktorNotlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoktorNotlari_Kullanicilar_DoktorId",
                        column: x => x.DoktorId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DoktorNotlari_Kullanicilar_HastaId",
                        column: x => x.HastaId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DoktorNotlari_Randevular_RandevuId",
                        column: x => x.RandevuId,
                        principalTable: "Randevular",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoktorNotlari_DoktorId",
                table: "DoktorNotlari",
                column: "DoktorId");

            migrationBuilder.CreateIndex(
                name: "IX_DoktorNotlari_HastaId",
                table: "DoktorNotlari",
                column: "HastaId");

            migrationBuilder.CreateIndex(
                name: "IX_DoktorNotlari_RandevuId",
                table: "DoktorNotlari",
                column: "RandevuId");

            // TibbiDosyalar tablosu
            migrationBuilder.CreateTable(
                name: "TibbiDosyalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HastaId = table.Column<int>(type: "integer", nullable: false),
                    DosyaAdi = table.Column<string>(type: "text", nullable: false),
                    DosyaTipi = table.Column<string>(type: "text", nullable: false),
                    DosyaVerisi = table.Column<byte[]>(type: "bytea", nullable: false),
                    DosyaBoyutu = table.Column<long>(type: "bigint", nullable: false),
                    YuklemeTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TibbiDosyalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TibbiDosyalar_Kullanicilar_HastaId",
                        column: x => x.HastaId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TibbiDosyalar_HastaId",
                table: "TibbiDosyalar",
                column: "HastaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "TibbiDosyalar");
            migrationBuilder.DropTable(name: "DoktorNotlari");
            migrationBuilder.DropForeignKey(name: "FK_Kullanicilar_Klinikler_KlinikId", table: "Kullanicilar");
            migrationBuilder.DropIndex(name: "IX_Kullanicilar_KlinikId", table: "Kullanicilar");
            migrationBuilder.DropColumn(name: "KlinikId", table: "Kullanicilar");
            migrationBuilder.DropTable(name: "Klinikler");
            migrationBuilder.DropColumn(name: "Telefon", table: "Kullanicilar");
        }
    }
}
