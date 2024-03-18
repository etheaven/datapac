using LibraryManagement.Model;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task AddBookAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBookAsync(Book updatedBook)
        {
            var existingBook = await _context.Books.FirstOrDefaultAsync(b => b.Id == updatedBook.Id);

            if (existingBook == null)
            {
                throw new KeyNotFoundException($"Book with ID {updatedBook.Id} not found.");
            }

            existingBook.Title = updatedBook.Title;
            existingBook.Author = updatedBook.Author;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Borrowing?> CreateBorrowingAsync(Borrowing borrowing)
        {
            var existingBorrowing = await _context.Borrowings
                .Where(b => b.BookId == borrowing.BookId && b.ReturnDate == null)
                .FirstOrDefaultAsync();

            if (existingBorrowing != null)
            {
                // already borrowed
                return null;
            }

            borrowing.BorrowedDate = DateTime.UtcNow;
            borrowing.DueDate = borrowing.BorrowedDate.AddDays(14);
            _context.Borrowings.Add(borrowing);
            await _context.SaveChangesAsync();
            return borrowing;
        }

        public async Task<Borrowing> ReturnBookAsync(int borrowingId)
        {
            var borrowing = await _context.Borrowings.FindAsync(borrowingId);
            if (borrowing == null)
            {
                throw new KeyNotFoundException("Borrowing not found");
            }
            if (borrowing.IsReturned)
            {
                throw new InvalidOperationException("The book is already returned");
            }
            borrowing.IsReturned = true;
            borrowing.ReturnDate = DateTime.UtcNow;
            borrowing.IsLateReturn = borrowing.ReturnDate > borrowing.DueDate;
            await _context.SaveChangesAsync();
            return borrowing;
        }
    }

}
