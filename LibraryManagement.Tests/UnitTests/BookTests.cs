using LibraryManagement.Data.Repositories;
using LibraryManagement.Model;
using LibraryManagement.Services;
using Microsoft.VisualBasic;
using Moq;
using System.Net;

namespace LibraryManagement.Tests.UnitTests;

public class BookTests
{
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly BookService _bookService;
    public BookTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _bookService = new BookService(_bookRepositoryMock.Object);
    }

    [Fact]
    public async Task AddBookAsync_AddsBook()
    {
        var book = new Book { Title = "Test Book", Author = "Test Author" };

        await _bookService.AddBookAsync(book);

        _bookRepositoryMock.Verify(repo => repo.AddBookAsync(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBookAsync_UpdatesExistingBook()
    {
        var bookToUpdate = new Book { Id = 1, Title = "Updated Book", Author = "Updated Author" };

        _bookRepositoryMock.Setup(repo => repo.GetBookByIdAsync(It.IsAny<int>())).ReturnsAsync(new Book { Id = 1, Title = "Original Book", Author = "Original Author" });

        await _bookService.UpdateBookAsync(bookToUpdate.Id, bookToUpdate);

        _bookRepositoryMock.Verify(repo => repo.UpdateBookAsync(It.Is<Book>(b => b.Title == "Updated Book" && b.Author == "Updated Author")), Times.Once);
    }

    [Fact]
    public async Task DeleteBookAsync_DeletesBook()
    {
        var bookIdToDelete = 1;

        await _bookService.DeleteBookAsync(bookIdToDelete);

        _bookRepositoryMock.Verify(repo => repo.DeleteBookAsync(It.Is<int>(id => id == bookIdToDelete)), Times.Once);
    }
    [Fact]
    public async Task CreateBorrowingAsync_CreatesBorrowing_ReturnsBorrowing()
    {
        var borrowing = new Borrowing { BookId = 0, BorrowedDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(14) };

        _bookRepositoryMock.Setup(repo => repo.CreateBorrowingAsync(It.IsAny<Borrowing>())).ReturnsAsync(borrowing);

        var result = await _bookService.CreateBorrowingAsync(new Borrowing());

        Assert.Equal(borrowing, result);
    }

    [Fact]
    public async Task ReturnBookAsync_ReturnsBook_UpdatesBorrowing()
    {
        var borrowingId = 1;
        var borrowing = new Borrowing { BookId = 0, BorrowedDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(14), IsReturned = true };
        _bookRepositoryMock.Setup(repo => repo.ReturnBookAsync(It.IsAny<int>())).ReturnsAsync(borrowing);

        var result = await _bookService.ReturnBookAsync(borrowingId);

        Assert.True(result.IsReturned);
        _bookRepositoryMock.Verify(repo => repo.ReturnBookAsync(borrowingId), Times.Once);
    }

    [Fact]
    public async Task ReturnBookAsync_ReturnsUpdatedBorrowing_WhenBookIsReturned()
    {
        // Arrange
        var bookRepositoryMock = new Mock<IBookRepository>();
        var testBorrowingId = 1;
        var testBorrowing = new Borrowing { Id = testBorrowingId, ReturnDate = null };

        bookRepositoryMock.Setup(r => r.ReturnBookAsync(It.IsAny<int>()))
                          .ReturnsAsync(new Borrowing { Id = testBorrowingId, ReturnDate = DateTime.UtcNow });

        var bookService = new BookService(bookRepositoryMock.Object);

        // Act
        var result = await bookService.ReturnBookAsync(testBorrowingId);

        // Assert
        Assert.NotNull(result.ReturnDate);
        bookRepositoryMock.Verify(r => r.ReturnBookAsync(testBorrowingId), Times.Once);
    }
}