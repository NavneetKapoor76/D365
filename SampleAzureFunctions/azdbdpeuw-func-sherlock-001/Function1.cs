using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SampleFunctionApp
{
      public static class Vehicle
    {
        [FunctionName("FunctionAUDI")]
        public static async Task<IActionResult> AUDI(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("FunctionAUDI processed a request.");
            //Add the Business Logic
            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This FunctionAUDI executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This FunctionAUDI executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("FunctionBMW")]
        public static async Task<IActionResult> BMW(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("FunctionBMW processed a request.");
            //Add the Business Logic

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This FunctionBMW executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This FunctionBMW executed successfully.";

            return new OkObjectResult(responseMessage);
        }
        [FunctionName("FunctionSkoda")]
        public static async Task<IActionResult> Skoda(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("FunctionSkoda processed a request.");

            //Add the Business Logic
            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This FunctionSkoda executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This FunctionSkoda executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}