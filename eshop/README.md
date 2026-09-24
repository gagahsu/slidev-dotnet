# EShop — 課程專案參考解答

「ASP.NET Core 實戰開發」每一章的最後，都會在同一個專案 **EShop（線上咖啡豆商店）** 上加入新功能。
這個資料夾存放每一章完成後的**完整參考解答**：`ch05/` 就是做完第 5 章「EShop 專案實作」後的樣子。
每個資料夾都是獨立的方案（`EShop.slnx`），可以單獨 `dotnet build`、`dotnet test`。

Ch05～Ch10 的參考解答＝「講義主線做完的 EShop」＋「這一章的延伸任務」；各章的練習與綜合練習答案**不**包含在內。

## 路線表

| 步驟 | 新增的功能 | 用到的觀念 | 主要檔案 |
| --- | --- | --- | --- |
| ch01 | 建立方案；版本標頭 `X-EShop-Version` 與請求計時 Middleware | 專案結構、Middleware | `EShop.Web/Program.cs` |
| ch02 | `Product`（`required`、`field`）、`Category`、`MemberLevel`、record、會員折扣；第一個 xUnit 測試 | 類別、record、switch expression | `Models/`、`Services/PriceCalculator.cs`、`EShop.Tests/` |
| ch03 | 記憶體商品目錄：搜尋、分類、排序、`Skip`/`Take` 分頁、`GroupBy` 統計 | LINQ、延遲執行 | `Data/SeedData.cs`、`Services/ProductQueryService.cs` |
| ch04 | 前台 `/products` 列表與 `/products/{id}` 詳情（找不到回 404）；整合測試 | MVC、Attribute Routing、Tag Helper | `Controllers/ProductsController.cs`、`Views/Products/` |
| ch05 | 講義的分類 CRUD（EF Core、Toastr）＋ **分類名稱不可重複**（去空白、編輯排除自己、唯一索引） | EF Core、Migration、驗證 | `Controllers/CategoryController.cs`、`Data/ApplicationDbContext.cs` |
| ch06 | 講義的 `IShippingService` ＋ **`IProductCatalog`（Singleton）建構子注入**、詳情頁運費試算；測試中替換服務 | DI、生命週期 | `Services/IProductCatalog.cs`、`Program.cs` |
| ch07 | 講義的四專案分層、Repository、UnitOfWork、Area ＋ **前幾章的程式碼搬家**、`/products` 移進 Customer Area、Admin **營運總覽** | 分層架構、Area | `EShop.DataAccess/Catalog/`、`Areas/Admin/Controllers/DashboardController.cs` |
| ch08 | 講義的 Product、`ProductVM`、Upsert、圖片上傳、DataTables、前台首頁 ＋ **記憶體目錄退場，搜尋／分頁／統計改在資料庫執行** | `IQueryable`、ViewModel | `EShop.DataAccess/Repository/ProductRepository.cs` |
| ch09 | 講義的 Identity、`ApplicationUser`、角色、`DbInitializer`、客製化註冊、分店 ＋ **`StoreId` claim 與授權 Policy**（`Staff` / `AdminOnly`） | Identity、Claim、Policy | `Services/ApplicationUserClaimsPrincipalFactory.cs`、`Program.cs` |
| ch10 | 講義的購物車、結帳、訂單、條件式扣庫存、訂單管理 ＋ **結帳抽成 `IOrderService`**、運費改用 `IShippingService`、交易回滾的整合測試 | 交易、整合測試 | `Services/OrderService.cs`、`EShop.Tests/OrderServiceTests.cs` |

## 執行

需要 [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)。

```bash
cd eshop/ch04
dotnet run --project EShop.Web          # http://localhost:5280
dotnet test                             # 執行所有測試
```

Ch01～Ch04 不需要資料庫。Bootstrap、jQuery、Toastr、DataTables 都從 CDN 載入，所以專案裡沒有 `wwwroot/lib`。

## SQL Server（Ch05 起）

正式執行照講義使用 SQL Server。用 Docker 啟動一個本機的 SQL Server：

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=EShop@2026Pass" \
  -p 1433:1433 --name eshop-sql -d mcr.microsoft.com/mssql/server:2022-latest
```

連線字串在 `EShop.Web/appsettings.json` 的 `ConnectionStrings:DefaultConnection`：

```json
"DefaultConnection": "Server=localhost,1433;Database=EShop;User Id=sa;Password=EShop@2026Pass;TrustServerCertificate=True"
```

建立資料庫：

```bash
dotnet tool install --global dotnet-ef

# Ch05～Ch06（單一專案）
cd eshop/ch06/EShop.Web && dotnet ef database update

# Ch07 起（多專案）
cd eshop/ch10
dotnet ef database update --project EShop.DataAccess --startup-project EShop.Web
```

Ch09 起，網站啟動時 `DbInitializer` 會自動套用 Migration，並建立角色、兩間分店與兩個帳號：

| 帳號 | 密碼 | 角色 |
| --- | --- | --- |
| `admin@eshop.com` | `Admin@1234` | Admin |
| `employee@eshop.com` | `Admin@1234` | Employee（信義店） |

Migration 都已經產生好放在專案裡；產生 Migration 不需要連線資料庫，只有 `database update` 需要。

## 測試

- 單元測試與整合測試都在 `EShop.Tests`（xUnit），整合測試用 `WebApplicationFactory` 在記憶體中啟動整個網站。
- **不需要 SQL Server**：Ch05 起測試改用 SQLite in-memory（`SqliteTestDb`、`EShopWebFactory`）。
  - SQLite 不支援 `decimal` 的排序與加總，測試用 `SqliteModelCustomizer` 把 decimal 存成 double（只影響測試）。
  - Migration 是為 SQL Server 產生的，所以測試用 `EnsureCreated` 建表；Ch09 起的 `DbInitializer` 會呼叫 `MigrateAsync`，測試用 `NoMigrationsAssembly` 讓它不做事。
- Ch09 起用 `TestAuthHandler` 以標頭 `X-Test-User: 角色;分店;使用者Id` 模擬登入，驗證授權 Policy。

## 檢查

```bash
pnpm check:project          # 每一步都要 build 無警告、測試通過，且投影片上的程式碼和這裡一致
pnpm check:project ch08     # 只檢查一章
```
