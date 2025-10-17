using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Infrastructure.Redis;
    public sealed class RedisOptions
    {
        public string ConnectionString { get; set; } = "redis:6379";
        public int DefaultTtlMinutes { get; set; } = 10080; // 7 days
        public string Namespace { get; set; } = "dev";       
    }