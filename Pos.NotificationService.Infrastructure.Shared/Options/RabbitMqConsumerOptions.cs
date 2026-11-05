namespace Pos.NotificationService.Infrastructure.Shared.Options;

public class RabbitMqConsumerOptions
{
    public bool Enabled { get; set; }
    public string SourceExchangeName { get; set; } = "inventory.events";
    public string QueueName { get; set; } = "notification.inventory-events";
    public string DeadLetterExchange { get; set; } = "notification.inventory-events.dlx";
    public string DeadLetterQueue { get; set; } = "notification.inventory-events.dlq";
    public string[] RoutingKeys { get; set; } = { "inventory.low-stock.detected.v1" };
    public ushort PrefetchCount { get; set; } = 10;
    public int MaxDeliveryAttempts { get; set; } = 5;
}
