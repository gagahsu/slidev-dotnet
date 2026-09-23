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
title: 依賴注入（Dependency Injection）
routeAlias: ch06
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

<div class="flex flex-col justify-center items-center h-full" style="background: #ffffff;">
  <p style="color: #5eada0; font-size: 1rem; font-weight: 600; letter-spacing: 0.2em; text-transform: uppercase; margin-bottom: 1.2rem;">ASP.NET Core Masterclass</p>
  <h1 style="color: #1a5c5c; font-size: 3.4rem; font-weight: 900; line-height: 1.15; margin-bottom: 1.5rem;">依賴注入（Dependency Injection）</h1>
  <div style="height: 4px; width: 320px; background: linear-gradient(90deg, #5eada0, #a7d9d0); border-radius: 2px; margin-bottom: 1.5rem;"></div>
  <p style="color: #4a7c7c; font-size: 1.15rem; font-style: italic;">「需要什麼就開口，不用自己 new」</p>
  <Link to="home" style="color: #9dc4c4; font-size: 0.85rem; margin-top: 2rem; text-decoration: none; letter-spacing: 0.05em;">← 返回目錄</Link>
</div>

<!--
大家好，歡迎來到第六章！

上一章我們完成了分類的 CRUD，但留下了一個問題：CategoryController 的建構子有一個 ApplicationDbContext 參數，我們從來沒有 new 過它，它是從哪裡來的？

答案就是這一章的主題：依賴注入，英文是 Dependency Injection，簡稱 DI。DI 是 ASP.NET Core 最核心的設計，框架裡幾乎所有東西，包括 DbContext、Logger、設定檔，都是透過 DI 取得的。

這一章我們會先看沒有 DI 會遇到什麼問題，接著介紹 IoC 控制反轉和 DI 依賴注入這兩個觀念，最後學習 DI 容器中三種服務生命週期的差別。
-->

---
layout: default
---

# Outline

- **回顧：CategoryController 的建構子**
- **6-1 介紹** — 沒有 DI 會遇到什麼問題？
- **6-2 IoC 控制反轉**
- **6-3 DI 依賴注入** — 介面、註冊、建構子注入
- **6-4 服務的生命週期** — Transient、Scoped、Singleton
- **總結**

<!--
這一章有四個小節，觀念比較抽象，所以我們會用一個「運費計算」的例子貫穿整章，一步一步把程式從「緊耦合」改成「依賴注入」。
-->

---

# 回顧：CategoryController 的建構子

```csharp
// Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CategoryController.cs
public class CategoryController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index() => View(await db.Categories.ToListAsync());
}
```

「我們從來沒有寫過 `new ApplicationDbContext(...)`，那 `db` 是誰給的？」

<!--
我們先回顧上一章的兩段程式碼。

在 Program.cs，我們用 AddDbContext 註冊了 ApplicationDbContext。在 CategoryController，我們只在建構子寫了一個 ApplicationDbContext 參數，就可以直接使用 db 查詢資料了。

整個專案裡，我們從來沒有寫過 new ApplicationDbContext，那這個 db 到底是誰建立、誰傳進來的？這一章就要來回答這個問題。
-->

---
layout: section
class: flex flex-col justify-center items-center text-center
---

# 6-1 介紹
### Why Dependency Injection?

<!--
第一個小節，我們先不談 DI，而是看看沒有 DI 的程式會遇到什麼問題。
-->

---

# 情境：EShop 要計算運費

一開始，EShop 只跟黑貓宅急便合作：

```csharp
public class BlackCatShipping
{
    public decimal Calculate(decimal orderTotal) => orderTotal >= 1500 ? 0 : 150;
}

public class CheckoutController : Controller
{
    private readonly BlackCatShipping _shipping = new BlackCatShipping();  // 自己 new

    public IActionResult Index(decimal total)
    {
        var fee = _shipping.Calculate(total);
        return Content($"運費：{fee} 元");
    }
}
```

<!--
我們用一個情境貫穿整章：EShop 結帳時要計算運費。

一開始只跟黑貓宅急便合作，所以我們寫了一個 BlackCatShipping 類別，滿 1500 免運，否則 150 元。CheckoutController 需要計算運費，就自己 new 一個 BlackCatShipping 來用。

這段程式碼可以正常執行，看起來也沒什麼問題。但接下來老闆的需求來了。
-->

---

# 問題來了：換物流商

老闆說：「下個月改跟超商取貨合作，運費一律 60 元。」

| 問題 | 說明 |
| --- | --- |
| 要改 Controller 的程式碼 | `CheckoutController` 寫死了 `new BlackCatShipping()` |
| 很多地方都用到 | 購物車、結帳、訂單……每個地方都要改 |
| 無法測試 | 想測試 Controller，卻一定會連帶執行真正的運費計算 |

「`CheckoutController` 和 `BlackCatShipping` **綁死**了，這就是**緊耦合（Tight Coupling）**。」

<!--
老闆說下個月要改跟超商取貨合作，運費一律 60 元。

這時候問題就出現了：CheckoutController 裡寫死了 new BlackCatShipping，要換物流商，就一定要改 Controller 的程式碼。如果購物車、結帳、訂單三個地方都有用到，就要改三個地方。

更麻煩的是測試：如果物流商的運費要呼叫外部 API 才能算出來，我們想測試 Controller 的時候，就一定會真的去呼叫那個 API。

這種「A 類別自己 new 了 B 類別，兩個綁死在一起」的情況，就叫做緊耦合。
-->

---

# 生活類比：插座與電器

| 設計 | 比喻 | 結果 |
| --- | --- | --- |
| 緊耦合 | 電器的電線**直接焊死**在牆壁的電線上 | 換電器要拆牆 |
| 鬆耦合 | 牆上裝**插座（介面）**，電器用插頭接上 | 換電器只要拔插頭 |

「只要大家都遵守插座的規格（介面），誰插上去都可以用。」

<!--
用插座來比喻就很清楚了。

緊耦合就像把電風扇的電線直接焊在牆壁的電線上，能用是能用，但哪天要換成冷氣，就得把牆拆開重新接線。

鬆耦合則是在牆上裝一個插座，插座有統一的規格。電風扇、冷氣、電鍋，只要插頭符合規格，插上去就能用，換電器只要拔插頭。

在程式裡，這個「插座規格」就是介面，interface。接下來兩個小節，我們就要用介面加上 IoC 和 DI，把緊耦合的程式改成鬆耦合。
-->

---
layout: default
---

# 練習 1：找出緊耦合
### 認證模擬題（單選）

下列哪一段程式碼的耦合程度**最高**？

A. `public class OrderController(IShippingService shipping) : Controller`
B. `private readonly BlackCatShipping _shipping = new();`
C. `builder.Services.AddScoped<IShippingService, BlackCatShipping>();`
D. `public interface IShippingService { decimal Calculate(decimal total); }`

<!--
【出題動機】
確認大家能辨認「自己 new 具體類別」就是緊耦合的訊號。

【解題引導】
哪一個選項把具體的物流商名稱寫死在使用它的類別裡？
-->

---
layout: default
---

# 練習 1：解析

**正確答案：B**

| 選項 | 解析 |
| --- | --- |
| A | 只依賴介面，由外部傳入 → 鬆耦合 |
| B ✅ | 在類別內部 `new` 具體類別 → **緊耦合** |
| C | 在 Program.cs 集中設定對應關係，這是 DI 的註冊方式 |
| D | 介面本身只是規格，沒有耦合問題 |

<!--
答案是 B。看到在類別裡面 new 一個具體的類別，就是緊耦合的訊號。A 和 C 就是等一下我們要學的寫法。
-->

---
layout: section
class: flex flex-col justify-center items-center text-center
---

# 6-2 IoC 控制反轉
### Inversion of Control

<!--
第二個小節，我們來看解決緊耦合的第一個觀念：IoC 控制反轉。
-->

---

# 什麼是 IoC？

| 名詞 | 意義 |
| --- | --- |
| **控制** | 「要用哪一個物件、什麼時候建立它」的決定權 |
| **反轉** | 決定權從「類別自己」轉移到「外部」 |

「IoC（Inversion of Control，控制反轉）的概念，就是**把物件的控制權，交給外部的容器來管理**。」

| | 傳統寫法 | IoC |
| --- | --- | --- |
| 誰建立物件 | `CheckoutController` 自己 `new` | 外部的 **IoC 容器** |
| 誰決定用哪家物流 | `CheckoutController` | 在 `Program.cs` 集中設定 |

<!--
IoC 全名是 Inversion of Control，控制反轉。我們把這四個字拆開來看。

「控制」指的是決定權：要用哪個物件、什麼時候建立它。「反轉」是這個決定權的方向反過來了：原本是 CheckoutController 自己決定要 new 黑貓，現在改成由外部決定。

「IoC 的概念，就是把物件的控制權，交給外部的容器來管理」。在 ASP.NET Core 裡，這個外部容器就是內建的 DI 容器，而我們在 Program.cs 用 builder.Services 註冊服務，就是在告訴容器要怎麼建立物件。
-->

---

# IoC 的生活類比：公司的行政部門

| 情境 | 傳統寫法 | IoC |
| --- | --- | --- |
| 員工需要印表機 | 自己去買一台放桌上 | 跟行政部門說「我需要印表機」 |
| 公司要換品牌 | 每個人自己換 | 行政部門統一換 |
| 誰掌握採購權 | 每位員工 | **行政部門（IoC 容器）** |

<div class="mt-4 p-3 bg-blue-50 border-l-4 border-blue-400 text-gray-700 text-sm text-left">
💡 員工只要說「我需要什麼」，不用管東西從哪裡來、是哪個品牌。
</div>

<!--
我們用公司的行政部門來比喻。

傳統的做法是，每位員工需要印表機，就自己去買一台。公司要換品牌的時候，每個人都要自己換，非常混亂。

IoC 的做法是，公司有一個行政部門，員工只要說「我需要一台印表機」，行政部門就會給你一台。公司要換品牌，只要行政部門統一處理，員工完全不用管。

在 ASP.NET Core 裡，行政部門就是 DI 容器，員工就是我們的 Controller。
-->

---

# IoC 的好處

| 好處 | 說明 |
| --- | --- |
| **鬆耦合** | 使用者只依賴介面，不知道具體是哪個類別 |
| **集中管理** | 物件怎麼建立、用哪個實作，都在 `Program.cs` 一個地方設定 |
| **容易替換** | 換物流商只要改一行註冊 |
| **容易測試** | 測試時可以換成假的實作（Mock） |
| **生命週期管理** | 容器負責建立與釋放物件，例如請求結束後關閉 DbContext 連線 |

<!--
IoC 有五個好處。

鬆耦合、集中管理、容易替換、容易測試，這四個都是剛剛情境的解法。

第五個好處比較隱藏，但非常重要：生命週期管理。像 DbContext 這種會開啟資料庫連線的物件，用完一定要關閉。交給容器管理之後，容器會在請求結束時自動幫我們釋放，我們就不用擔心忘記關連線。這個在 6-4 會詳細介紹。
-->

---
layout: default
---

# 練習 2：IoC 觀念
### 認證模擬題（單選）

關於 IoC（控制反轉），下列敘述何者**正確**？

A. IoC 是 ASP.NET Core 提供的一個類別，名稱叫做 `IoC`
B. IoC 的意思是讓類別自己決定要建立哪些物件
C. IoC 是一種設計概念，把物件的建立與管理交給外部容器
D. 使用 IoC 之後，程式就不需要介面了

<!--
【出題動機】
IoC 是概念而不是 API，這是很常見的誤解。

【解題引導】
回想行政部門的比喻：「控制」是什麼？被「反轉」到哪裡去了？
-->

---
layout: default
---

# 練習 2：解析

**正確答案：C**

| 選項 | 解析 |
| --- | --- |
| A ❌ | IoC 是設計概念，不是某個類別或 API |
| B ❌ | 相反：控制權從類別自己「反轉」給外部 |
| C ✅ | 外部容器負責建立與管理物件 |
| D ❌ | 介面正是 IoC / DI 實現鬆耦合的關鍵 |

<!--
答案是 C。IoC 是一個設計概念，ASP.NET Core 的 DI 容器是實現這個概念的工具。
-->

---
layout: section
class: flex flex-col justify-center items-center text-center
---

# 6-3 DI 依賴注入
### Dependency Injection

<!--
第三個小節，我們來看 IoC 的具體實作方式：DI 依賴注入。
-->

---

# 什麼是 DI？

| 名詞 | 意義 |
| --- | --- |
| **Dependency（依賴）** | `CheckoutController` 需要運費服務才能工作 → 它**依賴**運費服務 |
| **Injection（注入）** | 容器把運費服務的物件**放進** `CheckoutController` |

「DI 就是 IoC 容器**主動把類別需要的物件，注入給它**。」

| IoC vs DI | 說明 |
| --- | --- |
| IoC | 設計概念：控制權交給外部 |
| DI | 實作方式：外部把依賴的物件注入進來 |

<!--
我們拆解 Dependency Injection 這個詞。

Dependency 是依賴：CheckoutController 要算運費，它需要運費服務，所以說 CheckoutController 依賴運費服務。Injection 是注入：容器把運費服務的物件放進 CheckoutController 裡面。

合起來，DI 就是容器主動把類別需要的物件注入給它。

IoC 和 DI 常常一起出現，它們的關係是：IoC 是概念，說的是「控制權交給外部」；DI 是實作方式，說的是「外部怎麼把東西給你」。
-->

---

# DI 的生活類比：打針

| 角色 | 打針 | DI |
| --- | --- | --- |
| 病人 | 需要藥物的你 | `CheckoutController` |
| 藥物 | 針筒裡的藥 | `BlackCatShipping` 物件 |
| 護理師 | 幫你打針的人 | **DI 容器** |
| 伸出手臂 | 告訴護理師「打這裡」 | **建構子參數** |

「你只需要伸出手臂，不用自己把藥打進去。」

<!--
用打針來理解 DI 非常直觀。

你生病了，需要藥物，但你不會自己幫自己打針。護理師會幫你把藥注射進去。你唯一要做的，就是伸出手臂，告訴護理師要打在哪裡。

在 ASP.NET Core 裡，Controller 就是病人，DI 容器就是護理師，而「伸出手臂」就是在建構子寫上參數。建構子參數就是在告訴容器：「我需要這個東西，請注入給我。」
-->

---
zoom: 0.95
---

# 在 ASP.NET Core 中練習 DI — Step 1：定義介面

```csharp
// Services/IShippingService.cs
namespace EShop.Web.Services;

public interface IShippingService
{
    string Name { get; }
    decimal Calculate(decimal orderTotal);
}
```

```csharp
// Services/BlackCatShipping.cs
public class BlackCatShipping : IShippingService
{
    public string Name => "黑貓宅急便";
    public decimal Calculate(decimal orderTotal) => orderTotal >= 1500 ? 0 : 150;
}

// Services/ConvenienceStoreShipping.cs
public class ConvenienceStoreShipping : IShippingService
{
    public string Name => "超商取貨";
    public decimal Calculate(decimal orderTotal) => 60;
}
```

<!--
我們開始把運費的例子改寫成 DI。

第一步，定義介面 IShippingService，這就是插座的規格：任何物流商都要有名稱，都要能計算運費。

接著讓黑貓和超商兩個類別都實作這個介面，冒號後面接介面名稱。兩家物流的計算方式不同，但對外的規格是一樣的。
-->

---

# Step 2：在 Program.cs 註冊服務

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(/* ... */);

// 告訴容器：有人要 IShippingService 時，給他 BlackCatShipping
builder.Services.AddScoped<IShippingService, BlackCatShipping>();

var app = builder.Build();
```

| 寫法 | 意義 |
| --- | --- |
| `AddScoped<介面, 實作>()` | 要介面時給實作 |
| `AddScoped<類別>()` | 直接註冊類別本身（沒有介面時） |

<!--
第二步，在 Program.cs 註冊服務。

AddScoped 的尖括號裡有兩個型別：第一個是介面，第二個是實作。意思是「當有人需要 IShippingService 的時候，請給他一個 BlackCatShipping」。

Scoped 是生命週期的一種，6-4 會詳細說明，現在先照著寫就好。

大家有沒有發現，這跟上一章的 AddDbContext 是一樣的概念？AddDbContext 其實就是用 Scoped 生命週期幫我們註冊 ApplicationDbContext。
-->

---

# Step 3：建構子注入

```csharp
public class CheckoutController(IShippingService shipping) : Controller
{
    public IActionResult Index(decimal total = 1000)
    {
        var fee = shipping.Calculate(total);
        return Content($"{shipping.Name}：訂單 {total} 元，運費 {fee} 元");
    }
}
```

瀏覽 `/Checkout?total=1000`，畫面會顯示：

```text
黑貓宅急便：訂單 1000 元，運費 150 元
```

<!--
第三步，在 Controller 的建構子寫上 IShippingService 參數，這就是「伸出手臂」。

注意 Controller 只知道 IShippingService 這個介面，完全不知道背後是黑貓還是超商。

當請求進來，框架要建立 CheckoutController 的時候，會發現建構子需要一個 IShippingService，就去 DI 容器查：「IShippingService 要給誰？」容器說要給 BlackCatShipping，於是建立一個 BlackCatShipping 傳進建構子。這就是建構子注入，也是 ASP.NET Core 最主流的注入方式。
-->

---

# 換物流商：只要改一行

```csharp
// Program.cs
builder.Services.AddScoped<IShippingService, ConvenienceStoreShipping>();
```

```text
超商取貨：訂單 1000 元，運費 60 元
```

| 需要修改的檔案 | 緊耦合 | DI |
| --- | --- | --- |
| `CheckoutController` | ✏️ 要改 | ✅ 不用改 |
| 其他用到運費的 Controller | ✏️ 每個都要改 | ✅ 不用改 |
| `Program.cs` | — | ✏️ 改一行 |

<!--
現在老闆說要換成超商取貨，我們只要改 Program.cs 的一行註冊，把 BlackCatShipping 換成 ConvenienceStoreShipping。

CheckoutController 一行都不用改，其他用到運費服務的地方也都不用改，重新執行，運費就變成 60 元了。

這就是 DI 的威力：「使用服務的程式碼」和「決定用哪個服務」被分開了。
-->

---

# 回到開頭的問題：db 是誰給的？

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options => ...);   // ① 註冊

public class CategoryController(ApplicationDbContext db) : Controller  // ② 建構子要求
```

| 步驟 | 發生的事 |
| --- | --- |
| ① | `AddDbContext` 把 `ApplicationDbContext` 註冊到 DI 容器（Scoped） |
| ② | 請求 `/Category` 時，框架建立 `CategoryController`，發現建構子需要 `ApplicationDbContext` |
| ③ | DI 容器建立 `ApplicationDbContext`（帶入連線設定）並注入 |
| ④ | 請求結束時，容器自動 `Dispose` 這個 DbContext，關閉資料庫連線 |

<!--
學到這裡，我們終於可以回答這一章開頭的問題了：CategoryController 的 db 是誰給的？

第一步，Program.cs 的 AddDbContext 把 ApplicationDbContext 註冊到 DI 容器。第二步，請求進來時，框架要建立 CategoryController，發現建構子需要 ApplicationDbContext。第三步，DI 容器建立一個 ApplicationDbContext，並帶入我們設定的連線字串，注入給 Controller。第四步，請求結束的時候，容器會自動釋放這個 DbContext，關閉資料庫連線。

整個過程我們完全不用自己 new，也不用自己關連線，這就是 IoC 和 DI 帶來的好處。
-->

---

# 其他注入方式

| 方式 | 寫法 | 使用時機 |
| --- | --- | --- |
| **建構子注入** | `class X(IService s)` | ✅ 主流，整個類別都會用到 |
| Action 參數注入 | `Index([FromServices] IService s)` | 只有某一個 Action 需要 |
| View 注入 | `@inject IService s` | View 需要服務（例如讀設定） |
| Keyed Services（.NET 8+） | `[FromKeyedServices("store")] IShippingService s` | 同一介面有多個實作要選擇 |

```csharp
builder.Services.AddKeyedScoped<IShippingService, BlackCatShipping>("home");
builder.Services.AddKeyedScoped<IShippingService, ConvenienceStoreShipping>("store");

public class StoreController([FromKeyedServices("store")] IShippingService shipping) : Controller { }
```

<!--
除了建構子注入，ASP.NET Core 還有幾種注入方式。

Action 參數注入是在參數前面加上 FromServices，只有那個 Action 會拿到服務。View 注入是在 Razor 裡寫 @inject，第九章會用到。

.NET 8 之後還新增了 Keyed Services：同一個介面可以註冊多個實作，每個實作給一個 key，注入時用 FromKeyedServices 指定要哪一個。例如宅配用黑貓、超商取貨用超商，兩個可以同時存在。

不過九成以上的情況，我們都用建構子注入。
-->

---

# 使用 DI 的注意事項

| 注意事項 | 說明 |
| --- | --- |
| **之一：忘記註冊** | 執行時拋出 `Unable to resolve service for type 'IShippingService'` |
| **之二：依賴介面，不要依賴實作** | 建構子參數寫 `IShippingService`，不要寫 `BlackCatShipping` |
| **之三：同一介面註冊多次，後者覆蓋** | 注入單一實例時拿到**最後一個**註冊的實作 |

<!--
使用 DI 有三個注意事項。

之一，最常見的錯誤就是忘記註冊，錯誤訊息是 Unable to resolve service，看到這個訊息就去 Program.cs 檢查有沒有 Add。

之二，建構子參數要寫介面，不要寫具體類別，否則就失去換實作的彈性了。

之三，同一個介面如果註冊了兩次，注入的時候會拿到最後註冊的那一個。如果真的需要多個實作，可以用剛剛的 Keyed Services。
-->

---
layout: default
---

# 練習 3：折扣服務
### 任務說明

1. 定義 `IDiscountService`，方法 `decimal Apply(decimal total)`
2. 實作 `NoDiscount`（不打折）與 `AnniversaryDiscount`（週年慶全館 85 折）
3. 在 `Program.cs` 註冊 `IDiscountService`
4. `CheckoutController` 同時注入 `IShippingService` 與 `IDiscountService`，顯示折扣後金額與運費
5. 只修改 `Program.cs`，切換兩種折扣方式並觀察結果

<!--
【練習目的】
練習定義介面、註冊服務、建構子注入多個服務。

【解題引導】
primary constructor 可以有多個參數，用逗號隔開即可。運費要用折扣後的金額計算。
-->

---
layout: default
---

# 練習 3：解題提示

```csharp
public interface IDiscountService { decimal Apply(decimal total); }
public class NoDiscount : IDiscountService { public decimal Apply(decimal total) => total; }
public class AnniversaryDiscount : IDiscountService { public decimal Apply(decimal total) => total * 0.85m; }

builder.Services.AddScoped<IDiscountService, AnniversaryDiscount>();

public class CheckoutController(IShippingService shipping, IDiscountService discount) : Controller
{
    public IActionResult Index(decimal total = 2000)
    {
        var final = discount.Apply(total);
        return Content($"折扣後 {final:N0} 元，運費 {shipping.Calculate(final)} 元");
    }
}
```

<!--
2000 元打 85 折是 1700 元，超過 1500 所以黑貓免運。如果把折扣換成 NoDiscount，一樣是 2000 元，也是免運；可以試試 total=1600，打折後變 1360，就要付運費了。
-->

---
layout: section
class: flex flex-col justify-center items-center text-center
---

# 6-4 服務的生命週期
### Transient, Scoped, Singleton

<!--
最後一個小節，我們來看註冊服務時的 AddScoped，到底是什麼意思。
-->

---

# 什麼是服務的生命週期？

「生命週期決定了 DI 容器**什麼時候建立新的物件**、**什麼時候重複使用**。」

| 生命週期 | 註冊方法 | 建立時機 | 咖啡店比喻 |
| --- | --- | --- | --- |
| **Transient** | `AddTransient` | **每次**注入都建立新的 | 紙杯：每次都用新的 |
| **Scoped** | `AddScoped` | **每個 HTTP 請求**建立一個 | 托盤：同一位客人這次點餐共用一個 |
| **Singleton** | `AddSingleton` | 整個應用程式**只建立一個** | 店門口的招牌：全店共用一個 |

<!--
註冊服務的時候，除了告訴容器要給哪個實作，還要告訴它：這個物件要用多久？

Transient 就像紙杯，每次有人要就給一個新的，用完就丟。Scoped 就像托盤，同一位客人這次點餐從頭到尾共用同一個托盤，下一位客人再換新的，在網站裡「一位客人這次點餐」就是一個 HTTP 請求。Singleton 就像店門口的招牌，整家店從開門到打烊只有一個，所有客人共用。
-->

---

# 在 ASP.NET Core 中觀察生命週期 — 準備

```csharp
public interface IOperation { Guid Id { get; } }

public class Operation : IOperation
{
    public Guid Id { get; } = Guid.NewGuid();   // 每個物件建立時產生一個新 Guid
}

public interface ITransientOp : IOperation;
public interface IScopedOp    : IOperation;
public interface ISingletonOp : IOperation;
public class TransientOp : Operation, ITransientOp;
public class ScopedOp    : Operation, IScopedOp;
public class SingletonOp : Operation, ISingletonOp;
```

```csharp
builder.Services.AddTransient<ITransientOp, TransientOp>();
builder.Services.AddScoped<IScopedOp, ScopedOp>();
builder.Services.AddSingleton<ISingletonOp, SingletonOp>();
```

<!--
我們用一個實驗來觀察三種生命週期的差別。

Operation 類別在建立的時候會產生一個 Guid，每個物件的 Guid 都不一樣。所以只要看 Guid 有沒有變，就知道是不是同一個物件。

我們定義三個介面，分別用三種生命週期註冊。
-->

---

# 觀察生命週期 — 同時注入兩次

```csharp
public class LifetimeController(
    ITransientOp t1, ITransientOp t2,
    IScopedOp s1, IScopedOp s2,
    ISingletonOp g1, ISingletonOp g2) : Controller
{
    public IActionResult Index() => Content($"""
        Transient : {t1.Id.ToString()[..8]} / {t2.Id.ToString()[..8]}
        Scoped    : {s1.Id.ToString()[..8]} / {s2.Id.ToString()[..8]}
        Singleton : {g1.Id.ToString()[..8]} / {g2.Id.ToString()[..8]}
        """);
}
```

<!--
Controller 裡，每種生命週期都注入兩次，然後把 Guid 的前 8 碼印出來比較。

這裡用了第二章學的原始字串，三個雙引號，前面加錢字號，就可以寫多行的字串插值。[..8] 是 C# 的範圍運算子，取前 8 個字元。
-->

---

# 觀察生命週期 — 執行結果

第一次請求：

```text
Transient : 3f2a91c0 / 8b7d1e44     ← 兩個不同
Scoped    : a1c5e9f2 / a1c5e9f2     ← 同一個請求內相同
Singleton : 77e0b3d1 / 77e0b3d1     ← 相同
```

重新整理（第二次請求）：

```text
Transient : 5c9e02aa / e41f7b93     ← 又是新的
Scoped    : 0d3b6c18 / 0d3b6c18     ← 換了新的，但請求內仍相同
Singleton : 77e0b3d1 / 77e0b3d1     ← 永遠是同一個
```

<!--
我們來看執行結果。

Transient 注入兩次，兩個 Guid 都不一樣，因為每次注入都建立新的物件。Scoped 在同一個請求裡兩個 Guid 相同，但重新整理之後，換成另一組新的 Guid。Singleton 不管重新整理幾次，永遠都是同一個 Guid，直到網站重新啟動。

這個實驗把三種生命週期的差別看得非常清楚。
-->

---

# 生命週期怎麼選？

| 生命週期 | 適合的服務 | EShop 範例 |
| --- | --- | --- |
| **Transient** | 輕量、無狀態的服務 | 運費計算、格式轉換 |
| **Scoped** | 同一個請求需要共用狀態 | **`DbContext`**、Repository、UnitOfWork（第 7 章） |
| **Singleton** | 全域共用、執行緒安全、建立成本高 | 快取、設定、`HttpClient` 工廠 |

<div class="mt-4 p-3 bg-blue-50 border-l-4 border-blue-400 text-gray-700 text-sm text-left">
💡 <b>DbContext 為什麼是 Scoped？</b>同一個請求中，Controller 與 Repository 要共用同一個 DbContext，才能在同一次 <code>SaveChanges</code> 一起寫入；請求結束就釋放連線。
</div>

<!--
那要怎麼選擇生命週期？

Transient 適合輕量、沒有狀態的服務。Scoped 適合在同一個請求中需要共用狀態的服務，最典型的就是 DbContext。Singleton 適合全域共用的東西，例如快取，但它必須是執行緒安全的，因為所有請求會同時使用它。

特別說明一下 DbContext 為什麼是 Scoped：同一個請求裡，可能有好幾個類別都需要存取資料庫，它們必須共用同一個 DbContext，最後呼叫一次 SaveChanges 才能一起寫入。第七章的 UnitOfWork 就是利用這個特性。請求結束之後，DbContext 被釋放，資料庫連線也跟著關閉。
-->

---

# 使用生命週期的注意事項

| 注意事項 | 說明 |
| --- | --- |
| **之一：Singleton 不能注入 Scoped** | 長壽的物件抓住短命的物件（Captive Dependency），開發環境會直接拋例外 |
| **之二：Singleton 要執行緒安全** | 所有請求同時存取，不要在裡面存放個別使用者的資料 |
| **之三：DbContext 不是執行緒安全的** | 同一個 DbContext 不要平行執行多個查詢 |

```text
InvalidOperationException: Cannot consume scoped service 'ApplicationDbContext'
from singleton 'ProductCache'.
```

<!--
生命週期有三個注意事項。

之一，Singleton 不能注入 Scoped 服務。想想看，Singleton 活到網站關閉，如果它抓住了一個 Scoped 的 DbContext，那個 DbContext 就永遠不會被釋放，而且會被所有請求共用，資料就會亂掉。這個問題叫 Captive Dependency，ASP.NET Core 在開發環境會直接拋出例外提醒我們。

之二，Singleton 會被所有請求同時使用，所以不能放個別使用者的資料，否則 A 使用者可能看到 B 使用者的資料。

之三，DbContext 不是執行緒安全的，同一個 DbContext 不要同時跑多個查詢，要一個 await 完再跑下一個。
-->

---
layout: default
---

# 練習 4：生命週期判斷
### 認證模擬題（單選）

EShop 要做一個「瀏覽人次計數器」，所有使用者共用同一個計數，應該用哪個生命週期註冊？

A. `AddTransient`，因為每次都要建立新的計數器
B. `AddScoped`，因為每個請求都要計數
C. `AddSingleton`，因為整個網站只需要一個共用的計數器（並注意執行緒安全）
D. 不需要註冊，直接在 Controller 用 `static` 變數

<!--
【出題動機】
用實際情境檢驗生命週期的選擇。

【解題引導】
如果用 Transient 或 Scoped，每次請求都會拿到一個新的計數器，數字會怎樣？
-->

---
layout: default
---

# 練習 4：解析

**正確答案：C**

```csharp
public class VisitCounter
{
    private int _count;
    public int Increment() => Interlocked.Increment(ref _count);   // 執行緒安全的 +1
}

builder.Services.AddSingleton<VisitCounter>();
```

| 選項 | 解析 |
| --- | --- |
| A / B ❌ | 每次（每個請求）都是新物件，計數永遠從 0 開始 |
| C ✅ | 全站共用；用 `Interlocked` 確保多人同時 +1 不會算錯 |
| D ❌ | 可以動，但失去 DI 的可測試性與集中管理 |

<!--
答案是 C。Transient 和 Scoped 每次都是新物件，計數永遠是 1。

注意 Increment 用了 Interlocked.Increment，因為 Singleton 會被很多請求同時呼叫，如果只寫 _count++，兩個請求同時執行時可能會少算。這就是剛剛說的「Singleton 要執行緒安全」。
-->

---
layout: default
---

# 綜合練習：可切換的通知服務
### 任務說明

1. 定義 `INotifier`，方法 `Task SendAsync(string to, string message)`
2. 實作 `ConsoleNotifier`（印在 console）與 `FileNotifier`（附加寫入 `notify.log`）
3. 用 **Keyed Services** 同時註冊兩者，key 分別為 `"console"`、`"file"`
4. 建立 `OrderNotifyController`，注入兩個 notifier，下單時兩邊都通知
5. 加入 `VisitCounter`（Singleton），在回應中顯示「這是第 N 次下單」
6. 說明每個服務選擇該生命週期的理由

<!--
【練習目的】
整合介面、註冊、建構子注入、Keyed Services 與生命週期的選擇。

【解題引導】
FileNotifier 可以用 File.AppendAllTextAsync。思考一下兩個 notifier 適合用 Transient 還是 Scoped？
-->

---
layout: default
---

# 綜合練習：解題提示

```csharp
builder.Services.AddKeyedTransient<INotifier, ConsoleNotifier>("console");
builder.Services.AddKeyedTransient<INotifier, FileNotifier>("file");
builder.Services.AddSingleton<VisitCounter>();

public class OrderNotifyController(
    [FromKeyedServices("console")] INotifier console,
    [FromKeyedServices("file")] INotifier file,
    VisitCounter counter) : Controller
{
    public async Task<IActionResult> Create(string customer = "小明")
    {
        var n = counter.Increment();
        await console.SendAsync(customer, $"訂單成立（第 {n} 次下單）");
        await file.SendAsync(customer, $"訂單成立（第 {n} 次下單）");
        return Content($"已通知，這是第 {n} 次下單");
    }
}
```

<!--
兩個 notifier 沒有狀態，用 Transient 就好；計數器要全站共用，用 Singleton。

大家可以試試看把 VisitCounter 改成 AddScoped，會發現每次都是第 1 次，這樣就能親身體會生命週期的差別。
-->

---

# 總結

| 小節 | 重點 |
| --- | --- |
| 6-1 介紹 | 類別自己 `new` 具體類別 → 緊耦合，難以替換與測試 |
| 6-2 IoC | 設計概念：「把物件的控制權交給外部容器管理」 |
| 6-3 DI | 實作方式：介面 + `builder.Services.AddXxx<介面, 實作>()` + 建構子注入 |
| 6-4 生命週期 | Transient 每次新建、**Scoped 每個請求一個（DbContext）**、Singleton 全站一個 |

下一章我們會介紹 **系統架構與分層**，用 DI 把資料存取抽成 Repository 與 UnitOfWork。

<!--
我們來總結這一章。

類別自己 new 具體類別會造成緊耦合。IoC 是把控制權交給外部容器的概念，DI 則是實作方式：定義介面、在 Program.cs 註冊、在建構子要求注入。註冊時要選擇生命週期，DbContext 是 Scoped，每個請求一個。

現在大家應該能完全看懂 CategoryController 的建構子了。

不過目前 Controller 還是直接使用 DbContext，資料存取的程式碼都寫在 Controller 裡。下一章我們會介紹分層架構，用這一章學的 DI，把資料存取抽成 Repository 和 UnitOfWork，讓 Controller 變得更乾淨。
-->

---
layout: end
---

# 第 6 章結束
### 下一章：系統架構與分層
