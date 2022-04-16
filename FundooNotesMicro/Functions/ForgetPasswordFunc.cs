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
    public class ForgetPasswordFunc
    {
        GenerateToken auth = new GenerateToken();

        [FunctionName("ForgetPasswordFunc")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                ConnectionStringSetting = "CosmosDBConnection"
                )]
                DocumentClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request for Forget Password.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<ForgetPassword>(requestBody);

            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("UserDB", "UserDetails");
            var document = client.CreateDocumentQuery<UserRegModel>(collectionUri, option).Where(t => t.Email == data.Email)
                    .AsEnumerable().FirstOrDefault();
            if (document != null)
            {
                var token = this.auth.IssuingToken(document.Id.ToString());
                new MSMQ().MSMQSender(token);
                return new OkObjectResult(token);

            }
            return (ActionResult)new NotFoundResult();
        }
    }
}
