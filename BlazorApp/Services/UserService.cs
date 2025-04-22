using BlazorApp.Models; 
using Microsoft.EntityFrameworkCore;  

namespace BlazorApp
{
    public class UserService
    {
        private readonly AppDbContext _context;
        
        public UserService(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<User>> GetAll()
        {
            return await _context.Users
                .Include(u => u.Reviews)
                .ToListAsync();
        }
        
        public async Task<User?> GetById(int id)
        {
            return await _context.Users
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        
        public async Task Create(User user)
        {
            Console.WriteLine($"[UserService] Creating user: {user.UserName}");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        
        public async Task Update(User user)
        {
            Console.WriteLine($"[UserService] Updating user: {user.UserName}");
            
            // Отримуємо існуючий запис з бази даних
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                throw new Exception($"Користувач з ID {user.Id} не знайдений");
            }
            
            // Оновлюємо лише потрібні поля
            existingUser.UserName = user.UserName;
            existingUser.Password = user.Password;
            // Інші поля, які потрібно оновити
            
            // Зберігаємо зміни
            await _context.SaveChangesAsync();
        }
        
        public async Task Delete(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                // Перевіряємо, чи помилка пов'язана з порушенням обмеження зовнішнього ключа
                if (ex.InnerException != null && 
                    ex.InnerException.Message.Contains("foreign key constraint"))
                {
                    throw new Exception("Неможливо видалити користувача, оскільки існують пов'язані рецензії. Спочатку видаліть рецензії цього користувача.", ex);
                }
                throw;
            }
        }

        public async Task<User?> GetByUserName(string name)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == name);
        }
    }
}