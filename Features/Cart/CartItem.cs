namespace Cart.Features.Cart;

public class CartItem
{
    public Product Product { get; set; }
    public int Count { get; set; }

    public CartItem(Product product, int count)
    {
        Product = product;
        Count = count;
    }
}