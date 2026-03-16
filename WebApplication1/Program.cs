using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
<<<<<<< HEAD

using StackExchange.Redis;
using WebApplication1.Data;

using WebApplication1.Services.RedisManagement;
using WebApplication1.Services.ServiceDAO;

using WebApplication1.Services.AI.Embedding;
using WebApplication1.Services.AI.InitializationDBV;
using WebApplication1.Services.AI.LLM;
using WebApplication1.Services.AI.Reranking;
using WebApplication1.Services.AI.Retrieval;
using WebApplication1.Services.AI.SynchronizationDB;
using WebApplication1.Services.AI.VectorStore;
using WebApplication1.Services.AI.RagService;


=======
using WebApplication1.Data;
using WebApplication1.Services.AI;
using WebApplication1.Services.RedisManagement;
using WebApplication1.Services.ServiceDAO;

>>>>>>> b218cbdaaaed9a106850a8e85846553864058608
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
<<<<<<< HEAD
builder.Services.AddScoped<ProduitDAO>();


//// ********************************   Rag systeme build Interfaces ***********************************

builder.Services.AddHttpClient<IEmbeddingService, EmbeddingService>();

// builder vectore store
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379"));
builder.Services.AddSingleton<IVectorStore, RedisVectorStore>();

// builder Initialiser Vectoriel DB
builder.Services.AddScoped<IVectorDbInitializer, VectorDbInitializer>();

// builder SynchronizationDB
builder.Services.AddScoped<IAIIndexingService, AIIndexingService>();

// builder Product Service
builder.Services.AddScoped<IRetrievalService, ProductRetrievalService>();

// builder IRerankerService
builder.Services.AddScoped<IRerankerService, RerankerService>();

// build ILlmService
builder.Services.AddScoped<ILlmService, LlmService>();

// build orghestration 
builder.Services.AddScoped<IOrchestratorRagService, OrchestratorRagService>();

// ************************************************************************************************************


=======
// Dans Program.cs
builder.Services.AddScoped<ProduitDAO>();

>>>>>>> b218cbdaaaed9a106850a8e85846553864058608
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
<<<<<<< HEAD

=======
>>>>>>> b218cbdaaaed9a106850a8e85846553864058608
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

<<<<<<< HEAD
// Initialise vectoriel DB of product catalogue
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider
        .GetRequiredService<IVectorDbInitializer>();

    await initializer.InitializeAsync();
}

=======
>>>>>>> b218cbdaaaed9a106850a8e85846553864058608
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
<<<<<<< HEAD

=======
>>>>>>> b218cbdaaaed9a106850a8e85846553864058608
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
