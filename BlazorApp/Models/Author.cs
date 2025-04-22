using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ім'я автора обов'язкове.")]
        [MaxLength(50, ErrorMessage = "Ім'я не може перевищувати 50 символів.")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯіІїЇєЄґҐ'\s\-]*$", 
            ErrorMessage = "Ім'я може містити лише українські та англійські символи.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Прізвище автора обов'язкове.")]
        [MaxLength(50, ErrorMessage = "Прізвище не може перевищувати 50 символів.")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯіІїЇєЄґҐ'\s\-]*$", 
            ErrorMessage = "Прізвище може містити лише українські та англійські символи.")]
        public string Surname { get; set; } = string.Empty;

        private DateTime _birthDate;

        [Required(ErrorMessage = "Дата народження обов'язкова.")]
        public DateTime BirthDate
        {
            get => _birthDate;
            set => _birthDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public ICollection<Book> Books { get; set; } = new List<Book>();

        public bool IsBirthDateValid()
        {
            return BirthDate <= DateTime.Today.AddYears(-18);
        }
    }
}