using OralHistory.Models;
using RedDog.Search;
using RedDog.Search.Http;
using RedDog.Search.Model;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;

namespace OralHistory.Controllers
{
    public class InterviewsController : ApiController
    {
        // GET api/<controller>
        public async Task<Interview> Get(string id)
        {
            var apiKey = ConfigurationManager.ConnectionStrings["SearchAPIKey"];
            string connString = apiKey.ConnectionString;

            ApiConnection connection = ApiConnection.Create("oralhistory", connString);
            var client = new IndexQueryClient(connection);
            var result = await client.LookupAsync("interviews", new LookupQuery(id));

            var record = result.Body.Properties;

            return new Interview() 
            { 
                Title = (string)record["title"],
                Description = (string)record["description"],
                Transcription = (string)record["transcription"],
                Id = (string)record["id"],
            };
        }
    }
}
