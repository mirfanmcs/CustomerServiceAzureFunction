using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.Documents.Client;
using System;

namespace CustomerServiceAzureFunction
{
    public static class CustomerService
    {
        [FunctionName("CustomerService")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {

            log.Info("CustomerService HTTP trigger function processed a request.");

            string id = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "id", true) == 0)
                .Value;

            dynamic data = await req.Content.ReadAsAsync<object>();

            var customer = GetCustomer(id);

            return req.CreateResponse(HttpStatusCode.OK, customer, "application/json");

        }

        private static Customer GetCustomer(string id)
        {

             string EndpointUrl = "https://testirf.documents.azure.com:443/";
             string PrimaryKey = "Z8tFMEl6a2IfOevU9hy6euJj9Z4WsUJuJaAJlolGteo8FpCikLJFyNmQigSO2D1IBSYhJky6NkcsZyCB3bg2eg==";
             string databaseName = "CustomerDb";
             string collectionName = "Customer";

             var client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);


            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };

            IQueryable<Customer> customerQuery = client.CreateDocumentQuery<Customer>(
                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions)
                    .Where(f => f.id == id);

            var customer = customerQuery.ToList().FirstOrDefault();
            return customer;

        }
    }
    public class Customer
    {
        public string id { get; set; }
        public string CustomerName { get; set; }
    }

}
