using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ImageProcessor.Functions
{
    public class ProcessImageJob
    {
        private readonly ILogger<ProcessImageJob> _logger;

        public ProcessImageJob(ILogger<ProcessImageJob> logger)
        {
            _logger = logger;
        }

        [Function(nameof(ProcessImageJob))]
        public void Run([QueueTrigger("image-jobs", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
        }
    }
}
