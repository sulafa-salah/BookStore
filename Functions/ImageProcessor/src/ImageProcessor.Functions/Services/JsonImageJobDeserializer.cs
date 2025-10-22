using ImageProcessor.Functions.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ImageProcessor.Functions.Services;
    public  class JsonImageJobDeserializer : IImageJobDeserializer
    {
        private static readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

        public ImageJob? TryDeserialize(string raw)
        {
            try
            {
                return JsonSerializer.Deserialize<ImageJob>(raw, _opts);
            }
            catch
            {
                return null; 
            }
        }
    }