using Application.Commands.Products;
using Application.Mappings;
using Asp.Versioning;
using Domain.Interfaces.Products;
using Infrastructure.Contexts;
using Infrastructure.Repositories.Products;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region Versioning

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"));
    //new HeaderApiVersionReader("X-Version"),
    //new MediaTypeApiVersionReader("v"));
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

#endregion

#region JWT Authentication

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            }
        };
    });

#endregion

#region Configuring Swagger/OpenAPI 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    // Generate Swagger docs for different versions
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "CleanArchitecture.API.Endpoint", Version = "v1" });
    options.SwaggerDoc("v2", new OpenApiInfo { Title = "CleanArchitecture.API.Endpoint", Version = "v2" });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\n\nExample: \"Bearer abc123def456\""
    });

    // Set Security Requirements for JWT in Swagger
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>() // Empty string array means no specific scopes are required
        }
    });
});

#endregion

// Database Connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"))
);

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfiles));

// Register Repository
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Register CQRS
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateProductCommandHandler).Assembly));

// Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "SampleInstance";
});

builder.Services.AddControllers();

var app = builder.Build();

#region Seed Product Data

//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//    context.Database.Migrate(); // Apply pending migrations

//    if (!context.Products.Any()) // Check if data exists
//    {
//        context.Products.AddRange(
//            new Product
//            {
//                Name = "IPhone 14 Pro Max",
//                Price = 130000
//            },
//             new Product
//             {
//                 Name = "IPhone 15 Pro Max",
//                 Price = 140000
//             });

//        context.SaveChanges();
//    }
//}

#endregion

// Configure the HTTP request pipeline.
app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "API v2");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
