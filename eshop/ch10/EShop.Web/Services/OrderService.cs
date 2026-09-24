using EShop.DataAccess.Repository.IRepository;
using EShop.Models;
using EShop.Utility;

namespace EShop.Web.Services;

// 結帳流程從 Controller 搬出來：沒有 HTTP 也能測試
public class OrderService(IUnitOfWork unitOfWork, IShippingService shipping)
    : IOrderService
{
    public async Task<PlaceOrderResult> PlaceOrderAsync(
        string userId, OrderHeader shippingInfo)
    {
        var carts = await unitOfWork.ShoppingCart.GetAllAsync(
            filter: c => c.ApplicationUserId == userId,
            includeProperties: "Product");
        if (carts.Count == 0) return new(null, "購物車是空的");

        await using var transaction = await unitOfWork.BeginTransactionAsync();

        foreach (var c in carts)
        {
            var ok = await unitOfWork.Product
                .DecreaseStockAsync(c.ProductId, c.Count);
            if (!ok)   // 直接 return：沒有 Commit，交易自動 Rollback
                return new(null, $"「{c.Product!.Name}」庫存不足，請調整數量");
        }

        var order = BuildOrder(shippingInfo, userId, carts);
        unitOfWork.OrderHeader.Add(order);                // 主檔 + 明細
        unitOfWork.ShoppingCart.RemoveRange(carts);       // 清空購物車
        await unitOfWork.SaveAsync();
        await transaction.CommitAsync();                  // 全部成功才提交

        return new(order.Id, null);
    }

    private OrderHeader BuildOrder(
        OrderHeader order, string userId, List<ShoppingCart> carts)
    {
        order.ApplicationUserId = userId;
        order.OrderDate = DateTime.Now;
        order.OrderStatus = SD.StatusPending;
        // 金額一律由伺服器重新計算，不相信表單送來的值
        order.OrderTotal = carts.Sum(c => c.Product!.Price * c.Count);
        order.ShippingFee = shipping.Calculate(order.OrderTotal);   // 第 6 章的運費服務

        order.OrderDetails = carts.Select(c => new OrderDetail
        {
            ProductId = c.ProductId,
            Count = c.Count,
            Price = c.Product!.Price                      // 下單當下的單價
        }).ToList();

        return order;
    }
}
