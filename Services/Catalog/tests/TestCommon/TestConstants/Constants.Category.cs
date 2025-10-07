using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCommon.TestConstants;

public static partial class Constants
    {
        public static class Category
        {
            public static readonly Guid Id = Guid.NewGuid();

            public const string Name = "Fiction";
        public const string Description = "All fiction";
    }
    }