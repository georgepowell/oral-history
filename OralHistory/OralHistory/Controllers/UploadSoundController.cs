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
using System.Web.Http.ModelBinding;
using System.Net.Http;
using System.Net;
using System.Web;
using System.IO;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Queue;

namespace OralHistory.Controllers
{
    public class UploadSoundController : ApiController
    {
        public async Task<IHttpActionResult> Post(string id)
        {
            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            await Request.Content.ReadAsMultipartAsync(provider);

            MultipartFileData file = provider.FileData.First();

            AddQueueMessage("upload", id, file.LocalFileName);
            
            return Redirect(new Uri( System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/#/interview/" + id));
        }


        static void AddQueueMessage(string operation, string interviewID, string data)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["StorageConnection"].ConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("webasr");
            CloudQueueMessage message = new CloudQueueMessage(String.Format("{0}|{1}|{2}", operation, interviewID, data));
            queue.AddMessage(message);
        }
    }
}
