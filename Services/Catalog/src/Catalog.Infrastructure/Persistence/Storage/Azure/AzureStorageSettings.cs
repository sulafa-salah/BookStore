using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Persistence.Storage.Azure;

   public class AzureStorageSettings
{
    public const string Section = "AzureStorage";
    public string ConnectionString { get; init; } = null!;
    public string CoversContainer { get; init; } = "covers";
    public string ThumbsContainer { get; init; } = "thumbs";

}

 