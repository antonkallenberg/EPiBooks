using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace EPiBooks.PageTypes {
    [ContentType(GUID = "7d00a124-6743-453b-b8d0-072d7be09fc1",
                 DisplayName = "Article page",
                 GroupName = "Pages",
                 Description = "A simple article page")]
    public class ArticlePage : PageData {
        [CultureSpecific]
        [Display(Name = "Main body",
                 Description = "The main body will be shown in the main content area of the page",
                 GroupName = SystemTabNames.Content,
                 Order = 1)]
        public virtual XhtmlString MainBody { get; set; }
    }
}