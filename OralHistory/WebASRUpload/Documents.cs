using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebASRUpload
{
    class Documents
    {
        public List<DocumentVector> DocVectors = new List<DocumentVector>();
        DocumentVector documentFrequencies = new DocumentVector();

        public int Count { get { return DocVectors.Count; } }

        public int DocumentFrequency(string word)
        {
            return documentFrequencies.Count(word);
        }

        public void Add(string doc)
        {
            Add(new DocumentVector(doc));
        }

        public void Add(DocumentVector doc)
        {
            DocVectors.Add(doc);
            foreach (var word in doc.Words)
                documentFrequencies.CountPlusOne(word);
        }
    }
}
