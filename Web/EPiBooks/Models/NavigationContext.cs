using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.ServiceLocation;

namespace EPiBooks.Models {
    public class NavigationContext {
        public PageData Item { get; private set; }

        public IEnumerable<NavigationContext> GetItems(PageReference parent) {
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var firstLevel = FilterForVisitor.Filter(new PageDataCollection(contentLoader.GetChildren<PageData>(parent)));
            return firstLevel.Select(x => new NavigationContext { Item = x });
        }

        public bool IsSelected { get { return false; } }
    }
}