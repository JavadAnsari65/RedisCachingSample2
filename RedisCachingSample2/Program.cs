using Microsoft.EntityFrameworkCore;
using RedisCachingSample2.Application;
using RedisCachingSample2.Infrastructure.Configuration;
using RedisCachingSample2.Infrastructure.Entities;
using RedisCachingSample2.Infrastructure.Repository;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Add DbContext Srvice
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DbConnection")));

//Add Services
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<CRUDRepo<Post>>();

builder.Services.AddScoped<ICacheService, CacheService>();

//Add Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(option => 
    ConnectionMultiplexer.Connect(
        builder.Configuration.GetConnectionString("RedisCacheURL")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
