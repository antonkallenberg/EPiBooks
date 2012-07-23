using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace EPiBooks.PageTypes {
    [ContentType(GUID = "35b2a97f-f234-46f0-83bd-9cd79cc1eec8",
                 DisplayName = "Book",
                 GroupName = "Pages",
                 Description = "A bound book, audio books and e-books is inheriting from this page")]
    public class BookPage : PageData {
        [CultureSpecific]
        [Display(Name = "Book name",
                 Description = "The name of the book shown at the top of the page",
                 GroupName = SystemTabNames.Content,
                 Order = 1)]
        public virtual string BookName { get; set; }

        [CultureSpecific]
        [Display(Name = "Main body",
                 Description = "The main body will be shown in the main content area of the page",
                 GroupName = SystemTabNames.Content,
                 Order = 2)]
        public virtual XhtmlString MainBody { get; set; }
    }
}