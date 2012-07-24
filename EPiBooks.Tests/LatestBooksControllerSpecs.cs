using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EPiBooks.BlockTypes;
using EPiBooks.Controllers.Blocks;
using EPiBooks.Models.Blocks;
using EPiBooks.PageTypes;
using EPiServer;
using EPiServer.Core;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using ItIs = Moq.It;

namespace EPiBooks.Tests {
    [Subject("LatestBooksControllerSpecs")]
    public class LatestBooksControllerSpecs {

        public class heading_is_returned_to_view {

            private static readonly Mock<IContentRepository> ContentRepositoryMock = new Mock<IContentRepository>();
            private static readonly Mock<LatestBooksBlock> LatestBooksBlockMock = new Mock<LatestBooksBlock>();

            private Establish setup = () => {
                LatestBooksBlockMock.SetupProperty(x => x.BookRoot, new PageReference(5));
                LatestBooksBlockMock.SetupProperty(x => x.Heading, "latest books");
                ContentRepositoryMock.Setup(x => x.GetChildren<BookPage>(LatestBooksBlockMock.Object.BookRoot, ItIs.IsAny<ILanguageSelector>(), 0, 5)).Returns(new List<BookPage>());
            };

            private static LatestBooksViewModel viewModel;
            private Because index_is_execuded = () => {
                var latestBooksController = new LatestBooksController(ContentRepositoryMock.Object, new NullLanguageSelector());
                var view = (ViewResult)latestBooksController.Index(LatestBooksBlockMock.Object);
                viewModel = view.Model as LatestBooksViewModel;
            };

            private It title_is_passed_to_view = () => {
                viewModel.LatestBooksBlock.Heading.Equals("latest books", StringComparison.OrdinalIgnoreCase);
                ContentRepositoryMock.Verify(x => x.GetChildren<BookPage>(LatestBooksBlockMock.Object.BookRoot, ItIs.IsAny<ILanguageSelector>(), 0, 5), Times.Once());
            };
        }

        public class empty_book_list_is_retured {
            private static readonly Mock<IContentRepository> ContentRepositoryMock = new Mock<IContentRepository>();
            private static readonly Mock<LatestBooksBlock> LatestBooksBlockMock = new Mock<LatestBooksBlock>();

            private Establish setup = () => {
                LatestBooksBlockMock.SetupProperty(x => x.BookRoot, PageReference.EmptyReference);
            };

            private static LatestBooksViewModel viewModel;
            private Because index_is_execuded_without_a_book_folder_root = () => {
                var latestBooksController = new LatestBooksController(ContentRepositoryMock.Object, new NullLanguageSelector());
                var view = (ViewResult)latestBooksController.Index(LatestBooksBlockMock.Object);
                viewModel = view.Model as LatestBooksViewModel;
            };

            private It latest_books_should_be_empty = () => {
                viewModel.LatestBooks.ShouldBeEmpty();
                ContentRepositoryMock.Verify(x => x.GetChildren<BookPage>(ItIs.IsAny<ContentReference>()), Times.Never());
            };
        }
    }
}