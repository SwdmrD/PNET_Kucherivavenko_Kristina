using BlazorApp.Models; 
using Microsoft.EntityFrameworkCore;  

namespace BlazorApp
{
    public class ReviewService
    {
        private readonly AppDbContext _context;
        
        public ReviewService(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<Review>> GetAll()
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Book)
                .ToListAsync();
        }
        
        public async Task<Review?> GetById(int id)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        
        public async Task Create(Review review)
        {
            Console.WriteLine($"[ReviewService] Creating review for book {review.BookId}");
            
            // Очищуємо навігаційні властивості перед додаванням
            review.User = null;
            review.Book = null;
            
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }
        
        public async Task Update(Review review)
        {
            Console.WriteLine($"[ReviewService] Updating review {review.Id}");
            
            // Отримуємо існуючий запис з бази даних
            var existingReview = await _context.Reviews.FindAsync(review.Id);
            if (existingReview == null)
            {
                throw new Exception($"Рецензія з ID {review.Id} не знайдена");
            }
            
            // Оновлюємо лише потрібні поля
            existingReview.Rating = review.Rating;
            existingReview.Text = review.Text;
            existingReview.UserId = review.UserId;
            existingReview.BookId = review.BookId;
            // Інші поля, які потрібно оновити
            
            // Зберігаємо зміни
            await _context.SaveChangesAsync();
        }
        
        public async Task Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }

          public async Task<Review?> GetByUserAndBook(int userId, int bookId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.BookId == bookId);
        }
    }
}
