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
  
    public record GetCategoryQuery(Guid CategoryId)
    : IRequest<ErrorOr<Category>>;
}
