using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionSvcHttpClient>();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

try
{
  await DbInitializer.Init(app);
}
catch (Exception e)
{
  Console.WriteLine(e.Message);
}

app.Run();