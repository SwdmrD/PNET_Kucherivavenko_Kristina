namespace BlazorApp.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using System.Globalization;

    public class Genre
    {
        private string _name = string.Empty;
        
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва жанру обов'язкова.")]
        [MaxLength(100, ErrorMessage = "Назва жанру не може перевищувати 100 символів.")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯіІїЇєЄґҐ'\s\-]*$", 
            ErrorMessage = "Назва жанру може містити лише українські та англійські символи.")]
        public string Name 
        { 
            get => _name; 
            set => _name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower()); 
        }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}