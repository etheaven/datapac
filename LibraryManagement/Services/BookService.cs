using LibraryManagement.Data.Repositories;
using LibraryManagement.Model;

namespace LibraryManagement.Services
{
    public class BookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _bookRepository.GetAllBooksAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _bookRepository.GetBookByIdAsync(id);
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            await _bookRepository.AddBookAsync(book);
            return book;
        }

        public async Task<Book> UpdateBookAsync(int id, Book book)
        {
            await _bookRepository.UpdateBookAsync(book);
            return book;
        }

        public async Task DeleteBookAsync(int id)
        {
            await _bookRepository.DeleteBookAsync(id);
        }

        public async Task<Borrowing?> CreateBorrowingAsync(Borrowing borrowing)
        {
            return await _bookRepository.CreateBorrowingAsync(borrowing);
        }

        public async Task<Borrowing> ReturnBookAsync(int borrowingId)
        {
            var book = await _bookRepository.ReturnBookAsync(borrowingId);
            
            return book;
        }
        public async Task<bool> BookExists(int id)
        {
            return await _bookRepository.GetBookByIdAsync(id) != null;
        }
    }

}
