using EPiBooks.PageTypes;

namespace EPiBooks.Models {
    public class StartPageViewModel {

        public StartPageViewModel(StartPage currentPage) {
            CurrentPage = currentPage;
        }

        public StartPage CurrentPage { get; set; }
    }
}