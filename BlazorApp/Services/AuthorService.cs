using BlazorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp
{
    public class AuthorService
    {
        private readonly AppDbContext _context;

        public AuthorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Author>> GetAll()
        {
            return await _context.Authors
                .Include(a => a.Books)
                .ToListAsync();
        }

        public async Task<Author?> GetById(int id)
        {
            return await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task Create(Author author)
        {
            Console.WriteLine($"[AuthorService] Creating author: {author.Name}");
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Author author)
        {
            Console.WriteLine($"[AuthorService] Updating author: {author.Name}");
            
            // Отримуємо існуючий запис з бази даних
            var existingAuthor = await _context.Authors.FindAsync(author.Id);
            if (existingAuthor == null)
            {
                throw new Exception($"Автор з ID {author.Id} не знайдений");
            }
            
            // Оновлюємо лише потрібні поля
            existingAuthor.Name = author.Name;
            existingAuthor.Surname = author.Surname;
            // Інші поля, які потрібно оновити
            
            // Зберігаємо зміни
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);
                if (author != null)
                {
                    _context.Authors.Remove(author);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && 
                    ex.InnerException.Message.Contains("foreign key constraint"))
                {
                    throw new Exception("Неможливо видалити автора, оскільки існують пов'язані книги. Спочатку видаліть або змініть книги цього автора.", ex);
                }
                throw;
            }
        }
    }
}