using BlazorApp.Models;

namespace BlazorApp;

public class LibraryFacade
{
    private readonly AuthorService _authorService;
    private readonly BookService _bookService;
    private readonly GenreService _genreService;
    private readonly ReviewService _reviewService;
    private readonly UserService _userService;

    public LibraryFacade(
        AuthorService authorService,
        BookService bookService,
        GenreService genreService,
        ReviewService reviewService,
        UserService userService)
    {
        _authorService = authorService;
        _bookService = bookService;
        _genreService = genreService;
        _reviewService = reviewService;
        _userService = userService;
    }

    // === Author ===
    public Task<List<Author>> GetAllAuthors() => _authorService.GetAll();
    public Task<Author?> GetAuthorById(int id) => _authorService.GetById(id);
    public Task CreateAuthor(Author author) => _authorService.Create(author);
    public Task UpdateAuthor(Author author) => _authorService.Update(author);
    public Task DeleteAuthor(int id) => _authorService.Delete(id);

    // === Book ===
    public Task<List<Book>> GetAllBooks() => _bookService.GetAll();
    public Task<Book?> GetBookById(int id) => _bookService.GetById(id);
    public Task<Book?> GetBookByTitle(string title) => _bookService.GetByTitle(title);
    public Task CreateBook(Book book) => _bookService.Create(book);
    public Task UpdateBook(Book book) => _bookService.Update(book);
    public Task DeleteBook(int id) => _bookService.Delete(id);

    // === Genre ===
    public Task<List<Genre>> GetAllGenres() => _genreService.GetAll();
    public Task<Genre?> GetGenreById(int id) => _genreService.GetById(id);
    public Task<Genre> GetGenreByName(string name) => _genreService.GetByName(name);
    public Task CreateGenre(Genre genre) => _genreService.Create(genre);
    public Task UpdateGenre(Genre genre) => _genreService.Update(genre);
    public Task DeleteGenre(int id) => _genreService.Delete(id);

    // === Review ===
    public Task<List<Review>> GetAllReviews() => _reviewService.GetAll();
    public Task<Review?> GetReviewById(int id) => _reviewService.GetById(id);
    public Task<Review?> GetReviewByUserAndBook(int userId, int bookId) =>
        _reviewService.GetByUserAndBook(userId, bookId);
    public Task CreateReview(Review review) => _reviewService.Create(review);
    public Task UpdateReview(Review review) => _reviewService.Update(review);
    public Task DeleteReview(int id) => _reviewService.Delete(id);

    // === User ===
    public Task<List<User>> GetAllUsers() => _userService.GetAll();
    public Task<User?> GetUserById(int id) => _userService.GetById(id);
    public Task<User?> GetUserByUserName(string name) => _userService.GetByUserName(name);
    public Task CreateUser(User user) => _userService.Create(user);
    public Task UpdateUser(User user) => _userService.Update(user);
    public Task DeleteUser(int id) => _userService.Delete(id);
}
