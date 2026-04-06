using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterDB.Migrations
{
    /// <inheritdoc />
    public partial class nullableChatIdInClientMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientMessage_ChatMessage_ChatMessageId",
                table: "ClientMessage");

            migrationBuilder.DropIndex(
                name: "IX_ClientMessage_ChatMessageId",
                table: "ClientMessage");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChatMessageId",
                table: "ClientMessage",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_ClientMessage_ChatMessageId",
                table: "ClientMessage",
                column: "ChatMessageId",
                unique: true,
                filter: "[ChatMessageId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientMessage_ChatMessage_ChatMessageId",
                table: "ClientMessage",
                column: "ChatMessageId",
                principalTable: "ChatMessage",
                principalColumn: "MessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientMessage_ChatMessage_ChatMessageId",
                table: "ClientMessage");

            migrationBuilder.DropIndex(
                name: "IX_ClientMessage_ChatMessageId",
                table: "ClientMessage");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChatMessageId",
                table: "ClientMessage",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientMessage_ChatMessageId",
                table: "ClientMessage",
                column: "ChatMessageId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientMessage_ChatMessage_ChatMessageId",
                table: "ClientMessage",
                column: "ChatMessageId",
                principalTable: "ChatMessage",
                principalColumn: "MessageId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
