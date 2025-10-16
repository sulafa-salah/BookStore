using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Contracts.Authentication;
    public  record RevokeRefreshTokenRequest(string RefreshToken);