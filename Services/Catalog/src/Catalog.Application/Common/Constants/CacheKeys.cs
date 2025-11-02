using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Common.Constants;
    public static class CacheKeys
    {
        public static string Book(Guid bookId) => $"book:{bookId}";
        public static string Category(Guid categoryId) => $"category:{categoryId}";
        public static string Author(Guid authorId) => $"author:{authorId}";
    }