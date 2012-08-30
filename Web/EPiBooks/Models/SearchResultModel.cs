using System.Collections.Generic;
using EPiServer.Find;
using FindCrawler;

namespace EPiBooks.Models {
    public class SearchResultModel {
        public SearchResults<CrawledDocument> SearchResults { get; set; }
        public string SelectedSite { get; set; }
        public IDictionary<string, string> Sites { get; set; }

        public SearchResultModel(SearchResults<CrawledDocument> searchResults, IDictionary<string, string> sites) {
            SearchResults = searchResults;
            Sites = sites;
        }
    }
}