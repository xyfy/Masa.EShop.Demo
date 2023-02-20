
using System.Reflection;
using FluentValidation;
using Masa.BuildingBlocks.Caching;
using Masa.BuildingBlocks.Data.UoW;
using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.BuildingBlocks.Dispatcher.Events;
using Masa.BuildingBlocks.Dispatcher.IntegrationEvents;
using Masa.EShop.Service.Catalog.Infrastructure;
using Masa.EShop.Service.Catalog.Infrastructure.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMapster();

GlobalMappingConfig.Mapping();//指定自定义映射
builder.Services.AddMultilevelCache(distributedCacheOptions =>
{
    distributedCacheOptions.UseStackExchangeRedisCache();
});
builder.Services.AddMasaDbContext<CatalogDbContext>(dbContextBuilder =>
{
    dbContextBuilder
        .UseSqlite() //使用Sqlite数据库
        .UseFilter(); //数据数据过滤
});

builder.Services
    .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()) //添加指定程序集下的`FluentValidation`验证器
    .AddDomainEventBus(options =>
    {
        options.UseIntegrationEventBus(integrationEventBus =>
                integrationEventBus
                    .UseDapr()
                    .UseEventLog<CatalogDbContext>())
            .UseEventBus(eventBusBuilder => eventBusBuilder.UseMiddleware(new[] { typeof(ValidatorEventMiddleware<>), typeof(LoggingEventMiddleware<>) })) //使用验证中间件、日志中间件
            .UseUoW<CatalogDbContext>() //使用工作单元, 确保原子性
            .UseRepository<CatalogDbContext>();
    });

var app = builder.AddServices();
app.UseMasaExceptionHandler();
app.MapGet("/", () => "Hello World!");

app.Run();
