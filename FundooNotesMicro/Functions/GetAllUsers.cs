using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using UserService.Models;
using System.Collections.Generic;
using System.Linq;

namespace UserService.Functions
{
    public class GetAllUsers
    {
        private readonly CosmosClient _cosmosClient;

        public GetAllUsers(CosmosClient cosmosClient)
        {
            this._cosmosClient = cosmosClient;
        }

        [FunctionName("GetAllUsers")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            QueryDefinition query = new QueryDefinition(
                "select * from UserDetails");

            var container = this._cosmosClient.GetContainer("UserDB", "UserDetails");

            List<UserRegModel> userLists = new List<UserRegModel>();
            using (FeedIterator<UserRegModel> resultSet = container.GetItemQueryIterator<UserRegModel>(query))
            {
                while (resultSet.HasMoreResults)
                {
                    FeedResponse<UserRegModel> response = await resultSet.ReadNextAsync();
                    UserRegModel user = response.First();
                    log.LogInformation($"Id: {user.Id};");
                    if (response.Diagnostics != null)
                    {
                        Console.WriteLine($" Diagnostics {response.Diagnostics.ToString()}");
                    }

                    userLists.AddRange(response);

                }
            }

            return new OkObjectResult(userLists);


        }
    }
}
