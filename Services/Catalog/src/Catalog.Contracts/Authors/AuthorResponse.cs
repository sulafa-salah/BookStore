using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Contracts.Authors;

public record AuthorResponse(Guid Id, string Name, string Bio,bool IsActive);