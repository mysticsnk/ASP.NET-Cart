using Cart;
using Cart.Database;
using Cart.Features.Cart;
using Cart.Services;
using ILogger = Cart.Services.ILogger;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ILogger, ConsoleLogger>();
builder.Services.AddSingleton<IProductDbService, SimpleProductDbService>();
builder.Services.AddSingleton<CartService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();
AppState.App = app;

IProductDbService catalog = app.Services.GetRequiredService<IProductDbService>();

catalog.AddAll(new List<ProductDTO>([
    new ProductDTO("Počitač", 3099m), // Комп'ютер
    new ProductDTO("Myš", 50m), // Миша
    new ProductDTO("Sluchátka", 100m), // Навушники
    new ProductDTO("Klávesnice", 80m), // Клавіатура
    new ProductDTO("Nabíječka", 30m) // Зарядка
]));

ILogger logger = app.Services.GetRequiredService<ILogger>();

app.UseSession();

app.MapGet("/", () => "Hello World!");

app.MapGet("/visits", (HttpContext context) =>
{
    int visits = context.Session.GetInt32("VisitsCount") ?? 0;

    visits++;
    
    context.Session.SetInt32("VisitsCount", visits);

    return $"Visits: {visits}";
});

app.MapGet("/identify", (HttpContext context) =>
{
    string? cookie = context.Request.Cookies["UserId"];

    if (cookie == null)
    {
        logger.Log("UserId cookie not found!");
        Guid userId = new Guid();
        context.Response.Cookies.Append("UserId", userId.ToString());
        logger.Log($"Generated a new UserId: {userId}");
        return Results.Ok(userId);
    }
    
    if (Guid.TryParse(cookie, out Guid guid))
    {
        logger.Log($"UserId cookie found: {guid}");
        return Results.Ok(guid);
    }
    
    logger.Log("Invalid UserId cookie format!");
    return Results.BadRequest("Invalid UserId cookie format!");
});

app.MapGet("/test-session", (HttpContext context) =>
{
    context.Session.SetString("Test", "Hello");
    return Results.Ok("Session initialized!");
});

var api = app.MapGroup("/api");
api.AddCartEndpoints();

app.Run();