using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SmartTab.Data;
using SmartTab.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<EmailService>();

builder.Services.AddHttpClient<IMonobankService, MonobankService>(client =>
{
    client.BaseAddress = new Uri("https://api.monobank.ua/");
    client.Timeout = TimeSpan.FromSeconds(15);
});

// External API Services
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<ICurrencyApiService, CurrencyApiService>(client =>
{
    client.BaseAddress = new Uri("https://bank.gov.ua/");
    client.Timeout = TimeSpan.FromSeconds(10);
}).AddStandardResilienceHandler();

builder.Services.AddHttpClient<ICountryApiService, CountryApiService>(client =>
{
    client.BaseAddress = new Uri("https://restcountries.com/");
    client.Timeout = TimeSpan.FromSeconds(10);
}).AddStandardResilienceHandler();

builder.Services.AddControllersWithViews();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartTab API v1");
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
