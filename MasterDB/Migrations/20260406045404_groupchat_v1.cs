using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterDB.Migrations
{
    /// <inheritdoc />
    public partial class groupchat_v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientMessage",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 256, nullable: false),
                    ChatMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdatedOnUTC = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientMessage", x => new { x.TransactionId, x.SenderId });
                    table.ForeignKey(
                        name: "FK_ClientMessage_ChatMessage_ChatMessageId",
                        column: x => x.ChatMessageId,
                        principalTable: "ChatMessage",
                        principalColumn: "MessageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SentEventToQueueMappings",
                columns: table => new
                {
                    ClientTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AzureMessageId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    AzureCorrelationId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ProcessCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SentEventToQueueMappings", x => x.ClientTransactionId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientMessage_ChatMessageId",
                table: "ClientMessage",
                column: "ChatMessageId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientMessage");

            migrationBuilder.DropTable(
                name: "SentEventToQueueMappings");
        }
    }
}
