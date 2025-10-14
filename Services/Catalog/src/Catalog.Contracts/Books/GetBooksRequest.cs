using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Contracts.Books;
    public  record GetBooksRequest(
    int PageNumber = 1,
     int PageSize = 20,
     string? Search = null,
     string? SortBy = "createdAt",
     string? SortDir = "asc");