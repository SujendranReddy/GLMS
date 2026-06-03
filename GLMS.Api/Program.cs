using GLMS.Data;
using GLMS.Interfaces;
using GLMS.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add API controller support
builder.Services.AddControllers();

// Add Swagger/OpenAPI documentation support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the API database context with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the contract repository for dependency injection
builder.Services.AddScoped<IContractRepository, ContractRepository>();

var app = builder.Build();

// Enable Swagger during development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();