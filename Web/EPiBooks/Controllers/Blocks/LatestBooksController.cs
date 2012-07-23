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

        public LatestBooksController(IContentRepository contentRepository) {
            this.contentRepository = contentRepository;
        }

        public ActionResult Index(LatestBooksBlock latestBooksBlock) {
            var latestBooks = Enumerable.Empty<BookPage>();
            var bookRoot = latestBooksBlock.BookRoot;
            if (!PageReference.IsNullOrEmpty(bookRoot)) {
                latestBooks = contentRepository.GetChildren<BookPage>(bookRoot, LanguageSelector.AutoDetect(true), 0, 5).OrderByDescending(x => x.StartPublish);
            }
            return View(new LatestBooksViewModel(latestBooksBlock, latestBooks));
        }
    }
}
