using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Md2Medium.Models;

namespace Md2Medium.Functions
{
    public static class Md2MediumPublisher
    {
        [FunctionName("Md2MediaumPublisher")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Blog>(requestBody);

            var processor = new FileProcessor(data.RepoAddress, data.Branch, data.FilePath);
            var content = await processor.GetFileAsync();

            var publisher = new Publisher(Environment.GetEnvironmentVariable("INTEGRATION_TOKEN"));
            var res = await publisher.PublishAsync(data.Title, content);
            return new OkObjectResult(res);
        }
    }
}
