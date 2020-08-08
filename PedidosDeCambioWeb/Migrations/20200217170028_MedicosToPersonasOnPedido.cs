using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PedidosDeCambioWeb.Migrations
{
    public partial class MedicosToPersonasOnPedido : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Medicos_CausanteId",
                table: "Pedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Personas_PersonaId",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_PersonaId",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "PersonaId",
                table: "Pedidos");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Personas_CausanteId",
                table: "Pedidos",
                column: "CausanteId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Personas_CausanteId",
                table: "Pedidos");

            migrationBuilder.AddColumn<Guid>(
                name: "PersonaId",
                table: "Pedidos",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_PersonaId",
                table: "Pedidos",
                column: "PersonaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Medicos_CausanteId",
                table: "Pedidos",
                column: "CausanteId",
                principalTable: "Medicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Personas_PersonaId",
                table: "Pedidos",
                column: "PersonaId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
