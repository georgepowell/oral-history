using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OralHistory.Models
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
    }
}