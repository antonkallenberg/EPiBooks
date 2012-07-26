using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web.Mvc;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System;

namespace EPiBooks.Controllers.Blocks {
    [RenderDescriptor(Inherited = true, Tags = new[] { TagConstants.Preview })]
    public class ContentDataRendererController : ActionControllerBase, IRenderTemplate<IContentData> {
        private readonly IContentRepository contentRepository;

        public ContentDataRendererController(IContentRepository contentRepository) {
            this.contentRepository = contentRepository;
        }

        public ActionResult Index() {
            var contentLink = ControllerContext.RequestContext.GetContentLink();
            var languageSelector = String.IsNullOrEmpty(ControllerContext.RequestContext.GetLang()) ? LanguageSelector.MasterLanguage() : new LanguageSelector(ControllerContext.RequestContext.GetLang());
            var content = contentRepository.Get<IContent>(contentLink, languageSelector);
            
            return View(content);
        }
    }
}
