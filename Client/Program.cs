using Client.Services;
using Gateway_API_Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(60);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});
//Auto mapper
builder.Services.AddAutoMapper(o =>
{
    o.CreateMap<CarsDTO, CreateCarsDTO>().ReverseMap();
    o.CreateMap<UpdateDTO, CarsDTO>().ReverseMap();
});


//authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
{
    option.Cookie.HttpOnly = true;
    option.ExpireTimeSpan = TimeSpan.FromMinutes(50);
    option.SlidingExpiration = true;
    option.LoginPath = "/auth/login";
    option.AccessDeniedPath = "/auth/accessdenied";
});


//set base addresss for all the Http clients in this application
//CarGallaryAPI => CreateClient in BaseService
Console.WriteLine("CarsAPI from config = " + builder.Configuration["ServiceURL:CarsAPI"]);
builder.Services.AddHttpClient("CarGallaryAPI", client =>
{
    var carAPIUrl= builder.Configuration.GetValue<string>("ServiceURL:CarsAPI");
    client.BaseAddress=new Uri(carAPIUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});


//register the service
builder.Services.AddScoped<ICarServices, CarService>();
builder.Services.AddScoped<IAuthService, AuthService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
