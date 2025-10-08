using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Contracts.Authors;

   public record UpdateAuthorRequest(string Name, string bio, bool IsActive);
