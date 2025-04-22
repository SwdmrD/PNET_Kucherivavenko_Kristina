using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ідентифікатор користувача обов'язковий.")]
        public int? UserId { get; set; }
        public User? User { get; set; } = new User();

        [Required(ErrorMessage = "Ідентифікатор книги обов'язковий.")]
        public int? BookId { get; set; }
        public Book? Book { get; set; } = new Book();

        [Required(ErrorMessage = "Рейтинг обов'язковий.")]
        [Range(1, 5, ErrorMessage = "Рейтинг має бути від 1 до 5.")]
        public int Rating { get; set; }

        private DateTime _createdAt;

        [Required(ErrorMessage = "Дата створення обов'язкова.")]
        public DateTime CreatedAt
        {
            get =>_createdAt;
            set =>_createdAt = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
        [MaxLength(1000, ErrorMessage = "Текст відгуку не може перевищувати 1000 символів.")]
        public string Text { get; set; } = string.Empty;

        public bool IsCreatedAtValid()
        {
            return CreatedAt <= DateTime.Today;
        }
    }
}