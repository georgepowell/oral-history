using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using OralHistory.Controllers;
using OralHistory.Models;
using RedDog.Search;
using RedDog.Search.Http;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace OralHistory.Services
{
    public class InterviewCreator
    {
        static string databaseStr = "wZ4-AA==";
        static string collStr = "wZ4-AOlfZwA=";
        static string collectionLink;
        static DocumentClient client;
        static DocumentCollection collection;

        static ApiConnection connection;
        static IndexManagementClient searchClient;

        public InterviewCreator()
        {
            var apiKey = ConfigurationManager.ConnectionStrings["SearchAPIKey"];
            string connString = apiKey.ConnectionString;

            connection = ApiConnection.Create("oralhistory", "796F1DAEF6AB85BB6C203A242620B525");
            searchClient = new IndexManagementClient(connection);
        }


        public Interview GetInterviewFromRequest(UploadData data)
        {
            string pre = "api.soundcloud.com/tracks/";
            string post = "&amp;";
            int start = data.Soundcloud.IndexOf(pre) + pre.Length;
            int end = data.Soundcloud.IndexOf(post);
            string soundcloudId = data.Soundcloud.Substring(start, end - start);

            string dateAsString = String.Format("{0}-{1}-{2}", data.Date.Day, data.Date.Month, data.Date.Year);
            string intervieweeNoSpaces = data.Interviewee.Replace(' ', '-');

            return new Interview()
            {
                id = String.Format("{0}_{1}", intervieweeNoSpaces, dateAsString),
                Interviewer = data.Interviewer,
                ManualTranscription = data.Transcription,
                SummaryLines = data.Summary.Split('\n'),
                SoundcloudID = soundcloudId,
                DateOfInterview = dateAsString,
                Title = data.Title,
                Interviewee = data.Interviewee
            };
        }

        public void UploadInterviewToBlobStorage(Interview interview)
        {
            //StorageUri uri = new StorageUri(new Uri("https://gpowell.blob.core.windows.net/oralhistory"));
            StorageCredentials creds = new StorageCredentials("gpowell", "9OpNMvIUGz63EuCRxJC/pyfxgfOVrxKvKcn9CeO58QePo4sNDWryoXDOPrVGH+yehMBDNDXeNN0zobku/sfkSA==");
            CloudStorageAccount acc = new CloudStorageAccount(creds, true);

            var blobClient = acc.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("oralhistory");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(interview.id);
            blockBlob.UploadText(JsonConvert.SerializeObject(interview));

            var fromServer = container.GetBlockBlobReference(interview.id);
            Interview interviewFromServer = JsonConvert.DeserializeObject<Interview>(fromServer.DownloadText());
        }

        /*
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

        private static string ReadWholeFile(string path)
        {
            if (!File.Exists(path))
                return "";

            using (StreamReader reader = new StreamReader(path))
            {
                return reader.ReadToEnd();
            }
        }

        private static Document UploadInterviewToDocumentDB(Interview interview)
        {
            //return client.CreateDocumentAsync(collectionLink, interview).Result.Resource;
            var document = client.
                CreateDocumentQuery(collection.DocumentsLink)
                .Where(n => n.Id == interview.id)
                .Select(n => n)
                .ToList()
                .FirstOrDefault();
            Console.WriteLine(document.SelfLink);
            var result = client.ReplaceDocumentAsync(document.SelfLink, interview).Result;

            return result.Resource;
        }

        private static void UploadExtraFilesToBlobStorage(DirectoryInfo directory, Interview interview)
        {
            StorageCredentials creds = new StorageCredentials("gpowell", "9OpNMvIUGz63EuCRxJC/pyfxgfOVrxKvKcn9CeO58QePo4sNDWryoXDOPrVGH+yehMBDNDXeNN0zobku/sfkSA==");
            CloudStorageAccount acc = new CloudStorageAccount(creds, true);

            var blobClient = acc.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("files");

            // summary
            if (File.Exists(directory.FullName + @"\summary.docx"))
            {
                CloudBlockBlob summaryBlockBlob = container.GetBlockBlobReference("SummaryFor" + interview.id + ".docx");
                summaryBlockBlob.UploadFromStream(File.OpenRead(directory.FullName + @"\summary.docx"));
            }

            // manual transcription
           //if (File.Exists(directory.FullName + @"\manual-transcription.docx"))
           //{
           //    CloudBlockBlob summaryBlockBlob = container.GetBlockBlobReference("ManualTranscriptionFor" + interview.id + ".doc");
           //    summaryBlockBlob.UploadFromStream(File.OpenRead(directory.FullName + @"\manual-transcription.doc"));
           //}

            // manual transcription
            if (File.Exists(directory.FullName + @"\manual-transcription.doc"))
            {
                CloudBlockBlob summaryBlockBlob = container.GetBlockBlobReference("ManualTranscriptionFor" + interview.id + ".doc");
                summaryBlockBlob.UploadFromStream(File.OpenRead(directory.FullName + @"\manual-transcription.doc"));
            }
        }

        private static void UploadInterviewToSearch(Interview interview)
        {
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
        }*/
    }
}