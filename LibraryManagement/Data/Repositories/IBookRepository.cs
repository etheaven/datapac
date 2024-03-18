using LibraryManagement.Model;

namespace LibraryManagement.Data.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task AddBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);
        Task<Borrowing?> CreateBorrowingAsync(Borrowing borrowing);
        Task<Borrowing> ReturnBookAsync(int borrowingId);
    }
}
