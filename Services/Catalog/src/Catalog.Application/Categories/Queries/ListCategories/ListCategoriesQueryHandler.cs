using Catalog.Application.Common.Interfaces;
using Catalog.Application.Common.Models;
using Catalog.Domain.CategoryAggreate;
using ErrorOr;
using MediatR;

namespace Catalog.Application.Categories.Queries.ListCategories;
    public class ListCategoriesQueryHandler : IRequestHandler<ListCategoriesQuery, ErrorOr<PagedResult<Category>>>
{
        private readonly ICategoriesRepository _categoriesRepository;

        public ListCategoriesQueryHandler(ICategoriesRepository categoriesRepository) => _categoriesRepository = categoriesRepository;

        public async Task<ErrorOr<PagedResult<Category>>> Handle(ListCategoriesQuery r, CancellationToken ct)
        {
            var listCatgories = await _categoriesRepository.ListCategoriesAsync(
                  r.PageNumber, r.PageSize, r.Search, r.SortBy, r.SortDir.Equals("desc", StringComparison.OrdinalIgnoreCase), ct);
        return new PagedResult<Category>(listCatgories.Items, listCatgories.TotalCount, r.PageNumber, r.PageSize);
    }
    }
   