using GLMS.Data;
using GLMS.Interfaces;
using GLMS.Repositories;
using GLMS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
var jwtAudience = builder.Configuration["JwtSettings:Audience"];
var jwtKey = builder.Configuration["JwtSettings:Key"];

// Protects API endpoints using JWT bearer tokens.
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
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey!))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Allows Swagger to test protected endpoints with a JWT.
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter the JWT token only. Do not type Bearer."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();

builder.Services.AddHttpClient<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IContractFactory, ContractFactory>();
builder.Services.AddScoped<IObserver, AuditLogger>();
builder.Services.AddScoped<ISubject, ContractSubject>();

var app = builder.Build();

// Creates the Docker SQL database on startup if needed.
using (var scope = app.Services.CreateScope())
{
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var connectionString = configuration.GetConnectionString("DefaultConnection");
    var sqlBuilder = new SqlConnectionStringBuilder(connectionString);

    var databaseName = sqlBuilder.InitialCatalog;
    sqlBuilder.InitialCatalog = "master";

    for (var attempt = 1; attempt <= 20; attempt++)
    {
        try
        {
            await using var masterConnection = new SqlConnection(sqlBuilder.ConnectionString);
            await masterConnection.OpenAsync();

            await using var createDatabaseCommand = masterConnection.CreateCommand();
            createDatabaseCommand.CommandText =
                $"""
                IF DB_ID(N'{databaseName}') IS NULL
                BEGIN
                    CREATE DATABASE [{databaseName}]
                END
                """;

            await createDatabaseCommand.ExecuteNonQueryAsync();

            await Task.Delay(TimeSpan.FromSeconds(2));

            await using var databaseConnection = new SqlConnection(connectionString);
            await databaseConnection.OpenAsync();

            await dbContext.Database.EnsureCreatedAsync();

            break;
        }
        catch (SqlException ex) when (ex.Number == 1801 && attempt < 20)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
        catch when (attempt < 20)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();