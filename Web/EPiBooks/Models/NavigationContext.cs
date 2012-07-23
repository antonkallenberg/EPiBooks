using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.ServiceLocation;

namespace EPiBooks.Models {
    public class NavigationContext {
        public PageData Item { get; private set; }
        public ContentReference Selected { get; set; }

        public IEnumerable<NavigationContext> GetItems(PageReference parent, ContentReference selected, bool includeParent) {
            if (PageReference.IsNullOrEmpty(parent)) {
                return Enumerable.Empty<NavigationContext>();
            }

            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var firstLevel = new List<PageData>();
            if (includeParent) {
                firstLevel.Add(contentLoader.Get<PageData>(parent));
            }
            firstLevel.AddRange(FilterForVisitor.Filter(new PageDataCollection(contentLoader.GetChildren<PageData>(parent))));
            return firstLevel.Select(x => new NavigationContext { Item = x, Selected = selected });
        }

        public bool IsSelected { get { return Item.PageLink.CompareToIgnoreWorkID(Selected); } }
    }
}