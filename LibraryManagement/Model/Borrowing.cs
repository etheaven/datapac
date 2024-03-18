using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Model
{
    public class Borrowing
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        [Required]
        public Book Book { get; set; }
        [Required]
        public DateTime BorrowedDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsLateReturn { get; set; }
        public bool IsReturned { get; set; }
    }
    public class BorrowingDTO
    {
        public int Id { get; set; }
        public int BookId { get; set; }
    }
}
