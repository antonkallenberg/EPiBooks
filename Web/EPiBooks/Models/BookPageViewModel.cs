using EPiBooks.PageTypes;

namespace EPiBooks.Models {
    public class BookPageViewModel {

        public BookPageViewModel(BookPage currentPage) {
            CurrentPage = currentPage;
        }

        public BookPage CurrentPage { get; set; }
    }
}