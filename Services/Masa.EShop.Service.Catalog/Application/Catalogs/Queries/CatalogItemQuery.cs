using Masa.EShop.Contracts.Catalog.Dto;
using Masa.EShop.Contracts.Catalog.Request;
using Masa.EShop.Service.Catalog.Domain.Aggregates;
using Masa.Utils.Models;

namespace Masa.EShop.Service.Catalog.Application.Catalogs.Queries;

public record CatalogItemQuery: ItemsQueryBase<PaginatedListBase<CatalogListItemDto>>
{
    public string Name { get; set; }
    
    public override int Page { get; set; } = 1;

    public override int PageSize { get; set; } = 20;
    
    /// <summary>
    /// 存储查询结果
    /// </summary>
    public override PaginatedListBase<CatalogListItemDto> Result { get; set; }
}
