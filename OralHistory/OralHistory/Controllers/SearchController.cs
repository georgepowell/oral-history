﻿using OralHistory.Models;
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
        public async Task<IEnumerable<Interview>> Get(string q)
        {
            var apiKey = ConfigurationManager.ConnectionStrings["SearchAPIKey"];
            string connString = apiKey.ConnectionString;

            ApiConnection connection = ApiConnection.Create("oralhistory", connString);
            var client = new IndexQueryClient(connection);

            var results = await client.SearchAsync("interviews", new SearchQuery(q).SearchField("title").Count(true));

            return new List<Interview> { new Interview() { Title = "Hello World!", Description = "This is a test", Transcription = "Blah blah blah this are words" } };
        }
    }
}