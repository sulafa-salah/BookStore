using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Caching.Redis;
    public sealed class RedisOptions
    {
        public string ConnectionString { get; set; } = null!;
    public int DefaultTtlMinutes { get; set; } 
       
    }