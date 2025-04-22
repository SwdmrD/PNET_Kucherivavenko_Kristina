using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ім'я користувача обов'язкове.")]
        [MaxLength(50, ErrorMessage = "Ім'я користувача не може перевищувати 50 символів.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обов'язковий.")]
        [MinLength(8, ErrorMessage = "Пароль повинен містити щонайменше 8 символів.")]
        [MaxLength(100, ErrorMessage = "Пароль не може перевищувати 100 символів.")]
        public string Password { get; set; } = string.Empty;

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}