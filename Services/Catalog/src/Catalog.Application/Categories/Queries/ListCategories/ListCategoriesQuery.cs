using Catalog.Application.Common.Models;
using Catalog.Domain.CategoryAggreate;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Categories.Queries.ListCategories;
    public  record ListCategoriesQuery(
      int PageNumber = 1,
      int PageSize = 10,
      string? Search = null,
      string? SortBy = "name",
      string? SortDir = "asc"
  ) : IRequest<ErrorOr<PagedResult<Category>>>;