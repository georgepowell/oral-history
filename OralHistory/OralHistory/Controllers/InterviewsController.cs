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
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using OralHistory.Services;

namespace OralHistory.Controllers
{
    public class InterviewsController : ApiController
    {
        static string databaseStr = "wZ4-AA==";
        static string collStr = "wZ4-AOlfZwA=";
        CloudStorageAccount acc;
        StorageCredentials creds;
        CloudBlobClient blobClient;
        CloudBlobContainer container;

        public InterviewsController() : base()
        {
            creds = new StorageCredentials("gpowell", ConfigurationManager.ConnectionStrings["BlobAPIKey"].ConnectionString);
            acc = new CloudStorageAccount(creds, true);

            blobClient = acc.CreateCloudBlobClient();
            container = blobClient.GetContainerReference("oralhistory");
        }


        public async Task<IEnumerable<Segment>> Get(string id, string query)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(id);

            var fromServer = container.GetBlockBlobReference(id);
            string json = await fromServer.DownloadTextAsync();
            Interview rtn = JsonConvert.DeserializeObject<Interview>(json);

            return InterviewSearch.SearchInterview(rtn.AutomaticTranscription, query);
        }



        // GET api/<controller>
        public async Task<Interview> Get(string id)
        {
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(id);

            var fromServer = container.GetBlockBlobReference(id);
            string json = await fromServer.DownloadTextAsync();
            Interview rtn = JsonConvert.DeserializeObject<Interview>(json);
            return rtn;
        }

        public IEnumerable<Interview> Get()
        {
            return container.ListBlobs().Select(blob =>
            {
                string uri = blob.Uri.ToString();
                var split = uri.Split('/').Where(str => !String.IsNullOrWhiteSpace(str));
                var last = split.Last();
                var text = container.GetBlockBlobReference(last).DownloadText();
                return JsonConvert.DeserializeObject<Interview>(text);
            });
        }
    }
}
