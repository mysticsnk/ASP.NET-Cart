using System.Diagnostics.Metrics;
using Cart.Features.Cart;

namespace Cart.Database;

public interface IProductDbService
{
    public void Add(ProductDTO dto);
    public void AddAll(List<ProductDTO> productDtos);

    public void Remove(Product p);

    public void RemoveById(int id);

    public void Update(int id, ProductDTO dto);

    public void UpdateAll(List<Product> products);
    
    public Product? Get(int id);

    public List<Product> GetAll(int limit);
}