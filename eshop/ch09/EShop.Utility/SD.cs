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
}
