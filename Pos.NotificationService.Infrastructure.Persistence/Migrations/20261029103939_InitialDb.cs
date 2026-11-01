using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.NotificationService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "notification");

            migrationBuilder.CreateTable(
                name: "InboxMessages",
                schema: "notification",
                columns: table => new
                {
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsumerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxMessages", x => new { x.MessageId, x.ConsumerName });
                });

            migrationBuilder.CreateTable(
                name: "NotificationTemplates",
                schema: "notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    NextAttemptAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RecipientUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RecipientEmail = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    RecipientPhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Channel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    NextAttemptAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.UniqueConstraint("AK_Notifications_TenantId_Id", x => new { x.TenantId, x.Id });
                    table.ForeignKey(
                        name: "FK_Notifications_NotificationTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalSchema: "notification",
                        principalTable: "NotificationTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationDeliveryAttempts",
                schema: "notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ProviderMessageId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AttemptedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationDeliveryAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationDeliveryAttempts_Notifications_TenantId_NotificationId",
                        columns: x => new { x.TenantId, x.NotificationId },
                        principalSchema: "notification",
                        principalTable: "Notifications",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InboxMessages_ProcessedAt",
                schema: "notification",
                table: "InboxMessages",
                column: "ProcessedAt");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveryAttempts_TenantId_NotificationId_AttemptNumber",
                schema: "notification",
                table: "NotificationDeliveryAttempts",
                columns: new[] { "TenantId", "NotificationId", "AttemptNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Status_NextAttemptAt_CreatedAt",
                schema: "notification",
                table: "Notifications",
                columns: new[] { "Status", "NextAttemptAt", "CreatedAt" },
                filter: "[Status] = 'Pending'");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TemplateId",
                schema: "notification",
                table: "Notifications",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TenantId_RecipientUserId_CreatedAt_Id",
                schema: "notification",
                table: "Notifications",
                columns: new[] { "TenantId", "RecipientUserId", "CreatedAt", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TenantId_RecipientUserId_ReadAt",
                schema: "notification",
                table: "Notifications",
                columns: new[] { "TenantId", "RecipientUserId", "ReadAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TenantId_SourceEventId_RecipientEmail_Channel_Type",
                schema: "notification",
                table: "Notifications",
                columns: new[] { "TenantId", "SourceEventId", "RecipientEmail", "Channel", "Type" },
                unique: true,
                filter: "[SourceEventId] IS NOT NULL AND [RecipientUserId] IS NULL AND [RecipientEmail] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TenantId_SourceEventId_RecipientPhone_Channel_Type",
                schema: "notification",
                table: "Notifications",
                columns: new[] { "TenantId", "SourceEventId", "RecipientPhone", "Channel", "Type" },
                unique: true,
                filter: "[SourceEventId] IS NOT NULL AND [RecipientUserId] IS NULL AND [RecipientPhone] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TenantId_SourceEventId_RecipientUserId_Channel_Type",
                schema: "notification",
                table: "Notifications",
                columns: new[] { "TenantId", "SourceEventId", "RecipientUserId", "Channel", "Type" },
                unique: true,
                filter: "[SourceEventId] IS NOT NULL AND [RecipientUserId] IS NOT NULL");

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

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_NextAttemptAt_OccurredAt_Id",
                schema: "notification",
                table: "OutboxMessages",
                columns: new[] { "NextAttemptAt", "OccurredAt", "Id" },
                filter: "[PublishedAt] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InboxMessages",
                schema: "notification");

            migrationBuilder.DropTable(
                name: "NotificationDeliveryAttempts",
                schema: "notification");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "notification");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "notification");

            migrationBuilder.DropTable(
                name: "NotificationTemplates",
                schema: "notification");
        }
    }
}
