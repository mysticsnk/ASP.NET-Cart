using Cart.Features.Cart;

namespace Cart.Database;

public class SimpleProductDbService : IProductDbService
{
    private static int Counter; 
    public List<Product> Products { get; set; } = new List<Product>();

    public void Add(ProductDTO dto)
    {
        Products.Add(new Product(++Counter, dto.Name, dto.Price));
    }

    public void AddAll(List<ProductDTO> productDtos)
    {
        foreach (ProductDTO dto in productDtos)
        {
            Add(dto);
        }
    }

    public void Remove(Product p)
    {
        Products.Remove(p);
    }

    public void RemoveById(int id)
    {
        Products = Products.Where(p => p.Id != id).ToList();
    }

    public void Update(int id, ProductDTO dto)
    {
        Product? foundProduct = Products.FirstOrDefault(p => p.Id == id);
        if (foundProduct != null)
        {
            foundProduct.Name = dto.Name;
            foundProduct.Price = dto.Price;
        }
    }

    public void UpdateAll(List<Product> products)
    {
        Products = products;
    }

    public Product? Get(int id)
    {
        return Products.FirstOrDefault(p => p.Id == id);
    }

    public List<Product> GetAll(int limit)
    {
        return Products;
    }
}