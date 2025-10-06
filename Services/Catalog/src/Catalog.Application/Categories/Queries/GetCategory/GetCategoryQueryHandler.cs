using Catalog.Application.Common.Interfaces;
using Catalog.Domain.CategoryAggreate;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Categories.Queries.GetCategory
{
  
    public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, ErrorOr<Category>>
    {
        private readonly ICategoriesRepository _categoriesRepository;

        public GetCategoryQueryHandler(ICategoriesRepository categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
        }

        public async Task<ErrorOr<Category>> Handle(GetCategoryQuery query, CancellationToken cancellationToken)
        {
            var category = await _categoriesRepository.GetByIdAsync(query.CategoryId,cancellationToken);

            return category is null
                ? CategoryErrors.NotFound
                : category;
        }
    }

}
