using Catalog.Application.Common.Models;
using Catalog.Contracts.Common;



namespace Catalog.Application.Common.Mappings;

    public static class PagingMappings
    {
        public static PagedResponse<TOut> ToResponse<TIn, TOut>(
            this PagedResult<TIn> src,
            Func<TIn, TOut> projector)
        {
            var items = src.Items.Select(projector).ToList();

            return new PagedResponse<TOut>(
                Items: items,
                TotalCount: src.TotalCount,
                PageNumber: src.PageNumber,
                PageSize: src.PageSize,
                TotalPages: src.TotalPages,
                HasPrevious: src.HasPrevious,
                HasNext: src.HasNext
            );
        }
    }