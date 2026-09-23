---
theme: penguin
class: text-center
highlighter: shiki
lineNumbers: true
drawings:
  persist: false
transition: slide-left
fonts:
  provider: none
title: ASP.NET Core Masterclass
routeAlias: home
style: |
  .slidev-layout p,
  .slidev-layout li,
  .slidev-layout td,
  .slidev-layout th,
  .slidev-layout div {
    font-size: max(16px, 1em);
  }
  table {
    width: 100%;
    margin: 1rem 0;
    border-collapse: collapse;
  }
  th, td {
    padding: 8px !important;
    border: 1px solid #e2e8f0 !important;
  }
  .index-table td {
    text-align: center;
    font-family: monospace;
  }
---

<style>
.chapter-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1rem;
  width: 100%;
  max-width: 960px;
  margin-top: 1.2rem;
}
.chapter-card {
  display: block;
  background: #f0faf9;
  border: 2px solid #5eada0;
  border-radius: 12px;
  padding: 1.2rem 0.8rem;
  text-decoration: none !important;
  color: #1a5c5c !important;
  transition: all 0.2s ease;
}
.chapter-card:hover {
  background: #5eada0;
  color: white !important;
  transform: translateY(-3px);
  box-shadow: 0 6px 16px rgba(94, 173, 160, 0.35);
}
.chapter-card:hover .chapter-subtitle {
  color: rgba(255,255,255,0.85) !important;
}
.chapter-num {
  font-size: 1.6rem;
  font-weight: 900;
  margin-bottom: 0.3rem;
}
.chapter-subtitle {
  font-size: max(13px, 0.88rem);
  color: #4a7c7c;
  margin-top: 0.3rem;
}
.chapter-card-adv {
  border-color: #e0a96d;
}
.chapter-card-adv .chapter-badge {
  color: #c97b2c;
  font-size: 0.75rem;
  font-weight: 700;
  letter-spacing: 0.1em;
}
</style>

<div class="flex flex-col items-center h-full" style="background: #ffffff; overflow-y: auto; padding: 1.5rem 0;">
  <p style="color: #5eada0; font-size: 1rem; font-weight: 600; letter-spacing: 0.2em; text-transform: uppercase; margin-bottom: 1rem;">ASP.NET Core Masterclass</p>
  <h1 style="color: #1a5c5c; font-size: 2.8rem; font-weight: 900; line-height: 1.2; margin-bottom: 0.5rem;">課程目錄</h1>
  <div style="height: 4px; width: 240px; background: linear-gradient(90deg, #5eada0, #a7d9d0); border-radius: 2px; margin-bottom: 0.5rem;"></div>
  <p style="color: #9dc4c4; font-size: 0.9rem; margin-bottom: 0;">.NET 10 · C# 14 · EF Core 10 — 點擊章節卡片開始學習</p>
  <div class="chapter-grid">
    <Link to="ch01" class="chapter-card">
      <div class="chapter-num">Ch 1</div>
      <div>環境建置 & 關於 .NET 10</div>
      <div class="chapter-subtitle">Setup &amp; .NET 10</div>
    </Link>
    <Link to="ch02" class="chapter-card">
      <div class="chapter-num">Ch 2</div>
      <div>C# 基礎語法</div>
      <div class="chapter-subtitle">C# Fundamentals</div>
    </Link>
    <Link to="ch03" class="chapter-card">
      <div class="chapter-num">Ch 3</div>
      <div>LINQ 與資料查詢</div>
      <div class="chapter-subtitle">LINQ &amp; Querying</div>
    </Link>
    <Link to="ch04" class="chapter-card">
      <div class="chapter-num">Ch 4</div>
      <div>MVC 基本觀念</div>
      <div class="chapter-subtitle">MVC Concepts</div>
    </Link>
    <Link to="ch05" class="chapter-card">
      <div class="chapter-num">Ch 5</div>
      <div>CRUD 實作練習</div>
      <div class="chapter-subtitle">CRUD with EF Core</div>
    </Link>
    <Link to="ch06" class="chapter-card">
      <div class="chapter-num">Ch 6</div>
      <div>依賴注入</div>
      <div class="chapter-subtitle">Dependency Injection</div>
    </Link>
    <Link to="ch07" class="chapter-card">
      <div class="chapter-num">Ch 7</div>
      <div>系統架構與分層</div>
      <div class="chapter-subtitle">Repository &amp; UnitOfWork</div>
    </Link>
    <Link to="ch08" class="chapter-card">
      <div class="chapter-num">Ch 8</div>
      <div>Product 商品管理與首頁</div>
      <div class="chapter-subtitle">Product &amp; Home Page</div>
    </Link>
    <Link to="ch09" class="chapter-card">
      <div class="chapter-num">Ch 9</div>
      <div>會員與權限控管</div>
      <div class="chapter-subtitle">ASP.NET Core Identity</div>
    </Link>
    <Link to="ch10" class="chapter-card">
      <div class="chapter-num">Ch 10</div>
      <div>購物車與訂單系統</div>
      <div class="chapter-subtitle">Shopping Cart &amp; Orders</div>
    </Link>
  </div>
</div>

<!--
大家好，歡迎來到「ASP.NET Core 實戰開發」這門課！

這門課我們會從環境建置、C# 語法、LINQ 開始打底，接著學 MVC 架構與 Entity Framework Core，一路做到依賴注入、分層架構，最後完成一個有商品管理、會員權限、購物車和訂單的線上商店 EShop。

每一章都會先回顧上一章、再說明「為什麼需要」，最後帶著大家把程式碼實際跑起來。點擊任何一張章節卡片，就可以開始學習。
-->

---
src: ./ch01-dotnet-intro.md
---

---
src: ./ch02-csharp-basics.md
---

---
src: ./ch03-linq.md
---

---
src: ./ch04-mvc-concepts.md
---
