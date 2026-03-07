using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using WebApplication1.Data;
using WebApplication1.Services.AI;
using WebApplication1.Services.RedisManagement;
using WebApplication1.Services.ServiceDAO;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "WebApplication1:";
});



builder.Services.AddRazorPages();
builder.Services.AddDbContext<WebApplication1Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WebApplication1Context") ?? throw new InvalidOperationException("Connection string 'WebApplication1Context' not found.")));
builder.Services.AddScoped<ICartService, CartRedisService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<LlmService>();
builder.Services.AddScoped<LlmService>();
// Dans Program.cs
builder.Services.AddScoped<ProduitDAO>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
