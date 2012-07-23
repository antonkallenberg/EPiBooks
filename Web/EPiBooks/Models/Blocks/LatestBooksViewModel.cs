using System.Collections.Generic;
using EPiBooks.BlockTypes;

namespace EPiBooks.Models.Blocks {
    public class LatestBooksViewModel {
        public LatestBooksBlock LatestBooksBlock { get; private set; }
        public IEnumerable<PageTypes.BookPage> LatestBooks { get; set; }

        public LatestBooksViewModel(LatestBooksBlock latestBooksBlock, IEnumerable<PageTypes.BookPage> latestBooks) {
            LatestBooksBlock = latestBooksBlock;
            LatestBooks = latestBooks;
        }
    }
}