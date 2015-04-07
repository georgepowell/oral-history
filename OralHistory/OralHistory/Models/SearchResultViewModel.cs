using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OralHistory.Models
{
    public class SearchResultViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public List<HighlightResult> Highlights { get; set; }
    }

    public class HighlightResult
    {
        public string Field { get; set; }
        public string Tab { get; set; }
        public string[] Highlights { get; set; }
    }
}