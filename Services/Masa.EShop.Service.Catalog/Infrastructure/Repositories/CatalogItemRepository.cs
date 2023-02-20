using Masa.BuildingBlocks.Caching;
using Masa.BuildingBlocks.Data.UoW;
using Masa.Contrib.Ddd.Domain.Repository.EFCore;
using Masa.EShop.Service.Catalog.Domain.Aggregates;
using Masa.EShop.Service.Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Masa.EShop.Service.Catalog.Infrastructure.Repositories;


public class CatalogItemRepository : Repository<CatalogDbContext, CatalogItem, Guid>, ICatalogItemRepository
{
    /// <summary>
    /// 使用多级缓存
    /// </summary>
    private readonly IMultilevelCacheClient _multilevelCacheClient;

    public CatalogItemRepository(CatalogDbContext context, IUnitOfWork unitOfWork, IMultilevelCacheClient multilevelCacheClient) : base(context, unitOfWork)
    {
        _multilevelCacheClient = multilevelCacheClient;
    }

    public override async Task<CatalogItem?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        TimeSpan? timeSpan = null;
        var catalogInfo = await _multilevelCacheClient.GetOrSetAsync(id.ToString(), () =>
        {
            //仅当内存缓存、Redis缓存都不存在时执行, 当db不存在时此数据将在5秒内被再次访问时将直接返回`null`, 如果db存在则写入`redis`, 写入内存缓存 (并设置滑动过期: 5分钟, 绝对过期时间: 3小时)
            var info = Context.Set<CatalogItem>()
                .Include(catalogItem => catalogItem.CatalogType)
                .Include(catalogItem => catalogItem.CatalogBrand)
                .AsSplitQuery()
                .FirstOrDefaultAsync(catalogItem => catalogItem.Id == id, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();

            if (info != null)
                return new CacheEntry<CatalogItem>(info, TimeSpan.FromDays(3))
                {
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                };

            timeSpan = TimeSpan.FromSeconds(5);
            return new CacheEntry<CatalogItem>(info);
        }, timeSpan == null ? null : new CacheEntryOptions(timeSpan));
        return catalogInfo;
    }
}
