using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor.Functions.Options;
    public  class ProcessorOptions
    {
    public const string Section = "Values";
    // Storage
    public string AzureWebJobsStorage { get; set; } 
    public string MediaContainer { get; set; } 

        // Downstream catalog
        public string CatalogBaseURL { get; set; } 
        public string? InternalAPIKEY { get; set; }  
    }