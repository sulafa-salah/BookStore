using Catalog.Application.Common.Interfaces;
using Catalog.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.SubcutaneousTests.Common;
public  class TestCurrentUserProvider : ICurrentUserProvider
{
    private readonly CurrentUser _user;

    public TestCurrentUserProvider(Guid id, IEnumerable<string> roles)
    {
        _user = new CurrentUser(id, roles.ToList());
    }

    public CurrentUser GetCurrentUser() => _user;
}