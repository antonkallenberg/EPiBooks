using System.Web.Mvc;
using EPiBooks.Models;
using EPiBooks.PageTypes;
using EPiServer.Find;
using EPiServer.Web.Mvc;
using FindCrawler;

namespace EPiBooks.Controllers {
    public class SearchController : PageController<SearchPage> {
        private readonly IClient searchClient;

        public SearchController(IClient searchClient) {
            this.searchClient = searchClient;
        }

        public ActionResult Index(SearchPage currentPage) {
            return View(new SearchResultModel(null, currentPage.GetSites()));
        }

        [HttpPost]
        public ActionResult GetHits(SearchPage currentPage, string query, string selectedSite) {
            var result = searchClient.Search<CrawledDocument>().For(query).Filter(x => x.UrlToDocument.PrefixCaseInsensitive(selectedSite)).GetResult();
            return View("Index", new SearchResultModel(result, currentPage.GetSites()) { SelectedSite = selectedSite });
        }
    }
}