using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor.Functions.Contracts;
    public  record ImageJob(string BookId, string BlobName, string ContentType);