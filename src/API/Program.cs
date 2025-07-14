using MeterReadingsApi.Data;
using MeterReadingsApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MeterReadingsDbContext>(opt =>
    opt.UseInMemoryDatabase("MeterReadings"));

builder.Services.AddApplicationServices();
builder.Services.AddRepositories();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MeterReadingsDbContext>();

    if (!context.Accounts.Any())
    {
        var seedAccounts = MeterReadingsDbContext.GetSeedAccountsTestData();
        // Seed initial data if Accounts table is empty
        context.Accounts.AddRange(seedAccounts);
        context.SaveChanges();
    }
}

app.Run();
