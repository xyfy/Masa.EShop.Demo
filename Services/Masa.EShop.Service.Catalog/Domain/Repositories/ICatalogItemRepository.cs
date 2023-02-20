using Masa.BuildingBlocks.Ddd.Domain.Repositories;
using Masa.EShop.Service.Catalog.Domain.Aggregates;

namespace Masa.EShop.Service.Catalog.Domain.Repositories;

public interface ICatalogItemRepository : IRepository<CatalogItem, Guid>
{
    //如果有需要扩展的能力, 可在自定义仓储中扩展
}