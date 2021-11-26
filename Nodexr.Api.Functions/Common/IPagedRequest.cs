namespace Nodexr.Api.Functions.Common;

using MediatR;
using Nodexr.Api.Contracts.Pagination;

public interface IPagedRequest<T> : IRequest<Paged<T>>
{
    public PaginationFilter Pagination { get; }
}

public interface IPagedRequestHandler<in TRequest, TResponse>
    : IRequestHandler<TRequest, Paged<TResponse>>
    where TRequest : IPagedRequest<TResponse>
{

}
