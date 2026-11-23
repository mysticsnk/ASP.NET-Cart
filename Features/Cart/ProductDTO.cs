namespace Cart.Features.Cart;

public class ProductDTO
{
    public string Name { get; set; }
    public decimal Price { get; set; }

    public ProductDTO(string name, decimal price)
    {
        Name = name;
        Price = price;
    }
}