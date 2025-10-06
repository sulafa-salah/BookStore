using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Contracts.Categories;
    public  record UpdateCategoryRequest(string Name, string Description, bool IsActive);
