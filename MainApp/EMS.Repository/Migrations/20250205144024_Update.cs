using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketInOrders_Tickets_TicketId",
                table: "TicketInOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketInShoppingCarts_Tickets_TicketId",
                table: "TicketInShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_TicketInOrders_TicketId",
                table: "TicketInOrders");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "TicketsInEvents",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderedTicketId",
                table: "TicketInOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketInOrders_OrderedTicketId",
                table: "TicketInOrders",
                column: "OrderedTicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInOrders_TicketsInEvents_OrderedTicketId",
                table: "TicketInOrders",
                column: "OrderedTicketId",
                principalTable: "TicketsInEvents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInShoppingCarts_TicketsInEvents_TicketId",
                table: "TicketInShoppingCarts",
                column: "TicketId",
                principalTable: "TicketsInEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketInOrders_TicketsInEvents_OrderedTicketId",
                table: "TicketInOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketInShoppingCarts_TicketsInEvents_TicketId",
                table: "TicketInShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_TicketInOrders_OrderedTicketId",
                table: "TicketInOrders");

            migrationBuilder.DropColumn(
                name: "OrderedTicketId",
                table: "TicketInOrders");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "TicketsInEvents",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_TicketInOrders_TicketId",
                table: "TicketInOrders",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInOrders_Tickets_TicketId",
                table: "TicketInOrders",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketInShoppingCarts_Tickets_TicketId",
                table: "TicketInShoppingCarts",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
