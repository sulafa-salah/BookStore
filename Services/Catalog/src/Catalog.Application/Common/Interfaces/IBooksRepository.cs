using Catalog.Domain.BookAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Common.Interfaces;
  
   public interface IBooksRepository
{
        Task AddBookAsync(Book book, CancellationToken ct = default);
        Task<bool> IsIsbnTakenAsync(string isbn, CancellationToken ct = default);
        Task<bool> IsSkuTakenAsync(string sku, CancellationToken ct = default);
    }
