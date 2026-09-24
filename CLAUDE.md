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

pnpm check:project            # EShop: build (no warnings) + test every eshop/chNN, and match slide excerpts to the files
pnpm check:project ch08 --no-test   # …only ch08, excerpts only
pnpm check:width              # List code lines that are likely too wide for a slide
pnpm check:overflow 3030      # With a dev server running: report slides that overflow or wrap code lines
```

`check:project` needs the .NET 10 SDK; `check:overflow` needs Chromium
(`CHROMIUM_PATH=/opt/pw-browsers/chromium` in the cloud container).

Package manager is **pnpm** (not npm/yarn). The `.npmrc` sets `shamefully-hoist=true` required by Slidev.

## Architecture

All slide files live at the root level. A single server (`pnpm dev`) serves all chapters through one entry point.

- `index.md` — Portal page (課程目錄) with chapter cards; imports every chapter deck via `src:`.
- `chNN-<slug>.md` — one deck per chapter, `routeAlias: chNN`.
- `global-bottom.vue` — footer showing page X/Y on every slide.
- `style.css` — inline code badge, wide code blocks for PDF export, ligature suppression.
- `_template/` — blueprint for a new chapter.
- `eshop/chNN/` — course project **EShop**: the complete reference solution after each chapter's
  「EShop 專案實作」 step (its own `EShop.slnx` + `EShop.Tests`). See `eshop/README.md` for the roadmap.
- `scripts/` — `export-all.mjs` (PDF export), `check-project.mjs`, `check-overflow.mjs`, `check-line-width.py`, `set-zoom.py`.

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

Ch01–Ch04 grow the same project before the lecture uses it (middleware, `Product` / `Category`,
`MemberLevel` discount, in-memory `ProductQueryService`, `/products` storefront); Ch05 continues in
that solution instead of creating a new one.

## EShop 專案實作

- Every chapter ends (before `# 總結`) with a `layout: section`「# EShop 專案實作 / ## 第 N 步：…」divider,
  a task slide (`# EShop 第 N 步：<主題>` + `### 任務說明`, numbered tasks + expected result, often a table),
  and hint slides (`# EShop 第 N 步：解題提示`,「（續）」「（續 2）」…). The Outline has a
  「**EShop 專案實作** — 第 N 步：…」item and the 總結 table an「**EShop** 第 N 步」row, with a matching
  paragraph in the notes before the「下一章」paragraph. Ch01 also has 課程專案介紹 + 成長路線表; Ch10 ends
  with「EShop 完成了！」.
- Each step uses only what has been taught so far. Ch05–Ch10 = the lecture's EShop + one extension task
  that is not a copy of the chapter's 練習 / 綜合練習 (those answers are not in `eshop/`).
- Task and hint titles must be unique; every slide has presenter notes in the 古古 persona.

## Project excerpts (enforced by `pnpm check:project`)

- A code block whose first line is an `eshop/` path must match `eshop/chNN/<path>` exactly:
  `// eshop/EShop.Web/Program.cs` (csharp, js, jsonc), `@* eshop/EShop.Web/Views/Home/Index.cshtml *@` (razor),
  `<!-- eshop/… -->` (xml). A line containing only `// ...` (`@* ... *@`, `<!-- ... -->`) elides code;
  the parts before and after it are matched separately, in order.
- Changing project code: edit `eshop/chNN` **and every later snapshot** that contains the same code, keep
  `dotnet build` warning-free, run `pnpm check:project`, then fix the slides it reports.
- Production code uses SQL Server (connection string in `appsettings.json`, committed migrations); tests use
  SQLite in-memory (`SqliteTestDb`, `EShopWebFactory`, `SqliteModelCustomizer` for decimals,
  `NoMigrationsAssembly` from Ch09) so they run without SQL Server.

## Slide size

- 1280×720. `zoom:` only scales the whole slide — it never lets a code line hold more characters.
- A code line that wraps: shorten the code (in every snapshot and slide), or add `class: code-sm` to the
  slide frontmatter (12px code, ≈ 80 half-width columns; CJK ≈ 1.8). Changing only the `pre` font size does
  nothing — the penguin theme fixes code at 14px, so `style.css` sets `pre code` and `.line`.
- Vertical overflow: use `zoom:` (≥ 0.75) for small overflows, otherwise split into a「（續）」slide.
- Every deck must report 0 overflowing slides and 0 wrapped code lines in `pnpm check:overflow`.

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
