using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace EPiBooks.BlockTypes {
    [ContentType(GUID = "9E4C0D85-1139-48EA-B905-67C08BEE3C69",
        DisplayName = "Latest books",
        Description = "Simple block that displays latest books",
        GroupName = "Book listing",
        Order = 1)]
    public class LatestBooksBlock : BlockData {
        [CultureSpecific]
        [Display(
            GroupName = SystemTabNames.Content,
            Name = "Heading",
            Description = "The heading of the block.",
            Order = 1)]
        public virtual string Heading { get; set; }

        [CultureSpecific]
        [Display(
            GroupName = SystemTabNames.Content,
            Name = "Book root",
            Description = "The root where to get new books from",
            Order = 2)]
        public virtual PageReference BookRoot { get; set; }
    }
}