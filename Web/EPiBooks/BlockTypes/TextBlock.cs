using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace EPiBooks.BlockTypes {
    [ContentType(GUID = "6fe1bf14-2aeb-4954-9fcb-eb989f92d46b",
        DisplayName = "Text block",
        Description = "Simple block that displays text from a editor",
        Order = 2)]
    public class TextBlock : BlockData {
        [CultureSpecific]
        [Display(Name = "Main body",
                 Description = "The main body using the XHTML-editor you can insert for example text, images and tables.",
                 GroupName = SystemTabNames.Content,
                 Order = 1)]
        public virtual XhtmlString MainBody { get; set; }
    }
}