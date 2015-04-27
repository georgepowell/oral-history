using Iveonik.Stemmers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebASRUpload
{
    public class DocumentVector
    {
        EnglishStemmer stemmer = new EnglishStemmer();

        Dictionary<string, int> innerVector { get; set; }

        public IEnumerable<string> Words { get { return innerVector.Keys; } }

        public Segment XmlSegment { get; set; }

        public string Origin { get; private set; }

        public DocumentVector()
        {
            innerVector = new Dictionary<string, int>();
            Origin = "";
        }

        public DocumentVector(string document)
        {
            Origin = document;

            innerVector = new Dictionary<string, int>();
            List<string> words = document.Split(' ').ToList();

            foreach (string word in StopWords.Words)
                words.RemoveAll(w => w == word);

            foreach (string word in words)
                CountPlusOne(stemmer.Stem(word));
        }

        public double Size
        {
            get
            {
                return Math.Sqrt(innerVector.Sum(n => n.Value * n.Value));
            }
        }

        public void CountPlusOne(string word)
        {
            if (!innerVector.ContainsKey(word))
                innerVector.Add(word, 0);

            innerVector[word]++;
        }

        public int Count(string word)
        {
            return innerVector.ContainsKey(word) ? innerVector[word] : 0;
        }

        public double Similarity(DocumentVector v, Func<string, double> weight)
        {
            return -Dot(v, weight) / (Size * v.Size);
        }

        public double Dot(DocumentVector v)
        {
            return Dot(v, word => 1.0);
        }

        public double Dot(DocumentVector v, Func<string, double> weight)
        {
            double sum = 0;
            foreach (var word in innerVector.Keys)
                sum += weight.Invoke(word) * innerVector[word] * v.Count(word);

            return sum;
        }
    }
}
