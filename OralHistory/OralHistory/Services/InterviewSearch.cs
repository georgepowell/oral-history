using OralHistory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OralHistory.Services
{
    public static class InterviewSearch
    {
        public static IEnumerable<Segment> SearchInterview(Transcription transcription, string query)
        {
            Documents docs = new Documents();
            foreach (var segment in transcription.Segments)
            {
                string sentence = segment.Sentence;
                DocumentVector vec = new DocumentVector(sentence);
                vec.XmlSegment = segment;
                docs.Add(vec);
            }

            return MostSimilar(query, docs, 15);
        }

        static IEnumerable<Segment> MostSimilar(string sentence, Documents docs, int number)
        {
            DocumentVector v = new DocumentVector(sentence);
            List<Tuple<double, DocumentVector>> rtn = new List<Tuple<double, DocumentVector>>();

            foreach (var doc in docs.DocVectors)
            {
                double sim = doc.Similarity(v, word => Math.Log(docs.Count / (1 + docs.DocumentFrequency(word))));

                rtn.Add(new Tuple<double, DocumentVector>(sim, doc));
            }
            var ordered = rtn.Where(n => n.Item1 < -0.1 && !double.IsNaN(n.Item1)).OrderBy(n => n.Item1);
            var taken = ordered.Take(number);
            var toReturn = taken.Select(n => n.Item2.XmlSegment);

            return toReturn;
        }
    }
}