using System.Net.Http.Json;
using System.Text.Json;

namespace EShop.Tests;

public class AdminProductApiTests(EShopWebFactory factory) : IClassFixture<EShopWebFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAll_回傳DataTables格式()
    {
        var json = await _client.GetFromJsonAsync<JsonElement>("/Admin/Product/GetAll");

        var data = json.GetProperty("data");
        Assert.Equal(6, data.GetArrayLength());
        Assert.Equal("單品咖啡豆", data[0].GetProperty("category").GetString());
    }

    [Fact]
    public async Task Delete_刪除後列表少一筆()
    {
        var response = await _client.DeleteAsync("/Admin/Product/Delete/6");
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        var html = await _client.GetHtmlAsync("/products?categoryId=3");

        Assert.True(result.GetProperty("success").GetBoolean());
        Assert.DoesNotContain("綜合配方豆", html);
    }
}
