using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UserService.Authorization;
using Microsoft.Azure.Documents.Client;
using UserService.Models;
using System.Linq;

namespace UserService.Functions
{
    public class UserLogin
    {
        GenerateToken auth = new GenerateToken();

        [FunctionName("UserLogin")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                ConnectionStringSetting = "CosmosDBConnection"
                )]
            DocumentClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<UserRegModel>(requestBody);

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("UserDB", "UserDetails");
            var document = client.CreateDocumentQuery<UserRegModel>(collectionUri).Where(t => t.Email == data.Email && t.Password == data.Password)
                    .AsEnumerable().FirstOrDefault();

            if (document != null)
            {
                var token = auth.IssuingToken(document.Id.ToString());
                return new OkObjectResult(token);

            }
            return (ActionResult)new NotFoundResult();
        }
    }
}
