using ImageProcessor.Functions.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor.Functions.Services;
    public interface IImageJobDeserializer
    {
        /// Returns null for bad shape.
        ImageJob? TryDeserialize(string raw);
    }