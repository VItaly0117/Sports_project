using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web_site_analytic_sports.Data;
using Web_site_analytic_sports.Services;

var builder = WebApplication.CreateBuilder(args);

// Логирование строки подключения для отладки
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection String: {connectionString}");

// Конфигурация сервисов
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Регистрация сервисов
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

var app = builder.Build();

// Конвейер обработки запросов
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Инициализация базы данных с улучшенной обработкой ошибок
try
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Testing database connection...");
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Проверка подключения к базе
        if (await context.Database.CanConnectAsync())
        {
            logger.LogInformation("Database connection successful");

            // Применение миграций
            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Migrations applied successfully");

            // Заполнение начальными данными
            logger.LogInformation("Seeding initial data...");
            var dataService = services.GetRequiredService<IDataService>();
            await dataService.SeedWorldCupDataAsync(1998);
            logger.LogInformation("Data seeding completed");
        }
        else
        {
            logger.LogError("Database connection failed");

            // Попытка создания базы, если не существует
            logger.LogWarning("Attempting to create database...");
            await context.Database.EnsureCreatedAsync();
            logger.LogWarning("Database created (EnsureCreated)");
        }
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Database initialization failed");
}

await app.RunAsync();