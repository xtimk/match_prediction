using MatchPrediction.Data;
using MatchPrediction.Data.Contexts;
using MatchPrediction.Helpers.CsvHelper;
using MatchPrediction.Helpers.CsvHelper.Impl;
using MatchPrediction.Helpers.DataTables;
using MatchPrediction.Managers.PredictionManagers.PoissonExactResult;
using MatchPrediction.Managers.PredictionManagers.PoissonExactResult.Readers;
using MatchPrediction.Services.MatchPredictionServices.ExactResult;
using MatchPrediction.Services.MatchPredictionServices.ExactResult.Impl;
using MatchPrediction.Services.MatchStatsGetterService;
using MatchPrediction.Services.MatchStatsGetterService.Impl;
using MatchPrediction.Services.QueryServices;
using MathNet.Numerics;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration));

// Utilities
builder.Services.AddScoped(typeof(ICsvHelper<>), typeof(CsvHelperLib<>));
builder.Services.AddScoped<QueryService>();
builder.Services.AddScoped(typeof(DataTablesService<>));

// Db Initializer services
builder.Services.AddScoped<IMatchStatsGetterService, FootballDataCoUkGetter>();
builder.Services.AddTransient<DbInitializer>();

// Predictions
builder.Services.AddScoped<IMatchExactResultService, MatchExactResultService>();

// Readers
builder.Services.AddScoped<PredictionResponse_PoissonExactResult_TeamWinner>();
builder.Services.AddScoped<PredictionResponse_PoissonExactResult_BothTeamsToScore>();
builder.Services.AddScoped<PredictionResponse_PoissonExactResult_ExactResult>();

// Manager
builder.Services.AddScoped<PredictionResponseReaderManager>();


// Prediction testers
builder.Services.AddScoped<IMatchExactResultTesterService, MatchExactResultTesterService>();

builder.Services.AddSingleton<IServiceProvider>(sp => sp);
builder.Services.AddHttpClient();
builder.Services.AddDbContext<MatchPredictionContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});

var app = builder.Build();

// Eventually auto-apply migrations
await MigrateDatabase(app);
// Seed database if empty
await SeedDatabase(app);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

async static Task MigrateDatabase(WebApplication app)
{
    var scope = app.Services.CreateAsyncScope();
    var context = scope.ServiceProvider.GetRequiredService<MatchPredictionContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error has been encountered when applying migration(s).");
    }
    finally
    {
        await scope.DisposeAsync();
    }
}

async Task SeedDatabase(WebApplication app)
{
    var scope = app.Services.CreateAsyncScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbinitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();

    try
    {
        await dbinitializer.Initialize();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error has been encountered when seeding database.");
    }
    finally
    {
        await scope.DisposeAsync();
    }
}