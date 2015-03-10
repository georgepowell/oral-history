using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OralHistory.Models
{
    public class Interview
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Transcription { get; set; }
    }
}