using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Contracts.Authors;
    public  record GetAuthorsRequest(
     int PageNumber = 1,
     int PageSize = 10,
     string? Search = null,
     string? SortBy = "name",
     string? SortDir = "asc");