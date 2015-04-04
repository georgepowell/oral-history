using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OralHistory.Models
{
    public class Transcription
    {
        public List<Segment> Segments { get; set; }

        public Transcription()
        {
            Segments = new List<Segment>();
        }

        public static Transcription FromXml(XmlDocument xml)
        {
            Transcription rtn = new Transcription();
            XmlNodeList segments = xml.GetElementsByTagName("segment");

            for (int i = 0; i < segments.Count; i++)
                rtn.Segments.Add(Segment.FromXml(segments.Item(i)));

            return rtn;
        }

        public object FullText
        {
            get
            {
                StringBuilder rtn = new StringBuilder();
                foreach (var seg in Segments)
                    rtn.Append(seg.Sentence + " ");
                return rtn.ToString();
            }
        }
    }
}
