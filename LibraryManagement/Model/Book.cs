using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Model
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public required string Title { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public required string Author { get; set; }
    }
}
