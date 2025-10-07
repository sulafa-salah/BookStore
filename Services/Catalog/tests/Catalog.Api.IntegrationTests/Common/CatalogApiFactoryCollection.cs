using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Api.IntegrationTests.Common;


    [CollectionDefinition(CollectionName)]
    public class CatalogApiFactoryCollection : ICollectionFixture<CatalogApiFactory>
    {
        public const string CollectionName = "CatalogApiFactoryCollection";
    }
