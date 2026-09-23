# ASP.NET Core 實戰開發（Slidev 教材）

以 **.NET 10 / C# 14 / EF Core 10** 為基準的 ASP.NET Core 教學投影片，共 10 章。
全課程從環境建置一路做到一個完整的線上咖啡豆商店 **EShop**（前後台、會員權限、購物車、訂單與庫存）。

## 快速開始

```bash
pnpm install
pnpm dev            # http://localhost:3030 ，目錄頁可點選各章
pnpm run ch05       # 只開啟單一章節
pnpm build          # 輸出靜態網站到 dist/
pnpm run export:all # 各章匯出 PDF 到 dist/
```

## 課綱

| 章 | 主題 | 小節 |
| --- | --- | --- |
| Ch01 | 環境建置 & 關於 .NET 10 | ASP.NET Core 簡介、.NET 10 簡介、VS Code 開發環境、網站生命週期（Middleware Pipeline） |
| Ch02 | C# 基礎語法 | 程式架構、語法介紹、條件控制、迴圈控制、類別與物件（primary constructor、record） |
| Ch03 | LINQ 與資料查詢 | 委派與 Lambda、查詢 / 方法語法、Where/Select/OrderBy、FirstOrDefault/Any/Count/GroupBy、延遲執行、IEnumerable vs IQueryable |
| Ch04 | MVC 基本觀念 | MVC 概觀、檔案配置、職責、架構、.NET 中的 MVC（Routing、IActionResult、Razor、Tag Helper） |
| Ch05 | CRUD 實作練習 | 建立專案、EF Core 與 Migration、Read/Create/Edit/Delete、TempData & Toastr |
| Ch06 | 依賴注入 | 介紹、IoC、DI、Transient / Scoped / Singleton |
| Ch07 | 系統架構與分層 | 分層架構、多專案重構、泛型 Repository、UnitOfWork、Area |
| Ch08 | Product 商品管理與首頁 | Product Model、CRUD、關聯與圖片欄位、ViewBag/ViewData/ViewModel、Upsert、圖片上傳與靜態檔案、DataTable、前台首頁 |
| Ch09 | 會員與權限控管 | Identity、註冊登入、角色授權、客製化註冊、分店資訊 |
| Ch10 | 購物車與訂單系統 | ShoppingCart、購物車 VM、增減移除、結算畫面、OrderHeader/OrderDetail、訂單 Repository、購物車轉訂單、送出訂單與庫存、訂單狀態管理 |

## 撰寫慣例

- 版面架構沿用 `slidev-java`（penguin 主題、目錄頁、`routeAlias` + `<Link>` 導覽、`_template/`）
- 講稿（presenter notes）沿用 `slidev-springboot` 的古古人設：先情境再定義、生活類比、
  `回顧：` → `什麼是 XXX？` → `在 ASP.NET Core 中練習` → `注意事項` → `總結` 並預告下一章
- 每個小節附練習（任務說明 + 解題提示），每章最後有綜合練習

詳細規範見 [`CLAUDE.md`](./CLAUDE.md)。
