namespace Pos.NotificationService.Application.Constants;

public static class TenantRoles
{
    public const string TenantOwner = "TenantOwner";
    public const string Admin = "Admin";
    public const string Cashier = "Cashier";
    public const string InventoryStaff = "InventoryStaff";

    public static bool IsSupported(string role) =>
        role is TenantOwner or Admin or Cashier or InventoryStaff;
}
