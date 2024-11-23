using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MvcFlowers.Data;
using MvcFlowers.Models;

var builder = WebApplication.CreateBuilder(args);

// Настройка контекста базы данных
builder.Services.AddDbContext<MvcFlowersContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MvcFlowersContext") ??
    throw new InvalidOperationException("Connection string 'MvcFlowersContext' not found.")));

// Добавление сервисов для контроллеров API
builder.Services.AddControllers(); // Измените на AddControllers(), чтобы использовать только API

var app = builder.Build();

//// Инициализация данных
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    SeedDataMF.Initialize(services);
//}

// Настройка HTTP-запросов
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error"); // Обработка ошибок
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Настройка маршрутизации для API
app.MapControllers(); // Используйте MapControllers() для маршрутизации API

// Добавление маршрута для стартовой страницы
app.MapGet("/", () => "Welcome to the shop!");

app.Run();
