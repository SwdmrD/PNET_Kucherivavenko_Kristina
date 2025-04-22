using BlazorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp
{
    public class BookService
    {
        private readonly AppDbContext _context;

        public BookService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetAll()
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.Reviews)
                .ToListAsync();
        }

        public async Task<Book?> GetById(int id)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task Create(Book book)
        {
            Console.WriteLine($"[BookService] Creating book: {book.Title}");
            
            // Очищуємо навігаційні властивості перед додаванням
            book.Author = null;
            book.Genre = null;
            
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Book book)
        {
            Console.WriteLine($"[BookService] Updating book: {book.Title}");
            
            // Отримуємо існуючий запис з бази даних
            var existingBook = await _context.Books.FindAsync(book.Id);
            if (existingBook == null)
            {
                throw new Exception($"Книга з ID {book.Id} не знайдена");
            }
            
            // Оновлюємо лише потрібні поля
            existingBook.Title = book.Title;
            existingBook.AuthorId = book.AuthorId;
            existingBook.GenreId = book.GenreId;
            // Інші поля, які потрібно оновити
            
            // Зберігаємо зміни
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book != null)
                {
                    _context.Books.Remove(book);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                // Перевіряємо, чи помилка пов'язана з порушенням обмеження зовнішнього ключа
                if (ex.InnerException != null && 
                    ex.InnerException.Message.Contains("foreign key constraint"))
                {
                    throw new Exception("Неможливо видалити книгу, оскільки існують пов'язані рецензії. Спочатку видаліть рецензії для цієї книги.", ex);
                }
                throw;
            }
        }
        
        public async Task<Book?> GetByTitle(string title)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(b => b.Title.ToLower() == title.ToLower());
        }
    }
}