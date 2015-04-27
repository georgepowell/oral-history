using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WebASRUpload
{
    public class Interview
    {
        public string id { get; set; }
        public string Title { get; set; }
        public string[] SummaryLines { get; set; }
        public double[] SummaryTimes { get; set; }
        public string ManualTranscription { get; set; }
        public Transcription AutomaticTranscription { get; set; }
        public string SoundcloudID { get; set; }
        public string Interviewer { get; set; }
        public string Interviewee { get; set; }
        public string DateOfInterview { get; set; }

        public DateTime DateAsDateTime
        {
            get
            {
                string[] parts = DateOfInterview.Split('-');
                return new DateTime(int.Parse(parts[2]), int.Parse(parts[1]), int.Parse(parts[0]));
            }
        }

        public string FullSummary
        {
            get
            {
                StringBuilder rtn = new StringBuilder();
                foreach (var line in SummaryLines)
                    rtn.Append(line + " ");
                return rtn.ToString();
            }
        }
    }
}