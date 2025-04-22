using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва книги обов'язкова.")]
        [MaxLength(200, ErrorMessage = "Назва не може перевищувати 200 символів.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ідентифікатор автора обов'язковий.")]
        public int? AuthorId { get; set; }
        public Author? Author { get; set; }

        [Required(ErrorMessage = "Ідентифікатор жанру обов'язковий.")]
        public int? GenreId { get; set; }
        public Genre? Genre { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}