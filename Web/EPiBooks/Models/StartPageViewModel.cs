using EPiBooks.PageTypes;
using EPiServer.Core;

namespace EPiBooks.Models {
    public class StartPageViewModel {

        public StartPageViewModel(PageTypes.StartPage currentPage) {
            CurrentPage = currentPage;
        }

        public StartPage CurrentPage { get; set; }
    }
}