namespace EShop.Web.Models;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int DisplayOrder { get; set; }
}
