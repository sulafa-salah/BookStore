using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Contracts.Categories
{
   
    public record CategoryResponse(Guid Id, string Name,string Description);
}
