# Vendora Notification Service

.NET 10 service foundation. Notification entities and EF mappings are included. Migrations, RabbitMQ consumers and delivery workers are not implemented yet.

## Projects

- Core / Domain: models and constants.
- Core / Application: requests/handlers, validators, interfaces and response wrappers.
- Infrastructure / Persistence: EF Core SQL Server context, configurations, generic repository and unit of work.
- Infrastructure / Shared: HTTP clients and future messaging/delivery integrations.
- WebApi: authentication, current user, policies, versioned Swagger, logging and exception handling.

Application references Domain. Infrastructure references Application. WebApi composes the layers.

## Development

Open Pos-NotificationService.slnx, select WebApi as startup project, or run:

    dotnet run --project Pos.NotificationService.WebApi --launch-profile https

Swagger: https://localhost:7189/swagger
Liveness: https://localhost:7189/health/live

Development settings use Server=AHMEDHOSNEY and Database=PosNotificationDb. Change the connection string for your machine. No database is created automatically and no migration has been applied.

Identity issuer: Services:Identity:Issuer (development default https://localhost:7083/).
Use valid Identity access tokens. CurrentUserService reads user, tenant and roles from claims.
The Notifications.Own policy requires an authenticated tenant user with valid tenant/user IDs; subscription expiry does not block billing notifications.
Future notification queries must also scope records to the recipient, not only the tenant.

For deployment set ConnectionStrings__DefaultConnection and Services__Identity__Issuer through configuration/secrets. Do not commit provider credentials or API keys.

RabbitMQ.Client and HttpClient infrastructure are available. Broker connections, consumers and email providers are added with their business features; startup does not contact RabbitMQ.

## EF migrations (after models exist)

    dotnet ef migrations add InitialCreate --project Pos.NotificationService.Infrastructure.Persistence --startup-project Pos.NotificationService.WebApi
    dotnet ef database update --project Pos.NotificationService.Infrastructure.Persistence --startup-project Pos.NotificationService.WebApi

Owned schema and migration history schema: notification.
Liveness only verifies that the API is running, not database or broker readiness.

## Entities
Notification stores one recipient/channel message and its read/retry state. NotificationTemplate stores platform-managed templates shared across tenants. NotificationDeliveryAttempt records provider attempts. InboxMessage deduplicates consumers; OutboxMessage stages outgoing events.

Content is stored on Notification so template edits do not change old messages. Cross-service IDs have no foreign keys. Future handlers must validate recipient/channel combinations, scope all reads to tenant/recipient. Background consumers obtain tenant scope from validated events rather than HTTP claims. No automatic tenant filter is applied to this worker-facing context.

## Recipient client

INotificationRecipientClient calls POST /api/v1/internal/notification-recipients.
Identity must implement this endpoint before end-to-end use.

Request: TenantId, optional BranchId, Roles (TenantOwner, Admin, Cashier, InventoryStaff).
Identity should return active users from the specified tenant with ANY requested role, deduplicated.
Tenant-wide owners/admins remain eligible with BranchId; branch-scoped staff require verified assignments. Reject unsupported branch scoping rather than returning all staff.

Response: { "succeeded": true, "data": [ { "userId": "...", "fullName": "...", "email": null, "phoneNumber": null } ] }.
A successful empty array is valid. HTTP errors, timeouts and malformed responses return failure; caller cancellation propagates.

Configure IdentityRecipientClient:BaseUrl and IdentityRecipientClient:ApiKey through configuration/user secrets.
The client sends X-Internal-Api-Key with a dedicated credential that Identity must authenticate.
No user token, automatic retries, endpoint implementation or credential provisioning is included. Event processing owns bounded retries.

## Create notification templates

POST /api/v1/notification-templates with an Identity bearer token.
Only Platform Admin/SuperAdmin users can create templates. Tenant users cannot manage templates.
Templates have no TenantId; notifications retain their tenant and recipient ownership.
Codes are stored trimmed and uppercase; language tags trimmed and lowercase. Channel values use the defined constants exactly.
Duplicate code/channel/language combinations are rejected across the platform, including inactive templates.
Body is limited to 20,000 characters at the API boundary.

Example:
{
  "code": "LowStockDetected",
  "channel": "InApp",
  "language": "en",
  "subject": "Low stock alert",
  "body": "Product {{ProductId}} has {{QuantityOnHand}} units remaining."
}

Creation stores template text; INotificationTemplateService selects and renders it when creating notifications.
Apply the existing model migration before calling the endpoint. This feature does not apply database migrations.


## Template lookup and rendering

Inject INotificationTemplateService and call RenderAsync with code, channel, language,
placeholder values and cancellation token. One read-only query finds the active platform
template by Code, Channel and Language. There is no tenant or language fallback.
Subject becomes Title; placeholders are replaced with the supplied values.
Missing values, malformed placeholders, missing templates and titles exceeding 200
characters return Result failures. Replacement is a single pass; values are not rendered
recursively. Callers format dates and numbers before passing strings.
Content is plain text, not HTML. The service returns TemplateId, Title and Body;
it does not save or send notifications.

After this model change, create and review a PlatformManagedTemplates migration before
updating the database. Existing tenant templates must be reviewed for duplicate
Code/Channel/Language combinations before the platform-wide unique index can be applied.

## In-app notification creation

INotificationCreationService.CreateAsync accepts CreateNotificationRequest containing
TenantId, SourceEventId, TemplateCode, language, recipient roles, placeholder Values,
and optional branch/reference fields. Channel currently supports InApp only.
TemplateCode is normalized into Notification.Type and is limited to 80 characters.

The service explicitly validates worker input, resolves active tenant recipients through
Identity, deduplicates user IDs, renders once, and stages one record per recipient.
No recipients returns success with count zero. Lookup/rendering failures return Result
failures; caller cancellation propagates. No records are staged before lookup/rendering
succeed. In-app records use Sent/SentAt because they become available on commit;
ReadAt remains null. Recipient email/phone are not stored for this in-app flow.

The result count describes staged records, not committed records. The dispatcher must
check the inbox first, then commit notifications and its inbox record in the same local
transaction and acknowledge only after success. On failure/cancellation, discard the
scope and its tracked changes. Do not save failed work. This service does not independently
deduplicate redelivered events; the inbox and existing notification uniqueness indexes
provide that protection. No endpoint, migration, RabbitMQ consumer or email worker is
added by this feature.
