using System.Text.Json;

namespace Cart.Features.Cart;
using Services;
public static class CartEndpoints
{
    public static RouteGroupBuilder AddCartEndpoints(this RouteGroupBuilder builder)
    {
        var cart = builder.MapGroup("/cart");
        cart.MapGet("/", GetAll);
        cart.MapPost("/add/{productId}", Add);
        cart.MapGet("/{productId}", Get);
        
        cart.MapDelete("/clear", Clear);
        return cart;
    }

    public static IResult Add(HttpContext context, int productId)
    {
        ILogger logger = AppState.App.Services.GetService<ILogger>()!;
        CartService cart = AppState.App.Services.GetService<CartService>()!;

        List<CartItem> items = LoadSession(context) ?? new List<CartItem>();
        
        cart.Add(productId, items);
        logger.Log($"Created an item with Id: {productId}");
        SaveSession(context, items);
        return Results.Created($"/{productId}",cart.Get(productId, items));
    }

    public static IResult Get(HttpContext context, int productId)
    {
        ILogger logger = AppState.App.Services.GetService<ILogger>()!;
        CartService cart = AppState.App.Services.GetService<CartService>()!;
        
        List<CartItem> items = LoadSession(context) ?? new List<CartItem>();
        
        CartItem? item = cart.Get(productId, items);
        if (item == null)
        {
            logger.Log($"Item with id {productId} not found");
            return Results.NotFound();
        }
        logger.Log($"Item with id {productId} found!");
        return Results.Ok(item);
    }
    
    public static IResult GetAll(HttpContext context)
    {
        ILogger logger = AppState.App.Services.GetService<ILogger>()!;
        CartService cart = AppState.App.Services.GetRequiredService<CartService>();
        
        List<CartItem> items = LoadSession(context) ?? new List<CartItem>();
        
        logger.Log("Successfully got all items");
        return Results.Ok(items);
    }

    public static IResult Clear(HttpContext context)
    {
        ILogger logger = AppState.App.Services.GetService<ILogger>()!;
        CartService cart = AppState.App.Services.GetService<CartService>()!;
        
        List<CartItem> items = LoadSession(context) ?? new List<CartItem>();
        
        cart.Clear(items);
        SaveSession(context, new List<CartItem>());
        logger.Log("Successfully cleared the cart");
        return Results.Ok();
    }

    public static void SaveSession(HttpContext context, List<CartItem> items)
    {
        context.Session.SetAsJson("CartItems", items);

        Console.WriteLine("SAVED:");
        Console.WriteLine(JsonSerializer.Serialize(items));
    }

    public static List<CartItem>? LoadSession(HttpContext context)
    {
        var items = context.Session.GetFromJson<List<CartItem>>("CartItems");

        Console.WriteLine("LOADED:");
        Console.WriteLine(JsonSerializer.Serialize(items));

        return items;
    }
}