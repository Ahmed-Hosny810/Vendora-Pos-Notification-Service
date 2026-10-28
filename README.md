# Vendora Notification Service

.NET 10 service foundation. Notification entities, migrations, RabbitMQ consumers and delivery workers are intentionally not implemented yet.

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
