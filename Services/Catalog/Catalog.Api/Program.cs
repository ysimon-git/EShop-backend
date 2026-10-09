using Catalog.Api.Middleware;
using Catalog.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Controllers REST
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// Infrastructure : DbContext et IProductRepository
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Swagger uniquement en environnement Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AngularFrontend");
app.MapControllers();
app.UseStaticFiles();

app.Run();