using Cart.Database;

namespace Cart.Features.Cart;

public class CartService
{
    public void Add(int id, List<CartItem> items)
    {
        IProductDbService db = AppState.App.Services.GetRequiredService<IProductDbService>();
        Product? product = db.Get(id);

        if (product == null)
        {
            throw new Exception($"Product by id {id} not found");
        }
        
        CartItem? item = items.FirstOrDefault(i => i.Product.Id == product.Id);
        if (item != null)
        {
            item.Count++;
        }
        else
        {
            items.Add(new CartItem(product, 1));
        }
    }

    public CartItem? Get(int id, List<CartItem> items)
    {
        return items.FirstOrDefault(i => i.Product.Id == id);
    }

    public void Remove(CartItem item, List<CartItem> items)
    {
        CartItem? foundItem = items.FirstOrDefault(i => i.Product.Id == item.Product.Id);
        if (foundItem == null) return;

        if (foundItem.Count > 1) foundItem.Count--;
        else
        {
            Delete(foundItem, items);
        }
    }

    public void Delete(CartItem item, List<CartItem> items)
    {
        items.Remove(item);
    }

    public void RemoveById(int id, List<CartItem> items)
    {
        CartItem? foundItem = items.FirstOrDefault(i => i.Product.Id == id);
        if (foundItem == null) return;

        if (foundItem.Count > 1) foundItem.Count--;
        else
        {
            Delete(foundItem, items);
        }
    }

    public void Clear(List<CartItem> items)
    {
        items.Clear();
    }

    public CheckoutInfo Checkout(List<CartItem> items)
    {
        int itemsCount = items.Count;
        decimal total = items.Aggregate(0m, (i, item) => i += item.Product.Price);
        return new CheckoutInfo(itemsCount, total);
    }
}