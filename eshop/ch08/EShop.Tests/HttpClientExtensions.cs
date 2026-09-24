using System.Net;

namespace EShop.Tests;

public static class HttpClientExtensions
{
    // Razor 預設會把中文編碼成 &#x...;，比對前先解碼
    public static async Task<string> GetHtmlAsync(this HttpClient client, string url)
    {
        var html = await client.GetStringAsync(url);
        return WebUtility.HtmlDecode(html);
    }
}
