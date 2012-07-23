using System;
using System.Web.Mvc;
using EPiBooks.PageTypes;
using EPiServer.Web.Mvc;

namespace EPiBooks.Controllers {
    public class BookController : PageController<BookPage> {
        public ActionResult Index(BookPage currentPage) {
            return View(new Models.BookPageViewModel(currentPage));
        }
    }
}