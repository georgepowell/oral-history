using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebASRUpload
{
    class WebASRClient
    {
        HttpClient client;

        public async Task Connect()
        {
            client = new HttpClient();

            HttpContent httpContent = new StringContent("client=ehdemo", Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await client.PostAsync("http://www.webasr.com/controller?event=APICheckLogin", httpContent);
        }

        public async Task<string> StartTranscription(string filePath)
        {
            string xml = createXmlString(filePath);
            HttpContent httpContent = new StringContent(xml, Encoding.UTF8, "application/xml");
            var response = await client.PostAsync("http://www.webasr.com/controller?event=APIReceiveFileDataXML", httpContent);
            string uploadID;

            using (FileStream str = new FileStream(filePath, FileMode.Open))
            {
                httpContent = new StreamContent(str);
                response = await client.PostAsync("http://www.webasr.com/controller?event=APIReceiveFile", httpContent);
                uploadID = response.Headers.Get("UploadID").First();
                return uploadID;
            }
        }

        public async Task<bool> TranscriptionFinishedProcessing(string uploadID)
        {
            var response = await client.GetAsync("http://www.webasr.com/controller?event=APIGetStatus&uploadID=" + uploadID);
            try
            {
                return response.Headers.Get("status").First() == "completed";
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> DownloadTranscription(string uploadID)
        {
            var response = await client.GetAsync("http://www.webasr.com/controller?event=APIGetDocument&type=transcript&format=xml&uploadID=" + uploadID);
            return await response.Content.ReadAsStringAsync();
        }

        static string createXmlString(string fileName)
        {
            FileInfo inf = new FileInfo(fileName);

            string md5Hash;
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    byte[] bytes = md5.ComputeHash(stream);

                    StringBuilder sBuilder = new StringBuilder();

                    for (int i = 0; i < bytes.Length; i++)
                        sBuilder.Append(bytes[i].ToString("x2"));
                    md5Hash = sBuilder.ToString();
                }
            }

            string outXML = "";
            outXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
            outXML += "<java version=\"1.6.0_10-rc2\" class=\"java.beans.XMLDecoder\">\n";
            outXML += " <object class=\"java.util.Vector\">\n";
            outXML += "  <void method=\"add\">\n";
            outXML += "   <object class=\"uk.ac.shef.dcs.webasr.upload.FileData\">\n";
            outXML += "    <void property=\"clientFilename\">\n";
            outXML += "     <string>" + fileName.Split('/').Last() + "</string>\n";
            outXML += "    </void>\n";
            outXML += "        <void property=\"clientPath\">\n";
            outXML += "     <string>" + inf.FullName + "</string>\n";
            outXML += "    </void>\n";
            outXML += "    <void property=\"bytes\">\n";
            outXML += "     <long>" + inf.Length + "</long>\n";
            outXML += "    </void>\n";
            outXML += "    <void property=\"missingBytes\">\n";
            outXML += "     <long>" + inf.Length + "</long>\n";
            outXML += "    </void>\n";
            outXML += "    <void property=\"bits\">\n";
            outXML += "     <int>16</int>\n";
            outXML += "    </void>\n";
            outXML += "    <void property=\"samplingRate\">\n";
            outXML += "     <double>16000</double>\n";
            outXML += "    </void>\n";
            outXML += "    <void property=\"channels\">\n";
            outXML += "     <int>1</int>\n";
            outXML += "    </void>\n";
            outXML += "    <void property=\"fileType\">\n";
            outXML += "     <string>PCM_SIGNED</string>\n";
            outXML += "    </void>\n";
            outXML += "    <void property=\"md5\">\n";
            outXML += "     <long>" + md5Hash + "</long>\n";
            outXML += "    </void>\n";
            outXML += "   </object>\n";
            outXML += "  </void>\n";
            outXML += " </object>\n";
            outXML += "</java>\n";

            return outXML;
        }
    }

    public static class HeadersExtensionMethods
    {
        public static TValue Get<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> instance, TKey key)
        {
            return instance.FirstOrDefault(pair => Object.Equals(pair.Key, key)).Value;
        }
    }
}
