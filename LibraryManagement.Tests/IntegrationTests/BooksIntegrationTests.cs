using LibraryManagement;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using LibraryManagement.Model;

namespace LibraryManagement.Tests.IntegrationTests
{
    public class BooksIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private const string BaseUrl = "/api/books";
        private readonly int existingBookId;
        private readonly int availableBookId;
        private readonly int existingBorrowingId;

        public BooksIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            existingBookId = 1;
            availableBookId = 2;
            existingBorrowingId = 1;
        }

        [Fact]
        public async Task GetBooks_ReturnsSuccessStatusCode()
        {
            var response = await _client.GetAsync(BaseUrl);

            response.EnsureSuccessStatusCode(); 
        }

        [Fact]
        public async Task PostBook_ReturnsCreatedBook()
        {
            var book = new Book { Title = "Harry Potter", Author = "Author" };

            var response = await _client.PostAsJsonAsync(BaseUrl, book);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var returnedBook = await response.Content.ReadFromJsonAsync<Book>();
            Assert.Equal("Harry Potter", returnedBook?.Title);
        }


        [Fact]
        public async Task GetBook_ReturnsBook()
        {
            var bookId = existingBookId;
            var response = await _client.GetAsync($"{BaseUrl}/{bookId}");

            response.EnsureSuccessStatusCode();

            var book = await response.Content.ReadFromJsonAsync<Book>();
            Assert.NotNull(book);
            Assert.Equal(bookId, book.Id);
        }


        [Fact]
        public async Task PostBook_CreatesBook_ReturnsCreated()
        {
            var newBook = new Book { Author = "Simon Soka", Title = "How I said hello to Datapac team" };
            var response = await _client.PostAsJsonAsync(BaseUrl, newBook);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var createdBook = await response.Content.ReadFromJsonAsync<Book>();
            Assert.NotNull(createdBook);
            Assert.Equal(newBook.Title, createdBook.Title);
            Assert.Equal(newBook.Author, createdBook.Author);
        }

        [Fact]
        public async Task PutBook_UpdatesBook_WhenBookExists()
        {
            var bookToUpdate = new Book { Id = existingBookId, Title = "Updated Title", Author = "Updated Author" };
            var response = await _client.PutAsJsonAsync($"{BaseUrl}/{existingBookId}", bookToUpdate);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var getResponse = await _client.GetAsync($"{BaseUrl}/{existingBookId}");
            getResponse.EnsureSuccessStatusCode();
            var updatedBook = await getResponse.Content.ReadFromJsonAsync<Book>();

            Assert.Equal("Updated Title", updatedBook?.Title);
        }

        [Fact]
        public async Task DeleteBook_DeletesBook_WhenBookExists()
        {
            var response = await _client.DeleteAsync($"{BaseUrl}/{existingBookId}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var getResponse = await _client.GetAsync($"{BaseUrl}/{existingBookId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task BorrowBook_CreatesBorrowing_WhenBookIsAvailable()
        {
            var borrowing = new { BookId = availableBookId, BorrowedDate = DateTime.UtcNow };
            var response = await _client.PostAsJsonAsync($"{BaseUrl}/borrow", borrowing);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode); 

            var createdBorrowing = await response.Content.ReadFromJsonAsync<Borrowing>();
            Assert.NotNull(createdBorrowing);
            Assert.Equal(borrowing.BookId, createdBorrowing.BookId);
        }
        [Fact]
        public async Task ReturnBook_UpdatesBorrowing_WhenBorrowingExists()
        {
            var borrowingId = existingBorrowingId; 
            var response = await _client.PostAsync($"{BaseUrl}/return/{borrowingId}", null); 

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var returnedBorrowing = await response.Content.ReadFromJsonAsync<Borrowing>();
            Assert.Equal(returnedBorrowing.BookId, returnedBorrowing.BookId);
        }
    }
}
