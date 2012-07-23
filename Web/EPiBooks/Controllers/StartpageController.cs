using System.Web.Mvc;
using EPiBooks.PageTypes;
using EPiServer.Web.Mvc;

namespace EPiBooks.Controllers {
    public class StartpageController : PageController<StartPage> {
        public ActionResult Index(StartPage currentPage) {
            return View(new Models.StartPageViewModel(currentPage));
        }
    }
}
