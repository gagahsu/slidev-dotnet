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
title: C# 基礎語法
routeAlias: ch02
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
  <h1 style="color: #1a5c5c; font-size: 3.4rem; font-weight: 900; line-height: 1.15; margin-bottom: 1.5rem;">C# 基礎語法</h1>
  <div style="height: 4px; width: 320px; background: linear-gradient(90deg, #5eada0, #a7d9d0); border-radius: 2px; margin-bottom: 1.5rem;"></div>
  <p style="color: #4a7c7c; font-size: 1.15rem; font-style: italic;">「先學會說 C#，才能跟 ASP.NET Core 溝通」</p>
  <Link to="home" style="color: #9dc4c4; font-size: 0.85rem; margin-top: 2rem; text-decoration: none; letter-spacing: 0.05em;">← 返回目錄</Link>
</div>

<!--
大家好，歡迎來到第二章！

上一章我們把開發環境架好，也跑起了第一個 MVC 網站。但大家打開 Controller 的時候，可能會看到 class、public、async、=> 這些符號，不太確定它們在做什麼。

這一章我們就來把 C# 的基本功打好。我們會從程式架構開始，接著學變數與型別、條件判斷、迴圈，最後學最重要的類別與物件。這一章的範例全部使用 .NET 10 的 dotnet run app.cs，一個檔案就能執行，讓我們專心在語法本身。
-->

---
layout: default
---

# Outline

- **回顧：Program.cs 的兩個階段**
- **2-1 程式架構** — namespace、using、top-level statements
- **2-2 程式語法介紹** — 變數、型別、字串、運算子、nullable
- **2-3 條件流程控制** — if、switch expression、pattern matching
- **2-4 迴圈流程控制** — for、foreach、while、collection expressions
- **2-5 類別與物件** — 屬性、建構子、primary constructor、record
- **EShop 專案實作** — 第 2 步：商品模型與會員折扣
- **總結**

<!--
這一章分成五個小節，最後一節類別與物件是重點，因為 ASP.NET Core 裡的 Controller、Model、Service，全部都是類別。

如果大家已經學過 Java，會發現 C# 跟 Java 非常像，可以用比較快的速度看過，把注意力放在 C# 特有的語法上，例如屬性、var、string interpolation、switch expression 和 record。

最後的 EShop 專案實作，我們會用這一章的類別、record 和 switch expression 定義商品，並寫下第一個單元測試。
-->

---

# 回顧：Program.cs 的兩個階段

| 階段 | 寫法 | 做什麼 |
| --- | --- | --- |
| 服務註冊 | `builder.Services.AddControllersWithViews();` | 告訴框架要用哪些功能 |
| Pipeline 設定 | `app.UseRouting();`、`app.MapControllerRoute(...)` | 決定請求經過哪些 Middleware |
| 啟動 | `app.Run();` | 開始接收請求 |

「每個請求都會依照順序經過 Middleware，最後抵達 Controller。」

<!--
我們先快速回顧上一章。

Program.cs 分成兩個階段：Build 之前註冊服務，Build 之後設定 Middleware Pipeline，最後 app.Run 啟動網站。每個請求都會像剝洋蔥一樣經過 Middleware，最後抵達 Controller。

大家有沒有注意到，Program.cs 裡面沒有 class，也沒有 Main 方法，程式就直接開始寫了？這個就是 C# 的 top-level statements，我們這一章第一個小節就從這裡開始。
-->

---
layout: section
class: flex flex-col justify-center items-center text-center
---

# 2-1 程式架構
### Program Structure

<!--
第一個小節，我們來看一支 C# 程式長什麼樣子。
-->

---

# 什麼是 C# 程式的基本架構？

傳統的 C# 程式需要 namespace、class、Main 方法三層包裝：

```csharp
using System;

namespace HelloApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, C#!");
        }
    }
}
```

<!--
我們先看傳統的 C# 程式長什麼樣子。

最外層是 using，引入要用的命名空間；接著是 namespace，像是資料夾，用來分類程式碼；再來是 class；最裡面才是 Main 方法，程式從這裡開始執行。

為了印出一行 Hello，要寫十幾行的包裝，對初學者來說有點辛苦。
-->

---

# 現代寫法：Top-level Statements

從 C# 9 開始，程式可以直接從第一行開始寫：

```csharp
// hello.cs
Console.WriteLine("Hello, C#!");
```

```bash
dotnet run hello.cs
```

「Top-level statements 就是讓編譯器幫我們自動產生 `Program` 類別和 `Main` 方法。」

<div class="mt-4 p-3 bg-blue-50 border-l-4 border-blue-400 text-gray-700 text-sm text-left">
💡 ASP.NET Core 的 <code>Program.cs</code> 就是用 top-level statements 寫的，所以裡面看不到 <code>class Program</code> 和 <code>Main</code>。
</div>

<!--
現代的 C# 寫法就簡單多了，一行就好。

這個叫做 top-level statements，意思是「編譯器會自動幫我們把這些程式碼包進 Program 類別的 Main 方法裡」。所以不是沒有 Main，而是編譯器幫我們寫了。

這也解釋了上一章的疑問：Program.cs 裡面沒有 class 和 Main，就是因為它用了 top-level statements。
-->

---

# namespace 與 using

| 語法 | 用途 | 比喻 |
| --- | --- | --- |
| `namespace EShop.Models;` | 宣告這個檔案的類別屬於哪個命名空間 | 地址的「縣市區」 |
| `using EShop.Models;` | 引入其他命名空間，才能直接使用裡面的類別 | 通訊錄 |
| `global using` | 整個專案都自動 using | 公司共用通訊錄 |

```csharp
namespace EShop.Models;   // file-scoped namespace（C# 10+），不用多一層大括號

public class Category { }
```

<!--
namespace 就像地址。台北市有中正路，台中市也有中正路，要加上縣市才不會搞混。同樣的道理，兩個不同的套件都可能有叫 Category 的類別，namespace 可以區分它們。

using 則是像通訊錄，把要用的 namespace 先加進來，之後就可以直接寫類別名稱。

注意 namespace 後面直接加分號的寫法叫 file-scoped namespace，是 C# 10 之後的主流寫法，可以少一層縮排。這門課的所有類別都會用這種寫法。

上一章看到的 ImplicitUsings，就是 SDK 自動幫我們加上 global using System、System.Linq 這些常用的命名空間。
-->

---

# C# 的命名慣例

| 對象 | 慣例 | 範例 |
| --- | --- | --- |
| 類別、方法、屬性 | **PascalCase** | `ProductController`、`GetAll()`、`Price` |
| 區域變數、參數 | **camelCase** | `productName`、`orderTotal` |
| 私有欄位 | **_camelCase** | `_db`、`_unitOfWork` |
| 介面 | **I** + PascalCase | `IRepository`、`IEmailSender` |
| 常數 | PascalCase | `MaxQuantity` |

<div class="mt-4 p-3 bg-blue-50 border-l-4 border-blue-400 text-gray-700 text-sm text-left">
💡 跟 Java 最大的差別：C# 的<b>方法名稱用大寫開頭</b>，例如 <code>ToString()</code>、<code>GetAll()</code>。
</div>

<!--
命名慣例雖然不影響程式能不能執行，但它會影響團隊合作，所以我們一開始就養成好習慣。

C# 的類別、方法、屬性都用大寫開頭的 PascalCase；區域變數和參數用小寫開頭的 camelCase；私有欄位習慣加底線，例如 _db。介面一定用大寫 I 開頭，後面第六章學依賴注入時會大量看到。

學過 Java 的同學要特別注意：C# 的方法名稱是大寫開頭，這是兩個語言最明顯的差別。
-->

---
layout: default
---

# 練習 1：程式架構
### 認證模擬題（單選）

關於下列程式碼，哪一個描述是**正確**的？

```csharp
namespace EShop.Models;

public class Product { }
```

A. 這段程式碼無法編譯，因為 namespace 後面必須接大括號
B. 這是 file-scoped namespace 寫法，`Product` 屬於 `EShop.Models` 命名空間
C. 其他檔案要使用 `Product`，一定要寫 `EShop.Models.Product`，無法簡化
D. `Product` 應該命名為 `product`，因為類別名稱要用 camelCase

<!--
【出題動機】
確認大家看得懂現代 C# 的 file-scoped namespace，以及 using 和命名慣例。

【解題引導】
C# 10 之後 namespace 可以怎麼寫？其他檔案可以用哪個關鍵字簡化類別名稱？類別名稱的慣例是 PascalCase 還是 camelCase？
-->

---
layout: default
---

# 練習 1：解析

**正確答案：B**

| 選項 | 解析 |
| --- | --- |
| A ❌ | C# 10 起支援 `namespace X;` 的 file-scoped 寫法 |
| B ✅ | 整個檔案的型別都屬於 `EShop.Models` |
| C ❌ | 只要加上 `using EShop.Models;` 就可以直接寫 `Product` |
| D ❌ | 類別名稱使用 PascalCase |

<!--
答案是 B。file-scoped namespace 是現在的主流寫法，搭配 using 就能在其他檔案直接使用類別名稱。
-->

---
layout: section
class: flex flex-col justify-center items-center text-center
---

# 2-2 程式語法介紹
### Variables, Types & Operators

<!--
第二個小節，我們來學 C# 最基本的語法：變數、型別、字串和運算子。
-->

---

# 什麼是變數與型別？

想像我們在整理咖啡豆倉庫，每個箱子都要貼標籤，標明「裡面只能放什麼」：

| 型別 | 用途 | 範例 |
| --- | --- | --- |
| `int` | 整數 | `int stock = 50;` |
| `decimal` | 金額（精準的小數） | `decimal price = 350.5m;` |
| `double` | 科學計算的小數 | `double rate = 0.15;` |
| `bool` | 真假值 | `bool isActive = true;` |
| `string` | 文字 | `string name = "耶加雪菲";` |
| `DateTime` | 日期時間 | `DateTime now = DateTime.Now;` |

<!--
變數就像倉庫裡的箱子，型別就是箱子上的標籤，規定這個箱子只能放什麼東西。

C# 是強型別語言，一旦宣告為 int，就不能放文字進去，編譯器會直接報錯。這看起來很嚴格，但好處是很多錯誤在寫程式的時候就會被抓出來，而不是等到上線才爆炸。

特別注意金額要用 decimal，而且數字後面要加 m。double 在計算時會有浮點誤差，例如 0.1 加 0.2 不會剛好等於 0.3，用在金額上是很危險的。我們的 EShop 商品價格都會用 decimal。
-->

---

# 在 C# 中練習宣告變數

```csharp
// variables.cs
string productName = "衣索比亞 耶加雪菲";
decimal price = 450m;
int quantity = 3;
var total = price * quantity;     // var：由編譯器推斷型別（decimal）
const int MaxQuantity = 10;       // const：常數，之後不能修改

Console.WriteLine($"{productName} x {quantity} = {total:N0} 元");
Console.WriteLine($"每次最多購買 {MaxQuantity} 包");
```

執行後，console 會輸出：

```text
衣索比亞 耶加雪菲 x 3 = 1,350 元
每次最多購買 10 包
```

<!--
這段程式碼示範了三種宣告方式。

一般宣告是「型別 變數名 等於 值」。var 是讓編譯器根據右邊的值自動推斷型別，這裡 price 乘 quantity 是 decimal，所以 total 就是 decimal。注意 var 不是弱型別，推斷完之後型別就固定了。const 是常數，宣告之後就不能再改。

字串前面的錢字號叫 string interpolation，可以在大括號裡直接放變數，冒號 N0 是格式化成千分位、不含小數。

實務上，右邊型別很明顯時用 var，例如 new 物件的時候；型別不明顯時就寫清楚，讓程式更好讀。
-->

---

# 字串的常用寫法

| 寫法 | 範例 | 說明 |
| --- | --- | --- |
| 字串插值 | `$"總計 {total:C}"` | 最常用，大括號內放變數與格式 |
| 逐字字串 | `@"C:\images\coffee.jpg"` | 反斜線不用跳脫 |
| 原始字串 | `"""..."""` | 多行文字、JSON（C# 11+） |
| 比較 | `name.Equals("Latte", StringComparison.OrdinalIgnoreCase)` | 忽略大小寫比較 |
| 判斷空值 | `string.IsNullOrWhiteSpace(name)` | 表單驗證很常用 |

---

# 字串 — 範例

```csharp
// strings.cs
var name = "  Latte  ";
Console.WriteLine($"[{name.Trim()}]");                   // [Latte]
Console.WriteLine(name.Trim().ToUpper());                 // LATTE
Console.WriteLine(string.IsNullOrWhiteSpace("   "));      // True

var json = """
    { "name": "Latte", "price": 120 }
    """;
Console.WriteLine(json);
```

<!--
字串是網站開發最常處理的資料，這裡整理幾個最常用的寫法。

字串插值前面加錢字號，是我們最常用的寫法。逐字字串前面加小老鼠，反斜線就不用跳脫，適合寫檔案路徑。三個雙引號的原始字串是 C# 11 的新語法，可以直接寫多行文字或 JSON，不用處理跳脫字元。

string.IsNullOrWhiteSpace 會同時檢查 null、空字串和只有空白的字串，之後做表單驗證的時候非常好用。
-->

---

# 運算子

| 類型 | 運算子 | 範例 |
| --- | --- | --- |
| 算術 | `+` `-` `*` `/` `%` | `7 / 2` 結果是 `3`（整數除法） |
| 比較 | `==` `!=` `>` `<` `>=` `<=` | `price >= 500` |
| 邏輯 | `&&` `\|\|` `!` | `isMember && total > 1000` |
| 指派 | `=` `+=` `-=` `++` `--` | `stock -= quantity;` |
| 三元 | `條件 ? A : B` | `isMember ? 0.9m : 1m` |

```csharp
decimal total = 1200m;
bool isMember = true;
var discount = isMember && total > 1000 ? 0.9m : 1m;
Console.WriteLine($"應付金額：{total * discount}");   // 應付金額：1080.0
```

<!--
運算子跟大部分程式語言都一樣，這裡提醒兩個容易出錯的地方。

第一個是整數除法：7 除以 2 在 C# 裡面等於 3，小數會被捨去，因為兩邊都是 int。如果要得到 3.5，其中一邊要是小數型別。

第二個是三元運算子，問號前面是條件，成立就回傳冒號左邊，不成立就回傳右邊，很適合用在簡單的二選一，例如會員打九折。
-->

---

# 什麼是 Nullable？

想像一張會員資料表，「生日」欄位可以不填。那沒填的時候要存什麼？

| 寫法 | 意義 |
| --- | --- |
| `int age` | 一定有值，預設為 `0` |
| `int? age` | 可以是整數，也可以是 `null`（沒有值） |
| `string name` | 開啟 Nullable 後，編譯器認為它**不該是 null** |
| `string? nickname` | 明確表示「可能是 null」 |

「Nullable 讓我們在型別上就標明『這個值可能不存在』，讓編譯器幫忙檢查 null 錯誤。」

<!--
我們先想一個情境：會員資料的生日可以不填，那沒填的時候，資料庫存的是 null。但 int 一定要有值，預設是 0，總不能說這個人 0 歲吧？

這時候就需要 Nullable，在型別後面加一個問號，int? 就代表「可以是整數，也可以是 null」。

上一章的 csproj 開啟了 Nullable 設定，意思是連 string 這種參考型別也會被檢查：沒加問號的 string，編譯器認為它不應該是 null；加了問號的 string?，代表可能是 null。這可以幫我們在寫程式時就避免掉 NullReferenceException 這個最常見的錯誤。
-->

---

# 在 C# 中練習處理 null

| 運算子 | 名稱 | 意義 |
| --- | --- | --- |
| `a ?? b` | null 聯合 | a 是 null 就用 b |
| `a ??= b` | null 聯合指派 | a 是 null 才指派 b |
| `a?.B` | null 條件 | a 是 null 就回傳 null，不會拋例外 |
| `a?.B = x` | null 條件指派（C# 14） | a 不是 null 才指派 |

```csharp
string? nickname = null;
Console.WriteLine(nickname ?? "匿名顧客");       // 匿名顧客
Console.WriteLine(nickname?.Length ?? 0);        // 0
```

<!--
處理 null 有四個好用的運算子。

兩個問號是 null 聯合運算子，左邊是 null 就用右邊的值，很適合設定預設值。問號點是 null 條件運算子，物件是 null 的時候不會拋出例外，而是直接回傳 null。

C# 14 還新增了 null 條件指派，可以寫 customer?.Name = "小明"，customer 不是 null 的時候才會指派，以前要多寫一個 if 判斷。

這幾個運算子在 Controller 和 View 裡非常常見，大家看到的時候要認得出來。
-->

---

# 使用變數的注意事項

| 注意事項 | 說明 |
| --- | --- |
| **之一：金額用 decimal** | `double` 有浮點誤差：`0.1 + 0.2 == 0.3` 是 `false` |
| **之二：型別轉換要小心** | 字串轉數字用 `int.TryParse`，不要直接 `int.Parse` |
| **之三：var 不是弱型別** | 推斷後型別就固定，不能再放入其他型別 |

```csharp
if (int.TryParse("12a", out var qty))
    Console.WriteLine($"數量：{qty}");
else
    Console.WriteLine("請輸入正確的數字");      // 會印出這一行
```

<!--
最後整理三個使用變數的注意事項。

之一，金額一定用 decimal。

之二，字串轉數字時，使用者輸入的東西不一定是數字，如果用 int.Parse 轉換失敗會直接拋例外，讓整個請求失敗。用 TryParse 會回傳 true 或 false，轉換成功的值透過 out 參數接住，比較安全。

之三，var 只是讓編譯器幫我們推斷型別，推斷完就固定了，它還是強型別。
-->

---
layout: default
---

# 練習 2：計算購物金額
### 任務說明

建立 `order.cs`，完成以下需求：

1. 宣告商品名稱、單價（decimal）、數量（int）
2. 宣告一個可能為 null 的優惠碼 `string? coupon`
3. 若 `coupon` 是 null，印出「未使用優惠碼」，否則印出優惠碼內容
4. 計算總金額並以千分位格式印出

<!--
【練習目的】
綜合練習變數宣告、decimal、nullable 和字串插值。

【操作提示】
用 dotnet run order.cs 執行。試著把 coupon 分別設成 null 和 "SUMMER10"，觀察輸出差異。
-->

---
layout: default
---

# 練習 2：解題提示

```csharp
// order.cs
var name = "哥倫比亞 薇拉";
decimal price = 380m;
int quantity = 2;
string? coupon = null;

Console.WriteLine(coupon ?? "未使用優惠碼");
Console.WriteLine($"{name} x {quantity}，總計 {price * quantity:N0} 元");
```

<!--
用 ?? 運算子，一行就能處理 null 的情況。金額計算直接寫在字串插值的大括號裡，冒號 N0 格式化成千分位。
-->

---
layout: section
class: flex flex-col justify-center items-center text-center
---

# 2-3 條件流程控制
### if / switch / Pattern Matching

<!--
第三個小節，我們來學條件判斷，讓程式可以根據不同的情況做不同的事。
-->

---

# 什麼是條件流程控制？

想像咖啡店的會員制度：

| 累積消費 | 會員等級 | 折扣 |
| --- | --- | --- |
| 10,000 元以上 | 金卡 | 8 折 |
| 5,000 元以上 | 銀卡 | 9 折 |
| 其他 | 一般 | 不打折 |

「條件流程控制，就是讓程式根據條件成立與否，選擇要執行哪一段程式碼。」

<!--
我們用咖啡店的會員等級來當例子：消費越多，等級越高，折扣越多。

這種「如果……就……否則……」的邏輯，在程式裡就是條件流程控制。C# 提供 if 和 switch 兩種寫法，我們一個一個來看。
-->

---

# 在 C# 中練習 if / else if / else

```csharp
// member.cs
decimal totalSpent = 6800m;
string level;

if (totalSpent >= 10000)
    level = "金卡";
else if (totalSpent >= 5000)
    level = "銀卡";
else
    level = "一般";

Console.WriteLine($"累積消費 {totalSpent:N0} 元，會員等級：{level}");
```

執行後，console 會輸出：

```text
累積消費 6,800 元，會員等級：銀卡
```

<!--
if 的寫法很直觀：從上到下判斷條件，第一個成立的就執行，後面的就不再判斷了。

所以條件的順序很重要，要從最嚴格的條件開始寫。如果把大於等於 5000 寫在最前面，那消費一萬的人也會被判成銀卡。

只有一行程式碼的時候可以省略大括號，但如果有多行，一定要加大括號。很多團隊的規範是就算只有一行也要加，避免之後加程式碼時出錯。
-->

---

# 現代寫法：switch expression

C# 8 之後，多選一的判斷可以用 **switch expression**，搭配 pattern matching 更精簡：

```csharp
decimal totalSpent = 6800m;

var level = totalSpent switch
{
    >= 10000 => "金卡",
    >= 5000  => "銀卡",
    _        => "一般"      // _ 代表「其他所有情況」
};

Console.WriteLine(level);  // 銀卡
```

<div class="mt-4 p-3 bg-blue-50 border-l-4 border-blue-400 text-gray-700 text-sm text-left">
💡 switch expression 會<b>回傳一個值</b>，適合「根據條件算出結果」的情境；if 適合「根據條件執行不同動作」。
</div>

<!--
同樣的邏輯，用 switch expression 可以寫得更精簡。

注意它的結構：變數放在 switch 前面，每一行是「條件 箭頭 結果」，最後的底線代表其他所有情況，類似 else。整個 switch 會回傳一個值，直接指派給 level。

這是現代 C# 非常主流的寫法，第十章做訂單狀態的時候，我們就會用 switch expression 把狀態轉成中文顯示。
-->

---
zoom: 0.95
---

# Pattern Matching 常見模式

| 模式 | 範例 | 意義 |
| --- | --- | --- |
| 常數 | `"Latte" => 120` | 等於某個值 |
| 關係 | `>= 5000 => "銀卡"` | 大於、小於比較 |
| 邏輯 | `>= 18 and < 65` | `and`、`or`、`not` 組合 |
| 型別 | `obj is string s` | 判斷型別並轉型 |
| null | `x is null`、`x is not null` | 判斷 null 的推薦寫法 |

```csharp
string drink = "Latte";
var price = drink switch
{
    "Americano" or "Espresso" => 90,
    "Latte" => 120,
    _ => 0
};
```

<!--
Pattern matching 是 C# 近幾年很大的進步，讓條件判斷寫起來更像自然語言。

常數模式就是比對某個值；關係模式可以做大小比較；邏輯模式可以用 and、or、not 組合條件，例如美式或濃縮咖啡都是 90 元。

特別推薦 is null 和 is not null 這兩個寫法，它比雙等號判斷 null 更精確，也更好讀，現在的官方範例都是這樣寫。
-->

---

# 使用條件判斷的注意事項

| 注意事項 | 說明 |
| --- | --- |
| **之一：條件由嚴到寬** | `>= 10000` 要寫在 `>= 5000` 前面 |
| **之二：switch expression 要涵蓋所有情況** | 少了 `_` 時編譯器會警告，執行時遇到沒涵蓋的值會拋例外 |
| **之三：判斷 null 用 `is null`** | 比 `== null` 更精確，不受運算子多載影響 |

<!--
條件判斷的注意事項有三個。

之一，條件要由嚴到寬排列。之二，switch expression 如果沒有涵蓋所有可能，編譯器會警告，執行時遇到沒處理的值會拋出例外，所以最後一行通常會加上底線。之三，判斷 null 建議用 is null。
-->

---
layout: default
---

# 練習 3：訂單運費計算
### 任務說明

EShop 的運費規則如下，請用 **switch expression** 實作：

| 條件 | 運費 |
| --- | --- |
| 訂單金額 ≥ 1,500 元 | 免運 |
| 訂單金額 ≥ 800 元 | 60 元 |
| 其他 | 120 元 |

另外，若配送方式 `delivery` 為 `"自取"`，一律免運。

<!--
【練習目的】
練習 switch expression 與 pattern matching，並思考條件的優先順序。

【解題引導】
自取的條件要放在金額判斷的前面還是後面？switch expression 可以同時判斷兩個變數嗎？可以試試 tuple pattern。
-->

---
layout: default
---

# 練習 3：解題提示

```csharp
decimal amount = 900m;
string delivery = "宅配";

var shippingFee = (delivery, amount) switch
{
    ("自取", _)     => 0m,
    (_, >= 1500)   => 0m,
    (_, >= 800)    => 60m,
    _              => 120m
};
Console.WriteLine($"運費：{shippingFee} 元");   // 運費：60 元
```

<!--
這裡用到了 tuple pattern：把兩個變數用小括號包起來一起判斷，底線代表「這個位置不管是什麼都可以」。

自取的規則優先度最高，所以放在第一行。這種寫法比巢狀的 if 清楚很多。
-->

---
layout: section
class: flex flex-col justify-center items-center text-center
---

# 2-4 迴圈流程控制
### for / foreach / while

<!--
第四個小節是迴圈。網站開發裡，把商品清單、購物車品項一筆一筆顯示出來，全部都要靠迴圈。
-->

---

# 什麼是迴圈？

想像店員要幫購物車裡的每一個品項結帳：

| 迴圈 | 適用情境 |
| --- | --- |
| `for` | 知道要跑幾次，需要索引（第幾筆） |
| `foreach` | 走訪集合中的每一個元素（最常用） |
| `while` | 條件成立就一直做，不確定次數 |
| `do...while` | 至少執行一次再判斷 |

「迴圈就是讓同一段程式碼重複執行，直到條件不成立為止。」

<!--
想像店員要結帳，購物車裡有五個品項，他就要重複「掃條碼、加總金額」五次。這種重複的動作，在程式裡就是迴圈。

C# 有四種迴圈，其中 foreach 是我們在 ASP.NET Core 裡最常用的，View 裡面顯示商品清單幾乎都是用 foreach。
-->

---

# 集合的現代寫法：Collection Expressions

在學迴圈之前，先認識 C# 12 的集合寫法：

```csharp
int[] prices = [120, 90, 150];                       // 陣列
List<string> drinks = ["Latte", "Mocha", "Americano"];  // List
List<string> empty = [];                             // 空集合
int[] all = [..prices, 200];                         // 展開：120, 90, 150, 200
```

| 型別 | 特性 |
| --- | --- |
| 陣列 `T[]` | 長度固定 |
| `List<T>` | 長度可變，可以 `Add`、`Remove`（最常用） |

<!--
迴圈最常用來走訪集合，所以我們先認識一下 C# 的集合。

陣列長度固定；List 可以動態增減元素，是最常用的集合。尖括號裡的 T 代表這個集合要放什麼型別，這個叫泛型，第七章會大量用到。

C# 12 開始，建立集合可以直接用中括號，這叫 collection expressions，不管是陣列還是 List 都可以用同一種寫法，兩個點點則是把另一個集合展開放進來。這是目前的主流寫法。
-->

---

# 在 C# 中練習 for 與 foreach

```csharp
// loops.cs
List<string> drinks = ["Latte", "Mocha", "Americano"];

for (int i = 0; i < drinks.Count; i++)
{
    Console.WriteLine($"{i + 1}. {drinks[i]}");
}

foreach (var drink in drinks)
{
    Console.WriteLine($"今日推薦：{drink}");
}
```

執行後，console 會輸出：

```text
1. Latte
2. Mocha
3. Americano
今日推薦：Latte
今日推薦：Mocha
今日推薦：Americano
```

<!--
for 迴圈有三個部分：初始值、條件、每次結束後要做的事。它適合需要索引的情境，例如要印出第幾筆。

foreach 則是直接走訪集合裡的每一個元素，不用管索引，寫法更簡潔，也不會出現索引超出範圍的錯誤。

實務上，只要不需要索引，我們都優先使用 foreach。
-->

---

# while、do...while 與 break / continue

```csharp
int stock = 5;
while (stock > 0)
{
    Console.WriteLine($"賣出一包，剩下 {--stock} 包");
    if (stock == 2) break;          // break：立刻跳出迴圈
}

int[] prices = [120, 0, 90];
foreach (var price in prices)
{
    if (price == 0) continue;       // continue：跳過這一次，進入下一次
    Console.WriteLine(price);
}
```

| 關鍵字 | 作用 |
| --- | --- |
| `break` | 結束整個迴圈 |
| `continue` | 跳過本次，繼續下一次 |

<!--
while 迴圈是條件成立就一直執行，適合不知道要跑幾次的情況，例如一直賣到庫存沒了為止。do while 則是先執行一次再判斷，確保至少跑一次。

break 和 continue 是控制迴圈的兩個關鍵字：break 直接結束整個迴圈；continue 只跳過這一次，繼續下一次。

注意 while 迴圈的條件一定要有機會變成 false，不然就會變成無窮迴圈，網站會卡住。
-->

---

# 使用迴圈的注意事項

| 注意事項 | 說明 |
| --- | --- |
| **之一：索引從 0 開始** | 最後一個元素是 `Count - 1`，超出會拋 `ArgumentOutOfRangeException` |
| **之二：foreach 中不能修改集合** | 在 foreach 裡 `Add` / `Remove` 會拋 `InvalidOperationException` |
| **之三：避免無窮迴圈** | while 的條件必須有機會變成 `false` |

<!--
迴圈的注意事項有三個。

之一，索引從 0 開始，所以條件要寫小於 Count，不是小於等於。

之二，foreach 走訪的過程中不能增減集合的元素，否則會拋例外。如果要移除購物車裡的某些品項，可以先用 LINQ 篩選出新的集合，這個下一章會教。

之三，while 迴圈要小心無窮迴圈。
-->

---
layout: default
---

# 練習 4：購物車小計
### 任務說明

1. 用 collection expression 建立購物車品項價格 `[450, 380, 520, 0, 299]`
2. 用 `foreach` 計算總金額，**價格為 0 的品項要跳過**（贈品）
3. 當累計金額超過 1,000 元時，印出「已達免運門檻」並**停止**加總
4. 印出最後的總金額

<!--
【練習目的】
練習 collection expression、foreach、continue 與 break 的搭配。

【解題引導】
先判斷 0 元用 continue，再累加金額，最後判斷是否超過 1000 元用 break。
-->

---
layout: default
---

# 練習 4：解題提示

```csharp
int[] prices = [450, 380, 520, 0, 299];
decimal total = 0;

foreach (var price in prices)
{
    if (price == 0) continue;
    total += price;
    if (total > 1000) { Console.WriteLine("已達免運門檻"); break; }
}
Console.WriteLine($"總金額：{total}");   // 總金額：1350
```

<!--
執行後會印出「已達免運門檻」和總金額 1350。450 加 380 是 830，再加 520 是 1350，超過 1000 就 break 了，後面的 0 和 299 都不會被處理。
-->

---
layout: section
class: flex flex-col justify-center items-center text-center
---

# 2-5 類別與物件
### Class, Property & Record

<!--
最後一個小節，也是這一章最重要的：類別與物件。ASP.NET Core 的 Controller、Model、Service 全部都是類別，這一節學好，後面的章節就會輕鬆很多。
-->

---

# 什麼是類別與物件？

| 概念 | 說明 | 比喻 |
| --- | --- | --- |
| **類別（Class）** | 物件的設計圖 | 咖啡豆的「商品規格表」 |
| **物件（Object）** | 依照類別建立出來的實體 | 架上一包包實際的咖啡豆 |
| **屬性（Property）** | 物件的資料 | 品名、價格、產地 |
| **方法（Method）** | 物件的行為 | 計算折扣價、顯示資訊 |

「類別定義了『有哪些資料和行為』，物件則是依照類別 `new` 出來、實際存在記憶體裡的東西。」

<!--
我們用咖啡豆商店來理解類別和物件。

類別就像商品規格表，上面規定每個商品都要有品名、價格、產地。物件則是依照規格表做出來的實際商品，例如「耶加雪菲 450 元」、「薇拉 380 元」，每一包都是一個獨立的物件。

屬性是物件的資料，方法是物件能做的事。在 ASP.NET Core 裡，資料庫的一張表會對應到一個類別，表裡的每一筆資料就對應到一個物件，這個第五章會實際看到。
-->

---

# 在 C# 中練習定義類別

```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }

    public decimal GetDiscountPrice(decimal rate) => Price * rate;
}
```

```csharp
var latte = new Product { Id = 1, Name = "拿鐵咖啡豆", Price = 400m };
Console.WriteLine($"{latte.Name} 會員價：{latte.GetDiscountPrice(0.9m)}");
// 拿鐵咖啡豆 會員價：360.0
```

<!--
我們來定義一個 Product 類別。

Id、Name、Price 是屬性，大括號裡的 get; set; 代表可以讀取也可以寫入，這叫 auto-property，是 C# 最常用的寫法。Name 後面等於空字串，是給它一個預設值，避免 null。

GetDiscountPrice 是方法，箭頭這種寫法叫 expression-bodied member，只有一行回傳值的方法可以這樣簡寫。

建立物件的時候，我們用 new 搭配大括號，直接設定屬性的值，這叫 object initializer，是 C# 很常用的寫法。
-->

---

# 屬性（Property）vs 欄位（Field）

| | 欄位 Field | 屬性 Property |
| --- | --- | --- |
| 寫法 | `private int _stock;` | `public int Stock { get; set; }` |
| 可見度 | 通常 `private` | 通常 `public` |
| 用途 | 類別內部儲存資料 | 對外公開資料，可加驗證邏輯 |
| EF Core / Model Binding | 不使用 | ✅ 使用屬性對應資料庫欄位與表單 |

```csharp
public class Product
{
    public int Stock
    {
        get;
        set => field = value < 0 ? 0 : value;   // C# 14 field 關鍵字：庫存不能為負
    }
}
```

<!--
學過 Java 的同學會習慣寫 private 欄位加上 getter、setter 方法，C# 用屬性把這件事簡化了。

欄位是類別內部用的變數；屬性則是對外公開的存取介面，而且可以在 get 和 set 裡加入邏輯。更重要的是，EF Core 對應資料庫、MVC 綁定表單資料，都是看屬性，不是看欄位，所以 Model 類別一定要用屬性。

C# 14 新增了 field 關鍵字，可以在 set 裡直接存取編譯器自動產生的欄位，以前要多宣告一個 private 欄位才能做到。這個例子是：如果設定的庫存是負數，就自動變成 0。
-->

---

# 存取修飾詞

| 修飾詞 | 誰可以存取 |
| --- | --- |
| `public` | 所有人 |
| `private` | 只有類別自己（欄位的預設值） |
| `protected` | 自己與子類別 |
| `internal` | 同一個專案（組件）內（類別的預設值） |

<div class="mt-4 p-3 bg-blue-50 border-l-4 border-blue-400 text-gray-700 text-sm text-left">
💡 <b>實務原則：</b>能用 <code>private</code> 就不要用 <code>public</code>。Controller 裡注入的服務欄位，都會宣告成 <code>private readonly</code>。
</div>

<!--
存取修飾詞決定了誰可以使用這個類別或成員。

就像咖啡店的區域劃分：public 是用餐區，所有客人都能進；private 是老闆辦公室，只有老闆能進；protected 是員工休息室，自己人和繼承的人可以進；internal 是整棟大樓，同一個專案裡都能用。

實務上的原則是最小權限：能 private 就不要 public。第六章學依賴注入時，會看到很多 private readonly 的欄位。
-->

---

# 建構子（Constructor）

「建構子是 `new` 物件時自動執行的方法，用來設定物件的初始狀態。」

```csharp
public class CartItem
{
    public string ProductName { get; }
    public int Count { get; private set; }

    public CartItem(string productName, int count)
    {
        ProductName = productName;
        Count = count;
    }

    public void Increase() => Count++;
}
```

```csharp
var item = new CartItem("耶加雪菲", 1);
item.Increase();
Console.WriteLine($"{item.ProductName} x {item.Count}");   // 耶加雪菲 x 2
```

<!--
建構子是一個特殊的方法，名稱跟類別一樣，沒有回傳型別，在 new 的時候會自動執行。

這個例子中，ProductName 只有 get，代表建立之後就不能修改；Count 的 set 是 private，代表外部不能直接改，只能透過 Increase 方法增加。這就是封裝：把資料保護起來，只開放特定的操作方式。

購物車的數量增減，第十章就會用類似的設計。
-->

---

# 現代寫法：Primary Constructor（C# 12）

```csharp
// 傳統寫法
public class OrderService
{
    private readonly ILogger<OrderService> _logger;
    public OrderService(ILogger<OrderService> logger)
    {
        _logger = logger;
    }
}

// Primary constructor：參數直接寫在類別名稱後面
public class OrderService(ILogger<OrderService> logger)
{
    public void PlaceOrder() => logger.LogInformation("訂單成立");
}
```

<div class="mt-4 p-3 bg-blue-50 border-l-4 border-blue-400 text-gray-700 text-sm text-left">
💡 第 6 章依賴注入會大量使用 primary constructor，讓 Controller 與 Service 的程式碼更精簡。
</div>

<!--
C# 12 新增了 primary constructor，建構子的參數可以直接寫在類別名稱後面，類別裡的任何地方都能直接使用這個參數。

原本要宣告欄位、寫建構子、在建構子裡指派，至少五六行的程式碼，現在一行就搞定了。

這個寫法在 ASP.NET Core 裡非常實用，第六章學依賴注入的時候，我們的 Controller 幾乎都會用這種寫法。
-->

---

# required 與 init：確保物件建立時就有完整資料

```csharp
public class Category
{
    public int Id { get; init; }                    // init：只能在建立時設定
    public required string Name { get; set; }       // required：建立時一定要給值
    public int DisplayOrder { get; set; }
}

var c1 = new Category { Name = "單品咖啡豆", DisplayOrder = 1 };  // ✅
var c2 = new Category { DisplayOrder = 2 };                     // ❌ 編譯錯誤：缺少 Name
```

| 關鍵字 | 意義 |
| --- | --- |
| `init` | 只能在 object initializer 設定，之後唯讀 |
| `required` | 建立物件時**一定要**設定，否則編譯錯誤 |

<!--
有時候我們希望物件在建立時就一定要有某些資料，例如分類一定要有名稱。

required 關鍵字就是做這件事：少給就編譯錯誤，錯誤在寫程式的時候就被抓出來。init 則是只能在建立時設定，之後就變成唯讀。

這兩個關鍵字搭配 nullable，可以大幅減少「忘記設定某個屬性」造成的 bug。
-->

---

# 什麼是 record？

「record 是專門用來『裝資料』的型別，自動提供值相等比較、`ToString()` 與不可變性。」

```csharp
public record ProductDto(int Id, string Name, decimal Price);

var a = new ProductDto(1, "Latte", 120m);
var b = new ProductDto(1, "Latte", 120m);

Console.WriteLine(a == b);         // True：record 比較的是「內容」
Console.WriteLine(a);              // ProductDto { Id = 1, Name = Latte, Price = 120 }

var c = a with { Price = 99m };    // 複製一份並修改 Price
```

| 使用時機 | 建議 |
| --- | --- |
| 資料庫實體（EF Core Entity） | `class` |
| API 回傳資料、DTO、查詢結果 | `record` |

<!--
record 是 C# 9 加入的型別，專門用來裝資料。

一行就能定義完一個有三個屬性的型別。最大的特色是相等比較：兩個 class 物件就算內容一樣，用雙等號比較也是 false，因為它比較的是記憶體位置；但 record 比較的是內容，內容一樣就是 true。

with 運算式可以複製一份 record 並修改部分屬性，原本的不會被改到。

實務上，資料庫的實體類別用 class，因為 EF Core 需要追蹤物件的變化；API 回傳的資料、查詢結果這種單純裝資料的東西，就很適合用 record。
-->

---

# 使用類別的注意事項

| 注意事項 | 說明 |
| --- | --- |
| **之一：類別是參考型別** | `var b = a;` 只是複製參照，改 `b` 也會改到 `a` |
| **之二：string 屬性要處理 null** | 給預設值 `= ""`、加上 `required`，或宣告成 `string?` |
| **之三：一個檔案一個類別** | 檔名與類別名稱一致，例如 `Product.cs` |

<!--
類別的注意事項有三個。

之一，類別是參考型別，把一個物件指派給另一個變數，兩個變數指向的是同一個物件，改其中一個，另一個也會變。

之二，開啟 Nullable 之後，string 屬性如果沒有處理，編譯器會警告。處理方式有三種：給預設值、加 required、或是明確宣告成 string?。

之三，養成一個檔案一個類別的習慣，檔名跟類別名稱一致，專案大了之後才找得到東西。
-->

---
layout: default
---

# 練習 5：設計咖啡豆商品類別
### 任務說明

1. 建立 `CoffeeBean` 類別，包含 `Name`（required）、`Origin`、`Price`、`Stock`
2. `Stock` 使用 `field` 關鍵字，確保不能小於 0
3. 加入 `Sell(int count)` 方法：庫存足夠才扣除並回傳 `true`，否則回傳 `false`
4. 建立一個 `CoffeeBeanDto` record，只包含 `Name` 與 `Price`

<!--
【練習目的】
綜合練習屬性、required、field 關鍵字、方法與 record。

【解題引導】
Sell 方法要先判斷 Stock 是否大於等於 count。record 用一行 positional 寫法就可以了。
-->

---
layout: default
---

# 練習 5：解題提示

```csharp
public class CoffeeBean
{
    public required string Name { get; set; }
    public string Origin { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set => field = Math.Max(0, value); }

    public bool Sell(int count)
    {
        if (Stock < count) return false;
        Stock -= count;
        return true;
    }
}

public record CoffeeBeanDto(string Name, decimal Price);
```

<!--
Stock 的 set 用 Math.Max 取 0 和 value 的最大值，就能確保不會小於 0。Sell 方法先判斷庫存是否足夠，不夠就直接回傳 false，這種「先處理例外情況、提早 return」的寫法叫 guard clause，能讓程式更好讀。

第十章送出訂單扣庫存的時候，我們會用類似的邏輯。
-->

---

# 補充：C# 與 Java 語法對照

| 概念 | Java | C# |
| --- | --- | --- |
| 方法命名 | `getName()` | `GetName()` |
| Getter / Setter | 手寫方法 | `{ get; set; }` 屬性 |
| 字串插值 | `String.format(...)` | `$"...{x}..."` |
| 型別推斷 | `var`（Java 10+） | `var` |
| 資料類別 | `record`（Java 16+） | `record`（C# 9+） |
| 套件 | `package` / `import` | `namespace` / `using` |
| 繼承 / 實作 | `extends` / `implements` | 都用 `:` |

<!--
最後補充一張 C# 和 Java 的對照表，給學過 Java 的同學參考。

兩個語言的概念幾乎一樣，主要差別在寫法：C# 方法大寫開頭、用屬性取代 getter setter、繼承和實作都用冒號。熟悉這張表之後，Java 的經驗可以直接帶過來。
-->

---
layout: default
---

# 綜合練習：咖啡店點餐系統
### 任務說明

用一個 `shop.cs` 檔案完成：

1. 定義 `record MenuItem(string Name, decimal Price)`
2. 用 collection expression 建立菜單（至少 4 項）
3. 用 `foreach` 印出菜單，格式：`1. Latte - 120 元`
4. 模擬顧客點了 3 項，計算總金額
5. 用 `switch expression` 依總金額決定折扣（≥ 500 打 9 折、≥ 300 打 95 折）
6. 印出應付金額

<!--
【練習目的】
把本章五個小節全部整合：程式架構、變數、條件、迴圈、類別與 record。

【解題引導】
top-level statements 的檔案裡，型別宣告（record）要放在程式碼的最後面。
-->

---
layout: default
---

# 綜合練習：解題提示

```csharp
List<MenuItem> menu = [new("Latte", 120m), new("Mocha", 140m),
                       new("Americano", 90m), new("Cold Brew", 150m)];
for (int i = 0; i < menu.Count; i++)
    Console.WriteLine($"{i + 1}. {menu[i].Name} - {menu[i].Price} 元");

List<MenuItem> order = [menu[0], menu[1], menu[3]];
decimal total = 0;
foreach (var item in order) total += item.Price;

var rate = total switch { >= 500 => 0.9m, >= 300 => 0.95m, _ => 1m };
Console.WriteLine($"總計 {total} 元，應付 {total * rate:N0} 元");

record MenuItem(string Name, decimal Price);
```

<!--
注意 new 後面直接接小括號，型別可以省略，這叫 target-typed new，因為 List 的型別已經告訴編譯器要建立 MenuItem 了。

record 的宣告放在檔案最後面，這是 top-level statements 的規定：可執行的程式碼要放在型別宣告之前。

總金額 410 元，打 95 折，應付 390 元。
-->

---
layout: section
class: flex flex-col justify-center items-center text-center
---

# EShop 專案實作
## 第 2 步：商品模型與會員折扣

<!--
回到我們的 EShop。上一章我們只建好了一個空的網站，裡面什麼資料都沒有。

一間商店最基本的就是「商品」。這一步我們用這一章學的類別、record、enum 和 switch expression，替 EShop 定義商品和分類，再寫一個計算會員折扣的類別。最後，我們還要寫下 EShop 的第一個單元測試，讓電腦幫我們檢查折扣算得對不對。
-->

---

# EShop 第 2 步：商品模型與會員折扣
### 任務說明

1. 在 `EShop.Web/Models` 建立 `Category` 與 `Product`：`Name`、`Origin` 用 `required`；`Stock` 用 `field` 關鍵字確保**不小於 0**，並提供 `InStock`
2. 定義 `enum MemberLevel`（一般、銀卡、金卡）與兩個 record：`CartLine(Product, Quantity)`、`PriceQuote(Subtotal, Discount)`
3. 建立 `Services/PriceCalculator`（primary constructor 傳入會員等級），用 switch expression 計算折扣：

| 會員等級 | 折扣 |
| --- | --- |
| 金卡 | 9 折；小計滿 2,000 元改為 85 折 |
| 銀卡 | 95 折 |
| 一般 | 不打折 |

4. 建立 `EShop.Tests`（xUnit）測試專案，驗證上面的折扣規則

<!--
第二步的任務有四件事。

先定義商品和分類兩個類別。商品名稱和產地一定要有，所以加上 required；庫存不能是負數，我們用 C# 14 的 field 關鍵字在 set 裡把關。

接著定義會員等級的 enum，以及兩個 record：CartLine 代表購物車的一行，PriceQuote 代表試算的結果。這種「只裝資料、不會改變」的小型別，最適合用 record。

第三件事是折扣計算，規則在表格裡：金卡打 9 折，買滿兩千升級成 85 折；銀卡 95 折；一般會員不打折。

最後，我們要建立一個測試專案，把這張表變成自動化的測試。
-->

---

# EShop 第 2 步：解題提示
### Product：required 與 field 關鍵字

```csharp
// eshop/EShop.Web/Models/Product.cs
public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Origin { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }

    // C# 14 的 field 關鍵字：庫存永遠不會小於 0
    public int Stock
    {
        get;
        set => field = Math.Max(0, value);
    }

    public bool InStock => Stock > 0;
}
```

<!--
先看 Product。Name 和 Origin 加上 required，建立商品的時候如果忘了給名稱，編譯器就會直接報錯，不用等到執行才發現。

Stock 用了 field 關鍵字。以前要自己宣告一個私有欄位來存值，現在直接寫 field 就好。set 裡面用 Math.Max 取 0 和 value 比較大的那個，所以就算有人寫 Stock = -5，存進去的也會是 0。

InStock 是一個只有 get 的屬性，用箭頭寫法，庫存大於 0 就是有貨。Category 類別也是同樣的寫法，只有 Id、Name 和 DisplayOrder 三個屬性。
-->

---

# EShop 第 2 步：解題提示（續）
### enum 與 record

```csharp
// eshop/EShop.Web/Models/MemberLevel.cs
public enum MemberLevel
{
    Regular,   // 一般會員
    Silver,    // 銀卡
    Gold,      // 金卡
}
```

```csharp
// eshop/EShop.Web/Models/CartLine.cs
// 購物車的一行：哪個商品、買幾包
public record CartLine(Product Product, int Quantity)
{
    public decimal Subtotal => Product.Price * Quantity;
}

// 試算結果：小計、折扣、應付金額
public record PriceQuote(decimal Subtotal, decimal Discount)
{
    public decimal Total => Subtotal - Discount;
}
```

<!--
會員等級只有三種，而且是固定的，用 enum 最適合。比起直接用字串 "Gold"，enum 打錯字編譯器會抓到。

CartLine 和 PriceQuote 都用 positional record 的寫法，括號裡的參數會自動變成唯讀屬性。record 的大括號裡一樣可以加屬性，這裡加了 Subtotal 和 Total，都是用其他屬性算出來的。

record 還有一個好處：兩個 PriceQuote 只要內容一樣，用 Equals 比較就會相等，等一下寫測試的時候很方便。
-->

---

# EShop 第 2 步：解題提示（續 2）
### PriceCalculator：primary constructor + switch expression

```csharp
// eshop/EShop.Web/Services/PriceCalculator.cs
public class PriceCalculator(MemberLevel level)
{
    // 會員折扣：金卡 9 折、滿 2000 再升級為 85 折；銀卡 95 折
    public decimal GetDiscount(decimal subtotal) => (level, subtotal) switch
    {
        (_, <= 0) => 0,
        (MemberLevel.Gold, >= 2000) => Math.Round(subtotal * 0.15m),
        (MemberLevel.Gold, _) => Math.Round(subtotal * 0.10m),
        (MemberLevel.Silver, _) => Math.Round(subtotal * 0.05m),
        _ => 0,
    };

    public PriceQuote Quote(List<CartLine> lines)
    {
        decimal subtotal = 0;
        foreach (var line in lines)
        {
            subtotal += line.Subtotal;
        }
        return new PriceQuote(subtotal, GetDiscount(subtotal));
    }
}
```

<!--
PriceCalculator 用 primary constructor，類別名稱後面的括號直接接收會員等級，類別裡面任何地方都可以用 level。

GetDiscount 是這一步的重點。我們把會員等級和小計組成一個 tuple，再用 switch expression 比對。第一行的底線代表「不管是什麼等級」，只要小計小於等於 0，折扣就是 0。接下來金卡滿兩千、金卡、銀卡，最後的底線是其他情況。

大家注意順序很重要：金卡滿兩千那一行一定要放在金卡那一行前面，因為 switch 是由上往下比對，第一個符合的就會被採用。

Quote 用 foreach 把每一行的小計加起來，再算出折扣，回傳一個 PriceQuote。
-->

---

# EShop 第 2 步：解題提示（續 3）
### 補充：用 xUnit 寫第一個單元測試

```bash
dotnet new xunit -n EShop.Tests
dotnet sln add EShop.Tests/EShop.Tests.csproj
dotnet add EShop.Tests reference EShop.Web
```

```csharp
// eshop/EShop.Tests/PriceCalculatorTests.cs
    [Theory]
    [InlineData(MemberLevel.Regular, 1000, 0)]
    [InlineData(MemberLevel.Silver, 1000, 50)]
    [InlineData(MemberLevel.Gold, 1000, 100)]
    [InlineData(MemberLevel.Gold, 2000, 300)]
    [InlineData(MemberLevel.Gold, 0, 0)]
    public void GetDiscount_依會員等級計算折扣(
        MemberLevel level, int subtotal, int expected)
    {
        var calculator = new PriceCalculator(level);

        var discount = calculator.GetDiscount(subtotal);

        Assert.Equal(expected, discount);
    }
```

執行 `dotnet test`：`Passed! - Failed: 0, Passed: 7`

<!--
最後來寫測試。單元測試就是「用程式檢查程式」：我們寫下「金卡買一千元，折扣應該是一百元」，讓電腦幫我們一條一條驗證。

用三個指令建立 xUnit 測試專案，加入方案，再讓它參考 EShop.Web，這樣才能使用 PriceCalculator。

Theory 加上 InlineData，代表同一個測試方法用不同的資料跑好幾次，每一行 InlineData 就是一組輸入和預期結果。這樣上一頁的折扣表，就直接變成了五個測試案例。

在方案資料夾執行 dotnet test，看到 Passed 就代表全部通過。之後每一章我們都會加上新的測試，改程式的時候跑一下，就知道有沒有把原本的功能改壞。
-->

---

# 總結

| 小節 | 重點 |
| --- | --- |
| 2-1 程式架構 | top-level statements、file-scoped namespace、PascalCase 命名 |
| 2-2 語法介紹 | 金額用 `decimal`、`var` 型別推斷、`$""` 字串插值、`?` 與 `??` 處理 null |
| 2-3 條件控制 | `if` 執行動作、`switch expression` 算出結果、pattern matching |
| 2-4 迴圈控制 | `foreach` 最常用、collection expression `[...]` |
| 2-5 類別與物件 | 屬性 `{ get; set; }`、primary constructor、`required`、`record` |
| **EShop** 第 2 步 | `Product`（`required`、`field`）、`MemberLevel`、record、switch expression 會員折扣，加上第一個 xUnit 測試 |

下一章我們會介紹 **LINQ**，學會用幾行程式碼完成篩選、排序、分組等資料查詢。

<!--
我們來總結這一章。

C# 的程式架構可以用 top-level statements 寫得很精簡；變數要注意型別，金額用 decimal；條件判斷除了 if，還有現代的 switch expression；迴圈最常用 foreach；類別用屬性來定義資料，primary constructor 和 record 讓程式更精簡。

學完這一章，打開 ASP.NET Core 的程式碼，大部分的語法大家應該都看得懂了。

EShop 在這一章有了商品、分類和會員折扣：類別用 required 和 field 關鍵字把關資料，record 裝試算結果，switch expression 讓折扣規則一目了然。我們也寫下了第一個單元測試，之後改程式都有安全網。

下一章我們會介紹 LINQ，這是 C# 最有特色的功能，可以用幾行程式碼完成篩選、排序、分組，而且之後查詢資料庫也是用同樣的寫法。
-->

---
layout: end
---

# 第 2 章結束
### 下一章：LINQ 與資料查詢
