using BlazorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp
{
    public class GenreService
    {
        private readonly AppDbContext _context;

        public GenreService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Genre>> GetAll()
        {
            return await _context.Genres.ToListAsync();
        }

        public async Task<Genre?> GetById(int id)
        {
            return await _context.Genres
                .Include(g => g.Books)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task Create(Genre genre)
        {
            Console.WriteLine($"[GenreService] Creating genre: {genre.Name}");
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Genre genre)
        {
            Console.WriteLine($"[GenreService] Updating genre: {genre.Name}");
            
            // Отримуємо існуючий запис з бази даних
            var existingGenre = await _context.Genres.FindAsync(genre.Id);
            if (existingGenre == null)
            {
                throw new Exception($"Жанр з ID {genre.Id} не знайдений");
            }
            
            // Оновлюємо лише потрібні поля
            existingGenre.Name = genre.Name;
            // Інші поля, які потрібно оновити
            
            // Зберігаємо зміни
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            try
            {
                var genre = await _context.Genres.FindAsync(id);
                if (genre != null)
                {
                    _context.Genres.Remove(genre);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                // Перевіряємо, чи помилка пов'язана з порушенням обмеження зовнішнього ключа
                if (ex.InnerException != null && 
                    ex.InnerException.Message.Contains("foreign key constraint"))
                {
                    throw new Exception("Неможливо видалити жанр, оскільки існують книги цього жанру. Спочатку видаліть або змініть книги цього жанру.", ex);
                }
                throw;
            }
        }
           
        public async Task<Genre> GetByName(string name)
        {
            var normalizedName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower());
            return await _context.Genres.FirstOrDefaultAsync(g => g.Name == normalizedName);
        }
    }
}