using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Order.Api.Middleware;
using Order.Application.Catalog.Interfaces;
using Order.Application.Events;
using Order.Application.Orders.Commands.AddOrderItem;
using Order.Application.Orders.Commands.CreateOrder;
using Order.Application.Orders.Queries.GetOrderById;
using Order.Infrastructure;
using Order.Infrastructure.Catalog;
using Order.Infrastructure.Messaging;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Controllers REST
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

// Infrastructure : DbContext et IProductRepository
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<CreateOrderCommandHandler>();
builder.Services.AddScoped<AddOrderItemCommandHandler>();
builder.Services.AddScoped<GetOrderByIdQueryHandler>();
builder.Services.AddScoped<ICatalogServiceClient,CatalogServiceClient>();


//JWT
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = builder.Configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT key is missing.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key)),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

//only if RabbitMQ
//builder.Services.AddScoped<IOrderEventPublisher, RabbitMqOrderEventPublisher>();




//call catalog service from order service
builder.Services.AddHttpClient<ICatalogClient, CatalogClient>(client =>
{
    var baseUrl = builder.Configuration["Services:Catalog:BaseUrl"]
        ?? throw new InvalidOperationException(
            "CatalogService BaseUrl is not configured.");

    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddHttpClient<ICatalogServiceClient, CatalogServiceClient>(client =>
{
    var baseUrl = builder.Configuration["Services:Catalog:BaseUrl"]
        ?? throw new InvalidOperationException(
            "CatalogService BaseUrl is not configured.");

    client.BaseAddress = new Uri(baseUrl);
});



var app = builder.Build();

// Swagger uniquement en environnement Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();