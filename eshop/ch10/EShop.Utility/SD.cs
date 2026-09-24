namespace EShop.Utility;

public static class SD
{
    // TempData 通知的 key
    public const string Success = "success";
    public const string Error = "error";

    // 角色
    public const string Role_Admin = "Admin";
    public const string Role_Employee = "Employee";
    public const string Role_Customer = "Customer";

    // 授權 policy：規則集中在 Program.cs，Controller 只寫名稱
    public const string Policy_Admin = "AdminOnly";
    public const string Policy_Staff = "Staff";

    // 登入時放進 Cookie 的自訂 claim
    public const string Claim_StoreId = "StoreId";

    // 訂單狀態
    public const string StatusPending = "Pending";          // 待確認
    public const string StatusProcessing = "Processing";    // 處理中
    public const string StatusShipped = "Shipped";          // 已出貨
    public const string StatusCompleted = "Completed";      // 已完成
    public const string StatusCancelled = "Cancelled";      // 已取消

    public static string GetStatusText(string status) => status switch
    {
        StatusPending => "待確認",
        StatusProcessing => "處理中",
        StatusShipped => "已出貨",
        StatusCompleted => "已完成",
        StatusCancelled => "已取消",
        _ => status
    };

    public static string GetStatusBadge(string status) => status switch
    {
        StatusPending => "bg-warning text-dark",
        StatusProcessing => "bg-info text-dark",
        StatusShipped => "bg-primary",
        StatusCompleted => "bg-success",
        _ => "bg-secondary"
    };
}
