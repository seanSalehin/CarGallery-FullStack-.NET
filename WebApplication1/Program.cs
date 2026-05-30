using System.Text;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Gateway_API_Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);


// ----------- Identity to API ---------------
//ApplicationUser model is inherited from IdentityUser
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();


// -------------------- JWT --------------------
var jwtSecret = builder.Configuration["JwtSettings:Secret"];
if (string.IsNullOrWhiteSpace(jwtSecret) || jwtSecret.Length < 32)
    throw new InvalidOperationException("JwtSettings:Secret must be at least 32 characters.");

var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ValidateIssuer = false,
            ValidateAudience = false,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };
    });

// -------------------- DB --------------------
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// -------------------- CORS --------------------
builder.Services.AddCors();

// -------------------- AutoMapper --------------------
builder.Services.AddAutoMapper(o =>
{
    o.CreateMap<Cars, CreateCarsDTO>().ReverseMap();
    o.CreateMap<Cars, UpdateDTO>().ReverseMap();
    o.CreateMap<Cars, CarsDTO>().ReverseMap();
    o.CreateMap<UpdateDTO, CarsDTO>().ReverseMap();
    o.CreateMap<ApplicationUser, UserDTO>().ReverseMap();
    o.CreateMap<Features, FeaturesUpdateDTO>().ReverseMap();
    o.CreateMap<Features, FeaturesCreateDTO>().ReverseMap();
    o.CreateMap<Features, FeaturesDTO>()
        .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Cars != null ? src.Cars.Name : null));
    o.CreateMap<FeaturesDTO, Features>().ReverseMap();
});

// -------------------- DI --------------------
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers();

// -------------------- API Versioning --------------------
builder.Services
    .AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";      
        options.SubstituteApiVersionInUrl = true;
    });

// -------------------- OpenAPI--------------------
builder.Services.AddOpenApi("v1", options =>
{
    options.ShouldInclude = apiDesc => apiDesc.GroupName == "v1";

    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "DEMO API",
            Version = "v1",
            Description = "Car Gallery API - Version 1",
            Contact = new OpenApiContact
            {
                Name = "Sean Salehin",
                Email = "Sean.Salehin@gmail.com"
            }
        };

        AddJwtBearerSecurity(document);
        return Task.CompletedTask;
    });
});

builder.Services.AddOpenApi("v2", options =>
{
    options.ShouldInclude = apiDesc => apiDesc.GroupName == "v2"; 

    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "DEMO API",
            Version = "v2",
            Description = "Car Gallery API - Version 2",
            Contact = new OpenApiContact
            {
                Name = "Sean Salehin",
                Email = "Sean.Salehin@gmail.com"
            }
        };
        AddJwtBearerSecurity(document);
        return Task.CompletedTask;
    });
});

static void AddJwtBearerSecurity(OpenApiDocument document)
{
    document.Components ??= new OpenApiComponents();
    document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();

    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header. Example: Bearer {token}"
    };

    document.SecurityRequirements ??= new List<OpenApiSecurityRequirement>();
    document.SecurityRequirements.Add(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
}


var app = builder.Build();

// -------------------- Migrate --------------------
await SeedDataAsync(app);

// -------------------- Scalar --------------------
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/{documentName}.json");

    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.MapScalarApiReference(options =>
    {
        options.Title = "DEMO - Car Gallery";

        var sorted = provider.ApiVersionDescriptions.OrderBy(d => d.ApiVersion).ToList();

        foreach (var description in sorted)
        {
            var versionName = description.GroupName;           
            var displayName = $"Demo API => {description.ApiVersion}";

            var isDefault = versionName == "v2";

            options.AddDocument(versionName, displayName, $"/openapi/{versionName}.json", isDefault: isDefault);
        }
    });
}

// -------------------- Middleware --------------------
app.UseCors(o => o.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("*"));

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task SeedDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
}
