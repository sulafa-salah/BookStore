using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Contracts.Categories;
    public  record GetCategoriesRequest(
     int PageNumber = 1,
     int PageSize = 10,
     string? Search = null,
     string? SortBy = "name",
     string? SortDir = "asc");