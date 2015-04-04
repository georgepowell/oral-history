using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OralHistory.Models
{
    public class Segment
    {
        public double StartTime { get; set; }
        public double EndTime { get; set; }
        public string SpeakerId { get; set; }
        public string Sentence { get; set; }

        public static Segment FromXml(XmlNode xml)
        {
            Segment rtn = new Segment();
            rtn.SpeakerId = xml.Attributes["speakerid"].Value;
            rtn.StartTime = double.Parse(xml.Attributes["starttime"].Value);
            rtn.EndTime = double.Parse(xml.Attributes["endtime"].Value);

            XmlNodeList words = xml.ChildNodes;

            for (int i = 0; i < words.Count; i++)
            {
                string word = words.Item(i).InnerText;
                if (word.StartsWith("[")) continue;
                rtn.Sentence += word.ToLower() + " ";
            }

            rtn.Sentence = rtn.Sentence ?? "";
            rtn.Sentence = rtn.Sentence.TrimEnd();

            return rtn;
        }
    }
}
