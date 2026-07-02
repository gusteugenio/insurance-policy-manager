using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsurancePolicyManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClienteEApolice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Documento = table.Column<string>(type: "TEXT", maxLength: 18, nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Apolices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Numero = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ClienteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Placa = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    ValorPremio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataFim = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apolices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Apolices_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apolices_ClienteId",
                table: "Apolices",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Apolices_DataFim",
                table: "Apolices",
                column: "DataFim");

            migrationBuilder.CreateIndex(
                name: "IX_Apolices_Numero",
                table: "Apolices",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Apolices_Status",
                table: "Apolices",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Documento",
                table: "Clientes",
                column: "Documento",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Apolices");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
