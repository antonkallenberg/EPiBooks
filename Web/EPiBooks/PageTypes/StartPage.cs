using System.ComponentModel.DataAnnotations;
using EPiBooks.BlockTypes;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAbstraction.PageTypeAvailability;
using EPiServer.DataAnnotations;

namespace EPiBooks.PageTypes {
    [ContentType(GUID = "7E94EB59-1F75-4502-B3A9-ADB6BD671CFF",
                 DisplayName = "Start page",
                 GroupName = "Pages",
                 Description = "The start page is the main entrance to the web site containing a main and secondary body as well as different listings for news and events.")]
    [ImageUrl("")]
    [AvailablePageTypes(Availability = Availability.All, Exclude = new[] { typeof(StartPage) })]
    public class StartPage : PageData {
        [CultureSpecific]
        [Display(Name = "Header",
                 Description = "The main header shown at the top of the page",
                 GroupName = SystemTabNames.Content,
                 Order = 1)]
        public virtual string Header { get; set; }

        [CultureSpecific]
        [Display(Name = "Main body",
                 Description = "The main body will be shown in the main content area of the page, using the XHTML-editor you can insert for example text, images and tables.",
                 GroupName = SystemTabNames.Content,
                 Order = 2)]
        public virtual XhtmlString MainBody { get; set; }

        [Display(
            Name = "Latest books",
            Description = "A block that lists latest books.",
            GroupName = SystemTabNames.Content,
            Order = 3)]
        public virtual LatestBooksBlock LatestBooks { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Content area",
            Description = "A content for adding any shared block",
            GroupName = SystemTabNames.Content,
            Order = 4)]
        public virtual ContentArea ContentArea { get; set; }
    }
}