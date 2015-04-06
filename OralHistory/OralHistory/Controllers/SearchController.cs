using OralHistory.Models;
using RedDog.Search;
using RedDog.Search.Http;
using RedDog.Search.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace OralHistory.Controllers
{
    public class SearchController : ApiController
    {
        // GET api/<controller>
        public async Task<IEnumerable<SearchResultViewModel>> Get(string q)
        {
            var apiKey = ConfigurationManager.ConnectionStrings["SearchAPIKey"];
            string connString = apiKey.ConnectionString;

            ApiConnection connection = ApiConnection.Create("oralhistory", connString);
            var client = new IndexQueryClient(connection);

            var results = await client.SearchAsync("interviews", new SearchQuery(q)
            {
                Highlight = "automatictranscription,summary,interviewer,interviewee,manualtranscription"
            }.Count(true));


            return results.Body.Records.Select(record => new SearchResultViewModel()
            {
                Id = (string)record.Properties["id"],
                Title = (string)record.Properties["title"],
                Highlights = record.Highlights.Keys.Select(key => new HighlightResult()
                {
                    Field = ReadableKeys[key],
                    Highlights = record.Highlights[key]
                }).ToList()
            });
        }

        static Dictionary<string, string> ReadableKeys = new Dictionary<string, string>
        {
            {"manualtranscription", "Manual Transcription"},
            {"summary", "Summary"},
            {"automatictranscription", "Automatic Transcription"},
            {"interviewer", "Interviewer"},
            {"interviewee", "Interviewee"},
            
        };
    }
}