using EPiServer.Core;
using EPiServer.DataAbstraction.PageTypeAvailability;
using EPiServer.DataAnnotations;

namespace EPiBooks.PageTypes {
    [ContentType(GUID = "a099928d-f81f-40b6-aa7c-60842cb4d3d0",
                 DisplayName = "Folder",
                 GroupName = "Folders",
                 Description = "A folder that contains content like pages or other folders")]
    [AvailablePageTypes(Availability = Availability.All, Include = new[] { typeof(Folder), typeof(BookPage) })]
    public class Folder : PageData {
    }
}