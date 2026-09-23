# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a **Slidev-based teaching presentation** for an **ASP.NET Core 實戰開發** course (10 chapters, .NET 10 / C# 14).
It is **not** a .NET application — the content is about ASP.NET Core, but the repository itself is a Node.js/Slidev project.

## Commands

```bash
pnpm install          # Install dependencies
pnpm dev              # Start single dev server at localhost:3030 (all chapters)
pnpm run ch05         # Start only one chapter deck (ch01 … ch10)
pnpm build            # Build to dist/ for deployment
pnpm run export:all   # Export all chapter decks to dist/*.pdf (accepts "ch05" or "3-7")
```

Package manager is **pnpm** (not npm/yarn). The `.npmrc` sets `shamefully-hoist=true` required by Slidev.

## Architecture

All slide files live at the root level. A single server (`pnpm dev`) serves all chapters through one entry point.

- `index.md` — Portal page (課程目錄) with chapter cards; imports every chapter deck via `src:`.
- `chNN-<slug>.md` — one deck per chapter, `routeAlias: chNN`.
- `global-bottom.vue` — footer showing page X/Y on every slide.
- `style.css` — inline code badge, wide code blocks for PDF export, ligature suppression.
- `_template/` — blueprint for a new chapter.

Chapter numbers are dense and 1-indexed; the deck number, the `routeAlias`, and the `Ch N` label on
the index card must always agree.

| Deck | routeAlias | 主題 |
| --- | --- | --- |
| `ch01-dotnet-intro.md` | ch01 | 環境建置 & 關於 .NET 10 |
| `ch02-csharp-basics.md` | ch02 | C# 基礎語法 |
| `ch03-linq.md` | ch03 | LINQ 與資料查詢 |
| `ch04-mvc-concepts.md` | ch04 | MVC 基本觀念 |
| `ch05-crud.md` | ch05 | CRUD 實作練習（EF Core、Toastr） |
| `ch06-dependency-injection.md` | ch06 | 依賴注入 |
| `ch07-layered-architecture.md` | ch07 | 系統架構與分層（Repository、UnitOfWork、Area） |
| `ch08-product.md` | ch08 | Product 商品管理與首頁 |
| `ch09-identity.md` | ch09 | 會員與權限控管（Identity） |
| `ch10-cart-order.md` | ch10 | 購物車與訂單系統 |

## Running example project

Ch05–Ch10 build one continuous project, **EShop**（線上咖啡豆商店）:

- Ch05: `EShop.Web` single MVC project, `Category` CRUD with EF Core + SQL Server
- Ch07: split into `EShop.Models` / `EShop.DataAccess` / `EShop.Utility` / `EShop.Web`; `Areas/Admin` & `Areas/Customer`
- Ch08: `Product` + `ProductVM`, image upload to `wwwroot/images/product`, DataTables
- Ch09: Identity with `ApplicationUser`, roles in `SD` (`Admin` / `Employee` / `Customer`), `Store`（分店）
- Ch10: `ShoppingCart`, `OrderHeader`, `OrderDetail`, stock deduction, order status

Keep names consistent across chapters when editing.

## Slide Authoring Conventions

- **Language:** Traditional Chinese (zh-TW); English for code identifiers and technical terms
- **Syntax baseline:** .NET 10 / C# 14 / EF Core 10 — file-scoped namespaces, top-level statements,
  primary constructors, collection expressions, `required` members, nullable reference types, `MapStaticAssets()`
- **Persona (講稿):** presenter notes (`<!-- -->`) follow the 古古 persona from slidev-springboot `kucw-persona.md`:
  「我們」稱呼讀者、先情境再定義、日常生活類比、固定小節前綴（`回顧：`、`什麼是 XXX？`、
  `在 ASP.NET Core 中練習 XXX 的用法`、`使用 XXX 的注意事項`、`補充：`、`總結`），結尾預告下一章
- **Cover slide:** white flexbox with teal gradient divider + `<Link to="home">← 返回目錄</Link>`
- **Section dividers:** `layout: section` + `class: flex flex-col justify-center items-center text-center`
- **Code blocks:** ` ```csharp `, ` ```razor `（Razor views；Shiki has no `cshtml`）, ` ```json `, ` ```bash `; split long code into a「— 範例」page
- **Callouts:** `<div class="mt-4 p-3 bg-blue-50 border-l-4 border-blue-400 text-gray-700 text-sm text-left">`
- **Practice:** each section ends with a 練習 (任務說明 + 解題提示), each chapter ends with 綜合練習
- Never write `{{ }}` outside code blocks (Vue interpolation) and always wrap generics like `List<T>` in backticks
- Theme is `penguin` for all decks
