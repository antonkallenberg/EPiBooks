using System.Linq;
using System.Web.Mvc;
using EPiBooks.BlockTypes;
using EPiBooks.Models.Blocks;
using EPiBooks.PageTypes;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web;
using EPiServer.Web.Mvc;

namespace EPiBooks.Controllers.Blocks {
    public class LatestBooksController : ActionControllerBase, IRenderTemplate<LatestBooksBlock> {
        private readonly IContentRepository contentRepository;
        private readonly ILanguageSelector languageSelector;

        public LatestBooksController(IContentRepository contentRepository, ILanguageSelector languageSelector)
        {
            this.contentRepository = contentRepository;
            this.languageSelector = languageSelector;
        }

        public ActionResult Index(LatestBooksBlock latestBooksBlock) {
            var latestBooks = Enumerable.Empty<BookPage>();
            var bookRoot = latestBooksBlock.BookRoot;
            if (!PageReference.IsNullOrEmpty(bookRoot)) {
                latestBooks = contentRepository.GetChildren<BookPage>(bookRoot, languageSelector, 0, 5).OrderByDescending(x => x.StartPublish);
            }
            return View(new LatestBooksViewModel(latestBooksBlock, latestBooks));
        }
    }
}
