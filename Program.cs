using Microsoft.EntityFrameworkCore;
using KFH.Data;

var builder = WebApplication.CreateBuilder(args);

n
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

n
// Configure EF Core (SQLite by default for local development)
var connection = builder.Configuration.GetConnectionString("Default") ?? "Data Source=kfh.db";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(connection));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Ensure DB created (for simple development/demo)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
