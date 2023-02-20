using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace Masa.EShop.Contracts.Catalog.Request;

public abstract record ItemsQueryBase<TResult> : Query<TResult>
{
    public virtual int Page { get; set; } = 1;

    public virtual int PageSize { get; set; } = 20;
}