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
title: LINQ 與資料查詢
routeAlias: ch03
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
  <h1 style="color: #1a5c5c; font-size: 3.4rem; font-weight: 900; line-height: 1.15; margin-bottom: 1.5rem;">LINQ 與資料查詢</h1>
  <div style="height: 4px; width: 320px; background: linear-gradient(90deg, #5eada0, #a7d9d0); border-radius: 2px; margin-bottom: 1.5rem;"></div>
  <p style="color: #4a7c7c; font-size: 1.15rem; font-style: italic;">「用一句話，問出你想要的資料」</p>
  <Link to="home" style="color: #9dc4c4; font-size: 0.85rem; margin-top: 2rem; text-decoration: none; letter-spacing: 0.05em;">← 返回目錄</Link>
</div>

<!--
大家好，歡迎來到第三章！

上一章我們學了 C# 的基礎語法，會用 foreach 走訪集合，也會用 if 做判斷。但如果老闆問：「幫我找出所有價格低於 500 元、而且還有庫存的咖啡豆，照價格排序」，用 foreach 加 if 寫起來會很長。

這一章要介紹的 LINQ，可以讓我們用一行程式碼完成這種查詢。更重要的是，後面第五章開始查詢資料庫，用的也是 LINQ，所以這一章是連接 C# 語法和資料庫操作的關鍵橋樑。
-->

---
layout: default
---

# Outline

- **回顧：foreach 與類別**
- **3-1 委派（Delegate）與 Lambda 表達式**
- **3-2 查詢語法 vs 方法語法**
- **3-3 篩選與轉換：Where、Select、OrderBy**
- **3-4 聚合與單筆抓取：FirstOrDefault、Any、Count、GroupBy**
- **3-5 延遲執行 vs 立即執行（ToList）**
- **3-6 IEnumerable vs IQueryable**
- **總結**

<!--
這一章有六個小節。

第一節先學委派和 Lambda，因為 LINQ 的每個方法都要傳 Lambda 進去，不懂 Lambda 就看不懂 LINQ。接著介紹 LINQ 的兩種寫法，然後是最常用的方法。最後兩節是觀念：延遲執行，以及 IEnumerable 和 IQueryable 的差別，這兩個觀念如果沒搞懂，之後查資料庫很容易踩到效能地雷。
-->

---

# 回顧：用 foreach 篩選資料

上一章我們學過用 `foreach` + `if` 處理集合：

```csharp
List<Product> result = [];
foreach (var p in products)
{
    if (p.Price < 500 && p.Stock > 0)
        result.Add(p);
}
```

「如果還要排序、只取名稱、再分組……程式碼會越寫越長。」

<!--
我們先回顧上一章的寫法：要篩選資料，就用 foreach 走訪集合，用 if 判斷條件，符合的放進新的 List。

這個寫法沒有錯，但如果需求再加上「照價格排序」、「只要名稱」、「依產地分組」，程式碼就會越來越長，而且邏輯分散在迴圈的各個地方，很難一眼看懂在做什麼。

這就是 LINQ 要解決的問題。不過在學 LINQ 之前，我們要先搞懂一個東西：Lambda。
-->

---

# 本章使用的範例資料

```csharp
public record Product(int Id, string Name, string Origin, decimal Price, int Stock, int CategoryId);

List<Product> products =
[
    new(1, "耶加雪菲",   "衣索比亞", 450m, 20, 1),
    new(2, "西達摩",     "衣索比亞", 420m,  0, 1),
    new(3, "薇拉",       "哥倫比亞", 380m, 15, 1),
    new(4, "藝伎",       "巴拿馬",  1200m,  5, 2),
    new(5, "曼特寧",     "印尼",     400m, 30, 1),
    new(6, "綜合配方豆", "綜合",     299m, 50, 3),
];
```

<!--
這一章所有範例都會使用這份商品資料。

Product 用上一章學的 record 來定義，包含編號、名稱、產地、價格、庫存和分類編號。資料用 collection expression 建立，有六包咖啡豆，注意西達摩的庫存是 0。

大家可以把這段放在 linq.cs 的最上面，搭配 dotnet run linq.cs 邊學邊試。記得 record 的宣告要放在檔案最後面。
-->

---
layout: section
class: flex flex-col justify-center items-center text-center
---

# 3-1 委派與 Lambda 表達式
### Delegate & Lambda

<!--
第一個小節，我們來認識委派和 Lambda 表達式。
-->

---

# 什麼是委派（Delegate）？

想像咖啡店老闆要篩選商品，但「篩選條件」每天都不一樣：

| 今天的需求 | 條件 |
| --- | --- |
| 週一 | 價格低於 500 元 |
| 週二 | 有庫存的 |
| 週三 | 產地是衣索比亞 |

「委派就是一種型別，它的值是一個『方法』。我們可以把『條件』當作參數傳進去。」

<!--
我們先想一個情境：老闆要篩選商品，但每天的條件都不一樣。如果每種條件都寫一個方法，那就會有 FilterByPrice、FilterByStock、FilterByOrigin……寫不完。

更好的做法是：寫一個通用的 Filter 方法，然後把「條件」當作參數傳進去。但條件是一段邏輯，不是一個值，要怎麼當參數傳？

這就是委派的用途。「委派就是一種型別，它的值是一個方法」。有了委派，我們就可以把一段邏輯，像變數一樣傳來傳去。
-->

---

# 內建的委派型別：Func、Action、Predicate

| 委派 | 意義 | 範例 |
| --- | --- | --- |
| `Func<T, TResult>` | 接收 T，回傳 TResult | `Func<int, int> square` |
| `Func<T, bool>` | 接收 T，回傳 true/false（條件） | LINQ 的 `Where` 參數 |
| `Action<T>` | 接收 T，沒有回傳值 | `Action<string> log` |
| `Predicate<T>` | 等同 `Func<T, bool>` | `List<T>.FindAll` 參數 |

```csharp
Func<int, int> square = x => x * x;
Action<string> log = msg => Console.WriteLine($"[LOG] {msg}");

log($"5 的平方是 {square(5)}");    // [LOG] 5 的平方是 25
```

<!--
C# 已經幫我們準備好幾個通用的委派型別，實務上幾乎不用自己宣告。

Func 是有回傳值的，最後一個型別參數是回傳型別；Action 是沒有回傳值的；Predicate 是回傳 bool 的條件判斷。

其中最重要的是 Func of T, bool，也就是「接收一個物件，回傳 true 或 false」，LINQ 的 Where 方法要的就是這個。
-->

---

# 什麼是 Lambda 表達式？

「Lambda 就是一個沒有名字、可以直接寫在參數位置的簡短方法。」

| 寫法 | 範例 |
| --- | --- |
| 一般方法 | `static bool IsCheap(Product p) { return p.Price < 500; }` |
| Lambda | `p => p.Price < 500` |

| 符號 | 意義 |
| --- | --- |
| `p` | 參數名稱（隨意取，習慣用型別的第一個字母） |
| `=>` | 讀作「goes to」，左邊是參數，右邊是回傳的運算式 |
| `p.Price < 500` | 回傳值（只有一行時不用寫 `return`） |

<!--
如果每個條件都要寫一個有名字的方法，還是很麻煩。Lambda 就是把方法簡化到只剩最核心的部分。

原本要寫 static bool IsCheap 括號 Product p，大括號 return，現在只要寫 p 箭頭 p.Price 小於 500。

箭頭左邊是參數，右邊是回傳的結果。參數名稱可以隨便取，習慣上用型別的第一個字母，例如 Product 就用 p。
-->

---

# 在 C# 中練習 Lambda

```csharp
// 把「條件」當作參數傳入
static List<Product> Filter(List<Product> source, Func<Product, bool> condition)
{
    List<Product> result = [];
    foreach (var p in source)
        if (condition(p)) result.Add(p);
    return result;
}

var cheap   = Filter(products, p => p.Price < 500);
var inStock = Filter(products, p => p.Stock > 0);
var ethiopia = Filter(products, p => p.Origin == "衣索比亞");

Console.WriteLine($"便宜的有 {cheap.Count} 包");   // 便宜的有 5 包
```

<!--
我們把剛剛的情境實作出來。

Filter 方法的第二個參數是 Func of Product, bool，代表「傳一個條件進來」。方法裡面用 foreach 走訪，每一筆都呼叫 condition 判斷，符合就加入結果。

呼叫的時候，條件直接用 Lambda 寫在參數位置。週一要便宜的、週二要有庫存的、週三要衣索比亞的，都用同一個 Filter 方法，只是傳入不同的 Lambda。

其實這個 Filter 方法，就是 LINQ 的 Where 方法在做的事情。LINQ 就是微軟幫我們寫好的一整套這種方法。
-->

---

# 使用 Lambda 的注意事項

| 注意事項 | 說明 |
| --- | --- |
| **之一：多行要加大括號與 return** | `p => { var tax = p.Price * 0.05m; return p.Price + tax; }` |
| **之二：參數型別通常可省略** | 編譯器會從委派型別推斷 `p` 是 `Product` |
| **之三：可以捕捉外部變數** | `var max = 500m; Filter(products, p => p.Price < max);` |

<!--
Lambda 有三個注意事項。

之一，如果邏輯超過一行，要加大括號，也要明確寫 return。之二，參數的型別編譯器會自動推斷，通常不用寫。之三，Lambda 可以使用外面宣告的變數，這叫 closure，在 Controller 裡把使用者輸入的關鍵字放進查詢條件時，就會用到這個特性。
-->

---
layout: default
---

# 練習 1：委派與 Lambda
### 任務說明

1. 宣告一個 `Func<decimal, decimal>` 變數 `addTax`，回傳含 5% 營業稅的價格
2. 宣告一個 `Action<Product>` 變數 `print`，印出「品名：價格 元」
3. 用 `foreach` 走訪 `products`，對每個商品呼叫 `print`，並印出含稅價

<!--
【練習目的】
熟悉 Func 與 Action 的差異，以及 Lambda 的寫法。

【解題引導】
Func 最後一個型別參數是回傳型別；Action 沒有回傳值。
-->

---
layout: default
---

# 練習 1：解題提示

```csharp
Func<decimal, decimal> addTax = price => price * 1.05m;
Action<Product> print = p => Console.WriteLine($"{p.Name}：{p.Price} 元");

foreach (var p in products)
{
    print(p);
    Console.WriteLine($"  含稅價：{addTax(p.Price):N0} 元");
}
```

<!--
Func of decimal, decimal 代表接收 decimal、回傳 decimal；Action of Product 代表接收 Product、沒有回傳值。呼叫委派的方式跟呼叫一般方法一樣，後面加小括號就好。
-->

---
layout: section
class: flex flex-col justify-center items-center text-center
---

# 3-2 LINQ 核心觀念
### Query Syntax vs Method Syntax

<!--
有了 Lambda 的基礎，我們正式進入 LINQ。
-->

---

# 什麼是 LINQ？

「LINQ（Language Integrated Query，語言整合查詢）讓我們用統一的 C# 語法，查詢任何資料來源。」

| 資料來源 | LINQ 的名稱 | 本課程 |
| --- | --- | --- |
| 記憶體中的集合（`List`、陣列） | LINQ to Objects | 本章 |
| 資料庫 | LINQ to Entities（EF Core） | 第 5 章起 |
| XML | LINQ to XML | 補充 |

<div class="mt-4 p-3 bg-blue-50 border-l-4 border-blue-400 text-gray-700 text-sm text-left">
💡 <b>重點：</b>學會一套 LINQ 語法，查 List 和查資料庫的寫法幾乎一樣。
</div>

<!--
LINQ 全名是 Language Integrated Query，中文叫語言整合查詢。

「語言整合」的意思是：查詢的語法是 C# 語言的一部分，編譯器會幫我們檢查型別，打錯欄位名稱在寫程式時就會被抓出來。

LINQ 最厲害的地方是：不管資料是在記憶體的 List 裡，還是在資料庫裡，查詢的寫法幾乎一模一樣。這一章我們先查 List，第五章換成查資料庫，大家會發現寫法完全通用。
-->

---

# LINQ 的兩種寫法

需求：找出價格低於 500 元的商品名稱，依價格由低到高排序

```csharp
// 查詢語法（Query Syntax）：像 SQL
var q1 = from p in products
         where p.Price < 500
         orderby p.Price
         select p.Name;

// 方法語法（Method Syntax）：串接方法 + Lambda
var q2 = products
    .Where(p => p.Price < 500)
    .OrderBy(p => p.Price)
    .Select(p => p.Name);
```

兩者結果完全相同：`綜合配方豆、薇拉、曼特寧、西達摩、耶加雪菲`

<!--
LINQ 有兩種寫法。

查詢語法長得很像 SQL，from、where、orderby、select，學過 SQL 的同學會覺得很親切。不過注意它的順序是 from 開頭、select 結尾，跟 SQL 相反。

方法語法則是用點把一個一個方法串起來，每個方法傳入一個 Lambda。

兩種寫法編譯之後是一樣的，查詢語法最後也會被編譯器轉成方法語法。
-->

---

# 查詢語法 vs 方法語法：怎麼選？

| 比較 | 查詢語法 | 方法語法 |
| --- | --- | --- |
| 外觀 | 類似 SQL | 方法串接 + Lambda |
| 支援的操作 | 部分（沒有 `Count`、`First`、`Take`） | **全部** |
| 多表 join、`let` | 較好讀 | 較冗長 |
| 業界主流 | 較少 | ✅ **主流** |

「本課程以**方法語法**為主，EF Core 官方文件與大部分專案也都使用方法語法。」

<!--
那要選哪一種？

查詢語法在多表 join 的時候比較好讀，但它不支援所有操作，像 Count、First、Take 這些方法，最後還是要切換回方法語法。

方法語法支援所有操作，而且是業界的主流寫法，EF Core 的官方文件幾乎都是方法語法。所以這門課我們以方法語法為主，查詢語法大家看得懂就好。
-->

---
layout: default
---

# 練習 2：兩種語法互相轉換
### 任務說明

將下列查詢語法改寫成**方法語法**：

```csharp
var result = from p in products
             where p.Origin == "衣索比亞"
             orderby p.Price descending
             select new { p.Name, p.Price };
```

<!--
【練習目的】
熟悉兩種語法的對應關係。

【解題引導】
orderby 加上 descending，在方法語法中對應哪個方法？select new 大括號是什麼東西？下一節會詳細介紹匿名型別。
-->

---
layout: default
---

# 練習 2：解題提示

```csharp
var result = products
    .Where(p => p.Origin == "衣索比亞")
    .OrderByDescending(p => p.Price)
    .Select(p => new { p.Name, p.Price });

foreach (var r in result)
    Console.WriteLine($"{r.Name} {r.Price}");
// 耶加雪菲 450
// 西達摩 420
```

<!--
orderby descending 對應 OrderByDescending。select new 大括號會建立一個匿名型別，只包含 Name 和 Price 兩個屬性，下一節會詳細說明。
-->

---
layout: section
class: flex flex-col justify-center items-center text-center
---

# 3-3 常用資料篩選與轉換
### Where, Select, OrderBy

<!--
第三個小節，我們來看 LINQ 最常用的三個方法：Where、Select、OrderBy。
-->

---

# 篩選與轉換的方法

| 方法 | 用途 | SQL 對應 |
| --- | --- | --- |
| `Where(條件)` | 篩選符合條件的資料 | `WHERE` |
| `Select(轉換)` | 把每筆資料轉換成新的形狀 | `SELECT 欄位` |
| `OrderBy` / `OrderByDescending` | 升冪 / 降冪排序 | `ORDER BY ... ASC/DESC` |
| `ThenBy` / `ThenByDescending` | 第二排序條件 | `ORDER BY a, b` |
| `Skip(n)` / `Take(n)` | 跳過 n 筆 / 取 n 筆（分頁） | `OFFSET` / `FETCH` |
| `Distinct()` | 去除重複 | `DISTINCT` |

<!--
這張表列出最常用的篩選與轉換方法，右邊是對應的 SQL，學過資料庫的同學可以對照著看。

Where 篩選、Select 轉換、OrderBy 排序，這三個幾乎每個查詢都會用到。ThenBy 是第二排序條件，Skip 和 Take 搭配起來可以做分頁。
-->

---

# 在 C# 中練習 Where

```csharp
// 單一條件
var inStock = products.Where(p => p.Stock > 0);

// 多個條件：&& 串接，或是串接多個 Where
var cheapInStock = products.Where(p => p.Price < 500 && p.Stock > 0);
var same = products.Where(p => p.Price < 500).Where(p => p.Stock > 0);

// 搭配外部變數（例如使用者輸入的關鍵字）
string keyword = "雪";
var search = products.Where(p => p.Name.Contains(keyword));

Console.WriteLine(string.Join("、", cheapInStock.Select(p => p.Name)));
// 耶加雪菲、薇拉、曼特寧、綜合配方豆
```

<!--
Where 的參數就是一個回傳 bool 的 Lambda，回傳 true 的資料會被留下來。

多個條件可以用 && 寫在同一個 Lambda 裡，也可以串接多個 Where，結果一樣。串接的寫法在「根據使用者有沒有輸入條件，動態加上篩選」的時候特別好用，第八章做搜尋功能時會用到。

西達摩因為庫存是 0，藝伎因為價格超過 500，所以都被篩掉了。
-->

---

# 在 C# 中練習 Select：改變資料的形狀

```csharp
// 只取一個欄位
IEnumerable<string> names = products.Select(p => p.Name);

// 匿名型別：臨時組合需要的欄位
var list = products.Select(p => new { p.Name, 含稅價 = p.Price * 1.05m });

// 轉成 record（DTO）
var dtos = products.Select(p => new ProductDto(p.Name, p.Price));

foreach (var item in list)
    Console.WriteLine($"{item.Name}：{item.含稅價:N0}");

record ProductDto(string Name, decimal Price);
```

<!--
Select 是用來「改變資料的形狀」：輸入一個 Product，輸出什麼由我們決定。

可以只取出一個欄位，例如只要名稱；也可以用 new 大括號建立匿名型別，臨時組合需要的欄位，甚至可以取新的名稱；還可以轉成我們定義好的 record。

匿名型別只能在同一個方法裡使用，不能當作方法的回傳型別。如果要傳到 View 或回傳給前端，建議定義成 record 或 ViewModel，第八章會詳細介紹 ViewModel。
-->

---

# 在 C# 中練習 OrderBy 與分頁

```csharp
// 先依產地排序，同產地再依價格由高到低
var sorted = products
    .OrderBy(p => p.Origin)
    .ThenByDescending(p => p.Price);

// 分頁：每頁 2 筆，取第 2 頁
int page = 2, pageSize = 2;
var paged = products
    .OrderBy(p => p.Id)
    .Skip((page - 1) * pageSize)
    .Take(pageSize);

Console.WriteLine(string.Join("、", paged.Select(p => p.Name)));   // 薇拉、藝伎
```

<!--
OrderBy 是升冪排序，由小到大；OrderByDescending 是降冪。如果要第二個排序條件，要用 ThenBy，不要再寫一次 OrderBy，否則前一個排序會被覆蓋掉。

分頁是網站很常見的需求，公式是：跳過「頁數減一乘以每頁筆數」，再取「每頁筆數」。第二頁每頁兩筆，就是跳過兩筆，取兩筆，所以是薇拉和藝伎。

注意分頁之前一定要先排序，否則每次查詢的順序不保證一樣，尤其是查資料庫的時候。
-->

---

# 使用 Where / Select / OrderBy 的注意事項

| 注意事項 | 說明 |
| --- | --- |
| **之一：LINQ 不會修改原本的集合** | `products.Where(...)` 回傳新的序列，`products` 不變 |
| **之二：連續排序用 ThenBy** | 寫兩次 `OrderBy`，第一個排序會被覆蓋 |
| **之三：分頁前一定要排序** | 沒排序的 `Skip/Take`，每次結果可能不同 |

<!--
使用這三個方法的注意事項有三個。

之一，LINQ 不會修改原本的集合，它永遠回傳一個新的序列，這點跟上一章說的「foreach 裡面不能改集合」剛好互補：要移除某些資料，就用 Where 篩出新的集合。

之二，多重排序要用 ThenBy。之三，分頁前一定要排序。
-->

---
layout: default
---

# 練習 3：商品列表頁查詢
### 任務說明

模擬商品列表頁，依序完成：

1. 只顯示**有庫存**的商品
2. 若 `keyword` 不是空字串，再篩選名稱包含 `keyword` 的商品
3. 依價格**由高到低**排序
4. 只取出 `Name` 與 `Price`，轉成 `record ProductItem`
5. 每頁 3 筆，取第 1 頁

<!--
【練習目的】
把 Where、Select、OrderBy、Skip、Take 串成一個完整的查詢。

【解題引導】
第 2 點是「有條件才篩選」，可以先把查詢存在變數裡，再用 if 決定要不要串接 Where。
-->

---
layout: default
---

# 練習 3：解題提示

```csharp
string keyword = "";
int page = 1, pageSize = 3;

var query = products.Where(p => p.Stock > 0);
if (!string.IsNullOrWhiteSpace(keyword))
    query = query.Where(p => p.Name.Contains(keyword));

var items = query
    .OrderByDescending(p => p.Price)
    .Select(p => new ProductItem(p.Name, p.Price))
    .Skip((page - 1) * pageSize)
    .Take(pageSize);

record ProductItem(string Name, decimal Price);
```

<!--
關鍵在於：先把基本查詢存在 query 變數裡，有關鍵字的時候再串接一個 Where。這就是「動態組合查詢」的寫法，第八章的商品搜尋會直接用到。

結果會是藝伎、耶加雪菲、曼特寧。
-->

---
layout: section
class: flex flex-col justify-center items-center text-center
---

# 3-4 常用聚合與單筆抓取
### FirstOrDefault, Any, Count, GroupBy

<!--
第四個小節，我們來看「只要一筆資料」和「統計資料」的方法。
-->

---

# 單筆抓取的方法

| 方法 | 找到 | 找不到 | 找到多筆 |
| --- | --- | --- | --- |
| `First(條件)` | 回傳第一筆 | ❌ 拋例外 | 回傳第一筆 |
| `FirstOrDefault(條件)` | 回傳第一筆 | 回傳 `null` / 預設值 | 回傳第一筆 |
| `Single(條件)` | 回傳該筆 | ❌ 拋例外 | ❌ 拋例外 |
| `SingleOrDefault(條件)` | 回傳該筆 | 回傳 `null` / 預設值 | ❌ 拋例外 |

<div class="mt-4 p-3 bg-blue-50 border-l-4 border-blue-400 text-gray-700 text-sm text-left">
💡 <b>實務建議：</b>用 Id 查詢單筆資料時，最常用 <code>FirstOrDefault</code>，並搭配 <code>is null</code> 判斷找不到的情況。
</div>

<!--
很多時候我們只需要一筆資料，例如使用者點進商品詳細頁，我們要根據 Id 找出那一筆商品。

First 和 Single 系列的差別在於：找不到或找到多筆的時候，是拋例外還是回傳 null。

實務上最常用的是 FirstOrDefault，因為使用者可能在網址亂輸入一個不存在的 Id，我們不希望網站直接爆炸，而是回傳 null，然後我們自己判斷要顯示「找不到商品」的頁面。第五章的 Edit 和 Delete 都會用到這個寫法。
-->

---

# 單筆抓取 — 範例

```csharp
var p1 = products.FirstOrDefault(p => p.Id == 4);
Console.WriteLine(p1?.Name ?? "找不到");            // 藝伎

var p2 = products.FirstOrDefault(p => p.Id == 99);
if (p2 is null)
    Console.WriteLine("找不到商品");                 // 找不到商品

var p3 = products.First(p => p.Id == 99);           // ❌ InvalidOperationException
```

<!--
FirstOrDefault 找不到的時候會回傳 null，所以我們可以用上一章學的 ?. 和 ??，或是 is null 來處理。

First 找不到則會直接拋出 InvalidOperationException，如果沒有處理，整個請求就會失敗。所以除非你百分之百確定資料一定存在，否則用 FirstOrDefault 比較安全。
-->

---

# 判斷與統計的方法

| 方法 | 用途 | 範例 |
| --- | --- | --- |
| `Any(條件)` | 是否**至少有一筆**符合 | `products.Any(p => p.Stock == 0)` |
| `All(條件)` | 是否**全部**符合 | `products.All(p => p.Price > 0)` |
| `Count(條件)` | 符合的筆數 | `products.Count(p => p.Stock > 0)` |
| `Sum(欄位)` | 加總 | `cart.Sum(c => c.Price * c.Count)` |
| `Max` / `Min` / `Average` | 最大、最小、平均 | `products.Max(p => p.Price)` |

```csharp
Console.WriteLine(products.Any(p => p.Stock == 0));     // True（西達摩缺貨）
Console.WriteLine(products.Count(p => p.Stock > 0));    // 5
Console.WriteLine(products.Sum(p => p.Price * p.Stock)); // 庫存總價值
```

<!--
這一組方法是用來判斷和統計的。

Any 判斷是否至少有一筆符合，例如「有沒有缺貨的商品」；All 判斷是否全部符合；Count 算筆數；Sum 加總，第十章計算購物車總金額就是用 Sum；Max、Min、Average 則是最大、最小和平均值。

特別提醒：只想知道「有沒有」的時候，要用 Any，不要用 Count 大於 0。Any 找到第一筆就會停下來，Count 則要把全部數完，查資料庫的時候效能差很多。
-->

---

# 什麼是 GroupBy？

想像老闆問：「每個產地各有幾款咖啡豆？平均價格多少？」

```csharp
var groups = products
    .GroupBy(p => p.Origin)
    .Select(g => new
    {
        Origin = g.Key,              // 分組依據的值
        Count = g.Count(),           // 這一組有幾筆
        AvgPrice = g.Average(p => p.Price)
    });

foreach (var g in groups)
    Console.WriteLine($"{g.Origin}：{g.Count} 款，均價 {g.AvgPrice:N0}");
```

```text
衣索比亞：2 款，均價 435
哥倫比亞：1 款，均價 380
巴拿馬：1 款，均價 1,200
...
```

<!--
GroupBy 是分組，對應 SQL 的 GROUP BY。

GroupBy 之後，每一組都有一個 Key，就是分組依據的值，例如產地名稱；而這一組本身又是一個集合，可以再用 Count、Average、Sum 做統計。

通常 GroupBy 後面都會接一個 Select，把每一組整理成我們要的形狀。第十章做後台訂單統計的時候就會用到。
-->

---

# 補充：.NET 9 / 10 新增的 LINQ 方法

| 方法 | 版本 | 用途 |
| --- | --- | --- |
| `CountBy(key)` | .NET 9 | 直接依 key 計算筆數，不用 `GroupBy` + `Count` |
| `AggregateBy(key, ...)` | .NET 9 | 依 key 做自訂聚合 |
| `Index()` | .NET 9 | 走訪時同時取得索引 `(index, item)` |
| `LeftJoin` / `RightJoin` | .NET 10 | 外部連接，不用再寫 `GroupJoin` + `DefaultIfEmpty` |

```csharp
foreach (var (origin, count) in products.CountBy(p => p.Origin))
    Console.WriteLine($"{origin}：{count}");

foreach (var (i, p) in products.Index())
    Console.WriteLine($"{i + 1}. {p.Name}");
```

<!--
補充一下最近兩版 .NET 新增的 LINQ 方法。

CountBy 可以直接依某個欄位計算筆數，比 GroupBy 再 Count 簡潔很多。Index 可以在 foreach 的時候同時拿到索引，不用再改寫成 for 迴圈。.NET 10 還新增了 LeftJoin 和 RightJoin，以前寫外部連接要用 GroupJoin 加 DefaultIfEmpty，非常難讀，現在一個方法就搞定。

這些方法大家知道有就好，舊的寫法網路上還是很常見。
-->

---

# 使用聚合方法的注意事項

| 注意事項 | 說明 |
| --- | --- |
| **之一：空集合的 Max / Average 會拋例外** | 可以先用 `Any()` 判斷，或用 `DefaultIfEmpty()` |
| **之二：判斷存在用 Any，不要用 Count() > 0** | `Any` 找到一筆就停止 |
| **之三：FirstOrDefault 要處理 null** | 回傳值可能是 `null`，使用前先判斷 |

<!--
聚合方法的注意事項有三個。

之一，空集合呼叫 Max 或 Average 會拋出例外，因為沒有資料可以算，要先判斷。之二，判斷存在用 Any。之三，FirstOrDefault 的回傳值一定要處理 null 的情況。
-->

---
layout: default
---

# 練習 4：商品統計報表
### 任務說明

1. 查詢 Id 為 3 的商品名稱，找不到則印出「查無此商品」
2. 判斷是否有價格超過 1,000 元的商品
3. 計算所有有庫存商品的平均價格
4. 依 `CategoryId` 分組，印出每個分類的商品數量與庫存總數

<!--
【練習目的】
練習 FirstOrDefault、Any、Average、GroupBy 的組合使用。

【解題引導】
第 4 題 GroupBy 之後，Select 裡可以用 g.Sum(p => p.Stock) 計算庫存總數。
-->

---
layout: default
---

# 練習 4：解題提示

```csharp
var found = products.FirstOrDefault(p => p.Id == 3);
Console.WriteLine(found?.Name ?? "查無此商品");             // 薇拉

Console.WriteLine(products.Any(p => p.Price > 1000));       // True

var avg = products.Where(p => p.Stock > 0).Average(p => p.Price);
Console.WriteLine($"平均價格：{avg:N0}");

var report = products.GroupBy(p => p.CategoryId)
    .Select(g => new { CategoryId = g.Key, Count = g.Count(), Stock = g.Sum(p => p.Stock) });
foreach (var r in report)
    Console.WriteLine($"分類 {r.CategoryId}：{r.Count} 款，庫存 {r.Stock}");
```

<!--
分類 1 有四款、庫存 65；分類 2 有一款、庫存 5；分類 3 有一款、庫存 50。

注意第三題要先 Where 篩選有庫存的，再計算平均。
-->

---
layout: section
class: flex flex-col justify-center items-center text-center
---

# 3-5 延遲執行與立即執行
### Deferred Execution vs ToList

<!--
第五個小節是一個非常重要的觀念：延遲執行。這個觀念沒搞懂，之後查資料庫很容易出問題。
-->

---

# 什麼是延遲執行（Deferred Execution）？

想像我們在咖啡店寫一張「點餐單」：

| 動作 | 咖啡店 | LINQ |
| --- | --- | --- |
| 寫點餐單 | 只是寫下「要一杯拿鐵」，還沒開始做 | `var q = products.Where(...)` |
| 交給咖啡師 | 真的開始做咖啡 | `foreach`、`ToList()`、`Count()` |

「LINQ 查詢在宣告的時候**不會執行**，直到我們真正**使用結果**時才會執行，這就是延遲執行。」

<!--
我們用點餐單來理解延遲執行。

寫下 products.Where 的時候，就像在點餐單上寫「要一杯拿鐵」，咖啡還沒開始做。只有當我們真正需要結果的時候，例如用 foreach 走訪、呼叫 ToList 或 Count，點餐單才會交給咖啡師，開始真正執行查詢。

「LINQ 查詢在宣告的時候不會執行，直到真正使用結果時才會執行」，這就是延遲執行。
-->

---

# 在 C# 中觀察延遲執行

```csharp
List<int> stocks = [10, 0, 5];
var inStock = stocks.Where(s =>
{
    Console.WriteLine($"  檢查 {s}");
    return s > 0;
});

Console.WriteLine("① 查詢已宣告");
stocks.Add(8);                                    // 宣告之後才加入資料
Console.WriteLine("② 開始走訪");
foreach (var s in inStock) Console.WriteLine($"→ {s}");
```

```text
① 查詢已宣告
② 開始走訪
  檢查 10
→ 10
  檢查 0
  檢查 5
→ 5
  檢查 8
→ 8
```

<!--
我們在 Where 的 Lambda 裡印出「檢查」，觀察查詢到底什麼時候被執行。

結果很有趣：「查詢已宣告」印出來的時候，一次「檢查」都沒有，代表 Where 根本還沒執行。直到 foreach 開始走訪，才一筆一筆檢查。

更有趣的是，宣告查詢之後才加入的 8，也出現在結果裡。因為查詢是在 foreach 的時候才執行，那時候 8 已經在集合裡了。
-->

---

# 立即執行：ToList、ToArray 與聚合方法

| 類型 | 方法 | 何時執行 |
| --- | --- | --- |
| **延遲執行** | `Where`、`Select`、`OrderBy`、`Skip`、`Take`、`GroupBy` | 使用結果時 |
| **立即執行** | `ToList()`、`ToArray()`、`ToDictionary()` | 呼叫的當下 |
| **立即執行** | `Count()`、`Sum()`、`First()`、`Any()`、`Max()` | 呼叫的當下 |

```csharp
var snapshot = stocks.Where(s => s > 0).ToList();   // 立刻執行，結果存成 List
stocks.Add(100);
Console.WriteLine(snapshot.Count);                  // 不會包含 100
```

<!--
那如果我們希望查詢「現在就執行」，並把結果固定下來呢？就呼叫 ToList。

規則很簡單：回傳序列的方法，像 Where、Select、OrderBy，都是延遲執行；回傳具體結果的方法，像 ToList、Count、Sum、First，都是立即執行。

ToList 之後，結果就像拍了一張快照，之後原本的集合再怎麼改，都不會影響這個 List。
-->

---

# 使用延遲執行的注意事項

| 注意事項 | 說明 |
| --- | --- |
| **之一：重複走訪會重複執行** | 同一個查詢 `foreach` 兩次，就會執行兩次；查資料庫就是查兩次 |
| **之二：要重複使用結果就 ToList()** | 把結果固定下來，避免重複查詢 |
| **之三：不要太早 ToList()** | 先 `ToList()` 再 `Where`，會把全部資料載入記憶體才篩選 |

<div class="mt-4 p-3 bg-blue-50 border-l-4 border-blue-400 text-gray-700 text-sm text-left">
💡 <b>口訣：</b>「條件先串完，最後再 ToList()」。
</div>

<!--
延遲執行有三個注意事項。

之一，同一個查詢走訪兩次，就會執行兩次。在 List 上可能還好，但如果是查資料庫，就是對資料庫發了兩次 SQL。

之二，所以如果結果要重複使用，就先 ToList 固定下來。

之三，但也不能太早 ToList。如果先 ToList 再 Where，就會把整張表的資料全部載入記憶體，然後才在 C# 裡篩選，資料量大的時候會非常慢。

口訣是：條件先串完，最後再 ToList。這個口訣下一節會解釋得更清楚。
-->

---
layout: default
---

# 練習 5：預測輸出
### 任務說明

不執行程式，先預測下列程式碼的輸出，再實際執行驗證：

```csharp
List<string> cart = ["Latte", "Mocha"];
var q = cart.Where(c => c.Length > 4);
var list = q.ToList();
cart.Add("Cold Brew");

Console.WriteLine(q.Count());
Console.WriteLine(list.Count);
```

<!--
【練習目的】
確認大家理解延遲執行與 ToList 的差異。

【解題引導】
q 是查詢還是結果？list 是查詢還是結果？Cold Brew 加入之後，誰會看到它？
-->

---
layout: default
---

# 練習 5：解析

```text
3
2
```

| 變數 | 本質 | 說明 |
| --- | --- | --- |
| `q` | 查詢（點餐單） | `Count()` 時才執行，看得到後來加入的 `Cold Brew` |
| `list` | 結果（快照） | `ToList()` 當下就執行完畢，只有 Latte、Mocha |

<!--
答案是 3 和 2。

q 只是一張點餐單，呼叫 Count 的時候才執行，這時 Cold Brew 已經加進去了，而且長度大於 4，所以是 3 筆。list 則是在 ToList 的當下就執行完了，只有兩筆。
-->

---
layout: section
class: flex flex-col justify-center items-center text-center
---

# 3-6 IEnumerable vs IQueryable
### LINQ to Objects vs LINQ to Entities

<!--
最後一個小節，我們來看 IEnumerable 和 IQueryable 的差別。這是面試很常考、實務上也很容易踩雷的觀念。
-->

---

# 什麼是 IEnumerable 與 IQueryable？

| | `IEnumerable<T>` | `IQueryable<T>` |
| --- | --- | --- |
| 資料在哪裡 | 記憶體（`List`、陣列） | 外部資料來源（資料庫） |
| 條件怎麼執行 | C# 在記憶體中逐筆判斷 | 翻譯成 **SQL**，交給資料庫執行 |
| Lambda 的型別 | `Func<T, bool>`（委派） | `Expression<Func<T, bool>>`（運算式樹） |
| 典型來源 | `products.Where(...)` | `db.Products.Where(...)`（EF Core） |

「IQueryable 會把 LINQ 查詢**翻譯成 SQL**，讓資料庫只回傳需要的資料。」

<!--
IEnumerable 和 IQueryable 都能用 LINQ 查詢，寫法一模一樣，但背後的執行方式完全不同。

IEnumerable 是在記憶體裡查，C# 會一筆一筆跑 Lambda 判斷。IQueryable 則是把整個查詢「翻譯成 SQL」，送到資料庫執行，資料庫只回傳符合條件的資料。

IQueryable 之所以能翻譯，是因為它接收的不是委派，而是 Expression，也就是運算式樹。運算式樹把 Lambda 保存成一個「可以被分析的資料結構」，EF Core 就能讀懂 p.Price 小於 500 的意思，把它轉成 WHERE Price < 500。
-->

---

# 比喻：在倉庫挑貨 vs 請倉庫出貨

| 做法 | 比喻 | 結果 |
| --- | --- | --- |
| `IEnumerable` | 把整個倉庫的貨都搬回店裡，再自己挑 | 搬運量大、很慢 |
| `IQueryable` | 打電話告訴倉庫要什麼，倉庫只送需要的過來 | 只搬需要的 |

```csharp
// ❌ 先把整張表載入記憶體，再篩選（IEnumerable）
var bad = db.Products.ToList().Where(p => p.Price < 500);

// ✅ 條件翻譯成 SQL，資料庫只回傳符合的資料（IQueryable）
var good = db.Products.Where(p => p.Price < 500).ToList();
```

<!--
用倉庫來比喻就很清楚了。

IEnumerable 的做法，就像把整個倉庫的貨全部搬回店裡，再自己一箱一箱挑，資料量一大就非常慢。IQueryable 則是打電話告訴倉庫要什麼，倉庫只把需要的送過來。

看下面兩行程式碼，差別只在 ToList 的位置。第一行先 ToList，整張商品表都被載入記憶體，然後才篩選；第二行先 Where 再 ToList，條件會被翻譯成 SQL，資料庫只回傳便宜的商品。

如果商品表有一百萬筆，第一行會讓網站當掉，這就是上一節說「不要太早 ToList」的原因。
-->

---

# IQueryable 產生的 SQL

```csharp
var result = db.Products
    .Where(p => p.Price < 500 && p.Stock > 0)
    .OrderBy(p => p.Price)
    .Select(p => new { p.Name, p.Price })
    .Take(3)
    .ToList();
```

EF Core 會翻譯成類似這樣的 SQL：

```sql
SELECT TOP(3) [p].[Name], [p].[Price]
FROM [Products] AS [p]
WHERE [p].[Price] < 500.0 AND [p].[Stock] > 0
ORDER BY [p].[Price]
```

<!--
這是第五章之後會看到的 EF Core 查詢，大家先看一下 IQueryable 翻譯出來的 SQL。

Where 變成 WHERE，OrderBy 變成 ORDER BY，Select 只取 Name 和 Price 兩個欄位，Take 3 變成 TOP 3。整個查詢在資料庫裡一次完成，只回傳三筆、兩個欄位的資料。

這就是 LINQ to Entities 的威力：我們寫的是 C#，實際執行的是最佳化過的 SQL。
-->

---

# 使用 IQueryable 的注意事項

| 注意事項 | 說明 |
| --- | --- |
| **之一：不是所有 C# 都能翻譯成 SQL** | 呼叫自己寫的方法，EF Core 會拋出「could not be translated」例外 |
| **之二：回傳型別宣告成 IEnumerable 會失去翻譯能力** | 後續串接的 `Where` 會變成在記憶體中執行 |
| **之三：ToList() 是分界點** | 之前在資料庫執行，之後在記憶體執行 |

```csharp
// ❌ 自訂方法無法翻譯成 SQL
db.Products.Where(p => IsPopular(p)).ToList();
```

<!--
IQueryable 有三個注意事項。

之一，不是所有 C# 程式碼都能翻譯成 SQL。如果在 Where 裡面呼叫自己寫的方法，資料庫根本不知道那是什麼，EF Core 會拋出無法翻譯的例外。

之二，如果把 IQueryable 指派給 IEnumerable 型別的變數，後面串接的方法就會變成在記憶體裡執行。第七章寫 Repository 的時候，我們會特別注意回傳型別。

之三，ToList 就是分界點：之前的部分在資料庫執行，之後的部分在記憶體執行。
-->

---
layout: default
---

# 練習 6：IEnumerable vs IQueryable
### 認證模擬題（單選）

商品表有 100 萬筆資料，下列哪一種寫法**效能最好**？

A. `db.Products.ToList().Where(p => p.Stock > 0).Take(10)`
B. `db.Products.Where(p => p.Stock > 0).ToList().Take(10)`
C. `db.Products.Where(p => p.Stock > 0).Take(10).ToList()`
D. `db.Products.AsEnumerable().Where(p => p.Stock > 0).Take(10).ToList()`

<!--
【出題動機】
把「ToList 是分界點」這個觀念用實際情境檢驗。

【解題引導】
每個選項的 ToList 或 AsEnumerable 在哪裡？在它之前的部分會被翻譯成 SQL，之後的部分在記憶體執行。
-->

---
layout: default
---

# 練習 6：解析

**正確答案：C**

| 選項 | 資料庫回傳筆數 | 說明 |
| --- | --- | --- |
| A ❌ | 100 萬筆 | 整張表載入記憶體才篩選 |
| B ❌ | 所有有庫存的 | `Take(10)` 在記憶體執行 |
| C ✅ | 10 筆 | `WHERE` 與 `TOP(10)` 都在資料庫執行 |
| D ❌ | 100 萬筆 | `AsEnumerable()` 之後就變成 LINQ to Objects |

<!--
答案是 C。只有 C 把 Where 和 Take 都放在 ToList 之前，資料庫只回傳 10 筆。

D 的 AsEnumerable 大家可能沒看過，它的作用就是把 IQueryable 轉成 IEnumerable，之後的查詢都在記憶體執行，效果跟 A 一樣。
-->

---
layout: default
---

# 綜合練習：咖啡豆商店查詢服務
### 任務說明

撰寫一個 `ProductQuery` 類別，提供以下方法（資料來源為 `List<Product>`）：

1. `Search(string? keyword, decimal? maxPrice)`：可選的關鍵字與價格上限，回傳有庫存的商品
2. `GetById(int id)`：找不到回傳 `null`
3. `GetTop(int n)`：價格最高的前 n 筆
4. `GetReport()`：依產地分組，回傳產地、款數、平均價格的 record 清單
5. 所有方法都要在**最後**才呼叫 `ToList()`

<!--
【練習目的】
把本章的 Lambda、Where、OrderBy、FirstOrDefault、GroupBy、ToList 全部整合到一個類別中。這個類別的結構，就是第七章 Repository 的雛形。

【解題引導】
Search 的參數是 nullable，有值才串接 Where。GetReport 要定義一個 record 作為回傳型別，不要回傳匿名型別。
-->

---
layout: default
---

# 綜合練習：解題提示

```csharp
public class ProductQuery(List<Product> products)
{
    public List<Product> Search(string? keyword, decimal? maxPrice)
    {
        var q = products.Where(p => p.Stock > 0);
        if (!string.IsNullOrWhiteSpace(keyword)) q = q.Where(p => p.Name.Contains(keyword));
        if (maxPrice is not null) q = q.Where(p => p.Price <= maxPrice);
        return q.ToList();
    }
    public Product? GetById(int id) => products.FirstOrDefault(p => p.Id == id);
    public List<Product> GetTop(int n) => products.OrderByDescending(p => p.Price).Take(n).ToList();
    public List<OriginReport> GetReport() => products.GroupBy(p => p.Origin)
        .Select(g => new OriginReport(g.Key, g.Count(), g.Average(p => p.Price))).ToList();
}
public record OriginReport(string Origin, int Count, decimal AvgPrice);
```

<!--
這個類別用了上一章學的 primary constructor，把商品清單當作建構子參數傳入。

Search 用動態組合查詢，GetById 用 FirstOrDefault，回傳型別是 Product?，明確表示可能是 null。GetReport 回傳 record 清單。

大家先記住這個類別的樣子，第七章我們會把資料來源換成資料庫，然後把它改寫成泛型 Repository。
-->

---

# 總結

| 小節 | 重點 |
| --- | --- |
| 3-1 委派與 Lambda | `Func` / `Action` 讓方法可以當參數；`p => p.Price < 500` |
| 3-2 兩種語法 | 查詢語法像 SQL；**方法語法是主流** |
| 3-3 篩選與轉換 | `Where` 篩選、`Select` 轉換形狀、`OrderBy` + `ThenBy` 排序、`Skip` / `Take` 分頁 |
| 3-4 聚合與單筆 | `FirstOrDefault` 找不到回傳 null；`Any` 判斷存在；`GroupBy` 分組統計 |
| 3-5 延遲執行 | 查詢在使用結果時才執行；「條件先串完，最後再 ToList()」 |
| 3-6 IQueryable | 翻譯成 SQL 在資料庫執行；`ToList()` 是資料庫與記憶體的分界點 |

下一章我們會介紹 **MVC 基本觀念**，看看 ASP.NET Core 網站是怎麼分工合作的。

<!--
我們來總結這一章。

Lambda 讓我們可以把條件當作參數傳遞，這是 LINQ 的基礎。LINQ 有兩種寫法，方法語法是主流。最常用的方法是 Where、Select、OrderBy、FirstOrDefault、Any、Count 和 GroupBy。最後兩個觀念最重要：LINQ 是延遲執行的，而 IQueryable 會把查詢翻譯成 SQL，所以要記得「條件先串完，最後再 ToList」。

學完這一章，大家已經具備查詢資料庫的能力了，只差把資料來源從 List 換成資料庫。

下一章我們會介紹 MVC 基本觀念，看看 Model、View、Controller 三個角色是怎麼分工合作，完成一個網頁的。
-->

---
layout: end
---

# 第 3 章結束
### 下一章：MVC 基本觀念
