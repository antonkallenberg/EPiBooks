using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.DataAbstraction.PageTypeAvailability;
using EPiServer.DataAnnotations;

namespace EPiBooks.PageTypes {
    [ContentType(GUID = "2d7339bb-7cf9-4060-a472-8cc93151c750",
                 DisplayName = "Search page",
                 GroupName = "Pages",
                 Description = "The sites search page")]
    [AvailablePageTypes(Availability = Availability.None)]
    public class SearchPage : PageData {
        public Dictionary<string, string> GetSites() {
            return new Dictionary<string, string> { { "Sogeti.se", "http://www.sogeti.se" }, { "Sogeti.com", "http://www.sogeti.com" } };
        }
    }
}