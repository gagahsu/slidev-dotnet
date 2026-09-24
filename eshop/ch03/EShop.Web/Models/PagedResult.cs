namespace EShop.Web.Models;

// 一頁的資料，加上計算頁碼需要的資訊
public record PagedResult<T>(
    List<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}
