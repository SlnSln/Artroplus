using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Artroplus.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TblRoller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblRoller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TblKullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SifreHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblKullanicilar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblKullanicilar_TblRoller_RolId",
                        column: x => x.RolId,
                        principalTable: "TblRoller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TblRoller",
                columns: new[] { "Id", "Ad", "CreatedAt", "IsDeleted", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Yönetici", new DateTime(2026, 5, 1, 14, 40, 32, 799, DateTimeKind.Utc).AddTicks(2242), false, null },
                    { 2, "Kullanıcı", new DateTime(2026, 5, 1, 14, 40, 32, 799, DateTimeKind.Utc).AddTicks(2247), false, null }
                });

            migrationBuilder.InsertData(
                table: "TblKullanicilar",
                columns: new[] { "Id", "Ad", "CreatedAt", "IsDeleted", "KullaniciAdi", "RolId", "SifreHash", "Soyad", "UpdatedAt" },
                values: new object[] { 1, "Sistem", new DateTime(2026, 5, 1, 14, 40, 32, 799, DateTimeKind.Utc).AddTicks(2424), false, "admin", 1, "$2a$11$wE5c010n9a9iF/eA.f/jD.SgD4H0w/6s4.0z2u7g9A1/Y0m4B0B7C", "Yöneticisi", null });

            migrationBuilder.CreateIndex(
                name: "IX_TblKullanicilar_RolId",
                table: "TblKullanicilar",
                column: "RolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblKullanicilar");

            migrationBuilder.DropTable(
                name: "TblRoller");
        }
    }
}
