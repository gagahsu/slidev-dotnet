using EShop.Models;

namespace EShop.Web.Services;

// 下單結果：成功時有訂單編號，失敗時有原因
public record PlaceOrderResult(int? OrderId, string? Error)
{
    public bool Succeeded => OrderId is not null;
}

public interface IOrderService
{
    Task<PlaceOrderResult> PlaceOrderAsync(string userId, OrderHeader shippingInfo);
}
