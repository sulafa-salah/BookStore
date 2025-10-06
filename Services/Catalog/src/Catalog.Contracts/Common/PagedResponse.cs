using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Contracts.Common;

public sealed record PagedResponse<T>(
 IReadOnlyList<T> Items,
 int TotalCount,
 int PageNumber,
 int PageSize,
 int TotalPages,
 bool HasPrevious,
 bool HasNext);