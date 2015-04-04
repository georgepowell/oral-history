using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using OralHistory.Models;
using RedDog.Search;
using RedDog.Search.Http;
using RedDog.Search.Model;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Documents.Linq;
using System.Linq;

namespace OralHistory.Controllers
{
    public class InterviewsController : ApiController
    {
        static string databaseStr = "wZ4-AA==";
        static string collStr = "wZ4-AOlfZwA=";

        // GET api/<controller>
        public async Task<Interview> Get(string id)
        {
            var apiKey = ConfigurationManager.ConnectionStrings["DocumentAPIKey"];
            string connString = apiKey.ConnectionString;
            DocumentClient client;

            DocumentCollection collection;
            string collectionLink = String.Format("dbs/{0}/colls/{1}", databaseStr, collStr);
            string documentLink = String.Format("dbs/{0}/colls/{1}/docs/{2}", databaseStr, collStr, id);
            client = new DocumentClient(new Uri("https://oralhistory.documents.azure.com:443/"), connString);
            collection = await client.ReadDocumentCollectionAsync(collectionLink);

            return client.
                CreateDocumentQuery<Interview>(collection.DocumentsLink)
                .Where(n => n.id == id)
                .Select(n => n)
                .ToList()
                .FirstOrDefault();
        }
    }
}
