using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.NotificationService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PlatformManagedTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NotificationTemplates_Code_Channel_Language",
                schema: "notification",
                table: "NotificationTemplates");

            migrationBuilder.DropIndex(
                name: "IX_NotificationTemplates_TenantId_Code_Channel_Language",
                schema: "notification",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "notification",
                table: "NotificationTemplates");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_Code_Channel_Language",
                schema: "notification",
                table: "NotificationTemplates",
                columns: new[] { "Code", "Channel", "Language" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NotificationTemplates_Code_Channel_Language",
                schema: "notification",
                table: "NotificationTemplates");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "notification",
                table: "NotificationTemplates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_Code_Channel_Language",
                schema: "notification",
                table: "NotificationTemplates",
                columns: new[] { "Code", "Channel", "Language" },
                unique: true,
                filter: "[TenantId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_TenantId_Code_Channel_Language",
                schema: "notification",
                table: "NotificationTemplates",
                columns: new[] { "TenantId", "Code", "Channel", "Language" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");
        }
    }
}
