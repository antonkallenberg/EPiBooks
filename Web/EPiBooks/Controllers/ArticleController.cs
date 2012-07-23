using System.Web.Mvc;
using EPiBooks.PageTypes;
using EPiServer.Web.Mvc;

namespace EPiBooks.Controllers {
    public class ArticleController : PageController<ArticlePage> {
        public ActionResult Index(ArticlePage currentPage) {
            return View(currentPage);
        }
    }
}
