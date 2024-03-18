using LibraryManagement.Model;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookService _bookService;

        public BooksController(BookService bookService)
        {
            _bookService = bookService;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }
        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var addedBook = await _bookService.AddBookAsync(book);

            return CreatedAtAction(nameof(GetBook), new { id = addedBook.Id }, addedBook);
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != book.Id)
            {
                return BadRequest("The ID in the URL does not match the ID in the book data.");
            }

            var existingBook = await _bookService.GetBookByIdAsync(id);
            if (existingBook == null)
            {
                return NotFound($"Book with ID {id} not found.");
            }

            try
            {
                await _bookService.UpdateBookAsync(id, book);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return Ok();
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var bookExists = await _bookService.BookExists(id);
            if (!bookExists)
            {
                return NotFound();
            }

            try
            {
                await _bookService.DeleteBookAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("borrow")]
        public async Task<ActionResult<BorrowingDTO>> BorrowBook([FromBody] BorrowingDTO borrowingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var book = await _bookService.GetBookByIdAsync(borrowingDto.BookId);
            if (book == null)
            {
                return NotFound($"Book with ID {borrowingDto.BookId} not found.");
            }

            try
            {
                var borrowing = new Borrowing
                {
                    BookId = borrowingDto.BookId,
                    BorrowedDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(14),
                };

                var newBorrowing = await _bookService.CreateBorrowingAsync(borrowing);
                if (newBorrowing == null)
                {
                    return BadRequest("Unable to borrow, already occupied");
                }
                var resultDto = new BorrowingDTO
                {
                    Id = newBorrowing.Id,
                    BookId = newBorrowing.BookId,
                };

                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("return/{borrowingId}")]
        public async Task<ActionResult<BorrowingDTO>> ReturnBook(int borrowingId)
        {
            try
            {
                var returnedBorrowing = await _bookService.ReturnBookAsync(borrowingId);
                if (returnedBorrowing == null)
                {
                    return NotFound($"Borrowing with ID {borrowingId} not found.");
                }

                var resultDto = new BorrowingDTO
                {
                    Id = returnedBorrowing.Id,
                    BookId = returnedBorrowing.BookId,
                };

                return Ok(resultDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
