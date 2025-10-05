using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Contracts.Categories
{
    public  record CreateCategoryRequest (string Name, string Description);
}
