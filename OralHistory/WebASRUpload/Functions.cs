using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;
using System.Xml;
using Newtonsoft.Json;
using RedDog.Search.Http;
using RedDog.Search;
using RedDog.Search.Model;

namespace WebASRUpload
{
    public class Functions
    {
        public static async Task ProcessQueueMessage([QueueTrigger("webasr")] string message, TextWriter log)
        {
            string[] parts = message.Split('|');

            string operation = parts[0];
            string interviewId = parts[1];

            if (operation == "upload")
            {
                string path = parts[2];
                WebASRClient client = new WebASRClient();
                await client.Connect();

                log.WriteLine("starting to process file: " + path);
                var code = await client.StartTranscription(path);
                AddQueueMessage("check", interviewId, code);
            }
            else if (operation == "check")
            {

                string webAsrId = parts[2];
                WebASRClient client = new WebASRClient();
                await client.Connect();

                log.WriteLine("starting to check upload: " + message);
                bool finished = await client.TranscriptionFinishedProcessing(webAsrId);
                if (finished)
                {
                    string xml = await client.DownloadTranscription(webAsrId);
                    await SetInterviewTranscription(xml, interviewId);
                }
                else
                    AddQueueMessage("check", interviewId, webAsrId);
            }
        }

        static void AddQueueMessage(string operation, string interviewID, string data)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("webasr");
            CloudQueueMessage message = new CloudQueueMessage(String.Format("{0}|{1}|{2}", operation, interviewID, data));
            queue.AddMessage(message, null, TimeSpan.FromMinutes(1));
        }

        static async Task SetInterviewTranscription(string xmlTranscription, string interviewID)
        {
            string databaseStr = "wZ4-AA==";
            string collStr = "wZ4-AOlfZwA=";
            CloudStorageAccount acc;
            StorageCredentials creds;
            CloudBlobClient blobClient;
            CloudBlobContainer container;
            creds = new StorageCredentials("gpowell", ConfigurationManager.ConnectionStrings["BlobAPIKey"].ConnectionString);
            acc = new CloudStorageAccount(creds, true);
            blobClient = acc.CreateCloudBlobClient();
            container = blobClient.GetContainerReference("oralhistory");

            var blockBlob = container.GetBlockBlobReference(interviewID);
            string json = await blockBlob.DownloadTextAsync();
            Interview interview = JsonConvert.DeserializeObject<Interview>(json);


            var doc = new XmlDocument();
            doc.LoadXml(xmlTranscription);
            Transcription automaticTranscription = Transcription.FromXml(doc);

            interview.AutomaticTranscription = automaticTranscription;
            interview.SummaryTimes = GetSummaryTimes(interview.SummaryLines, automaticTranscription);

            await blockBlob.UploadTextAsync(JsonConvert.SerializeObject(interview));

            ApiConnection connection = ApiConnection.Create("oralhistory", ConfigurationManager.ConnectionStrings["SearchAPIKey"].ConnectionString);
            IndexManagementClient searchClient = new IndexManagementClient(connection);
            var result = searchClient.PopulateAsync("interviews",
                new IndexOperation(IndexOperationType.MergeOrUpload, "id", interview.id)
                    .WithProperty("title", interview.Title)
                    .WithProperty("date", new DateTimeOffset(interview.DateAsDateTime))
                    .WithProperty("automatictranscription", interview.AutomaticTranscription.FullText)
                    .WithProperty("summary", interview.FullSummary)
                    .WithProperty("interviewer", interview.Interviewer)
                    .WithProperty("interviewee", interview.Interviewee)
                    .WithProperty("manualtranscription", interview.ManualTranscription)).Result;
        }

        private static double[] GetSummaryTimes(string[] summary, Transcription transcription)
        {
            List<double> rtn = new List<double>();

            Documents docs = new Documents();
            foreach (var segment in transcription.Segments)
            {
                string sentence = segment.Sentence;
                DocumentVector vec = new DocumentVector(sentence);
                vec.XmlSegment = segment;
                docs.Add(vec);
            }

            return summary.Select(n =>
            {
                var a = MostSimilar(n, docs);
                return a == null ? 0.0 : a.StartTime;
            }).ToArray();
        }

        static Segment MostSimilar(string sentence, Documents docs)
        {
            DocumentVector v = new DocumentVector(sentence);

            double minSimilarity = double.MaxValue;
            DocumentVector match = null;

            foreach (var doc in docs.DocVectors)
            {
                double sim = doc.Similarity(v, word => Math.Log(docs.Count / (1 + docs.DocumentFrequency(word))));
                if (sim < minSimilarity)
                {
                    minSimilarity = sim;
                    match = doc;
                }
            }

            return minSimilarity < -2.0 ? match.XmlSegment : null;
        }

    }
}
