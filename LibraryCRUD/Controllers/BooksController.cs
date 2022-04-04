using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryCRUD.Data;
using LibraryCRUD.Models;
using LibraryCRUD.Services.Interfaces;

namespace LibraryCRUD.Controllers
{
    public class BooksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IValidation _validation;
        private readonly IStringTransformer _stringTransformer;

        public BooksController(AppDbContext context, IValidation iValidation, IStringTransformer iStringTransformer)
        {
            _context = context;
            _validation = iValidation;
            _stringTransformer = iStringTransformer;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            return View(await _context.Book.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            BookDetails details = await GetDetails(book);
            return View(details);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details([Bind("BookId,AuthorId")] BookDetails details)
        {
            Book book = await _context.Book.FirstOrDefaultAsync(m => m.Id == details.BookId);

            if(book == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Author author = await _context.Author.FirstOrDefaultAsync(m => m.Id == details.AuthorId);

                Author_Book author_Book = new Author_Book();
                author_Book.Book = book;
                author_Book.Author = author;

                _context.Author_Book.Add(author_Book);
                await _context.SaveChangesAsync();

                ModelState.Clear();
            }

            details = await GetDetails(book);
            return View(details);
        }

        private async Task<BookDetails> GetDetails (Book book)
        {
            BookDetails details = new BookDetails();

            List<Author_Book> book_authors = await _context.Author_Book
                .Where(m => m.Book == book)
                .Include(m => m.Author)
                .ToListAsync();

            List<int> authorsIds = book_authors.
                Select(m => m.Author.Id).
                ToList();

            List<Author> authors = await _context.Author
                .Where(m => !authorsIds.Contains(m.Id))
                .ToListAsync();

            details.Book = book;
            details.Book_Authors = book_authors;
            details.Authors = authors;

            return details;
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Book book)
        {
            List<string> recordedNames = await _context.Book
                .Select(m => m.Name)
                .ToListAsync();

            bool duplicateValidationResult = _validation.IsDuplicate(
                _stringTransformer.CleanString(book.Name),
                _stringTransformer.CleanStrings(recordedNames)
                );

            if (duplicateValidationResult)
            {
                ModelState.AddModelError("Name", $"The name {book.Name} is already in use");
            }

            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            Book actualBook = _context.Book.Find(book.Id);

            if (actualBook.Name != book.Name)
            {
                List<string> recordedNames = await _context.Book
                    .Select(m => m.Name)
                    .ToListAsync();

                bool duplicateValidationResult = _validation.IsDuplicate(
                    _stringTransformer.CleanString(book.Name),
                    _stringTransformer.CleanStrings(recordedNames)
                    );

                if (duplicateValidationResult)
                {
                    ModelState.AddModelError("Name", $"The name {book.Name} is already in use");
                }

            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Book.FindAsync(id);
            _context.Book.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Remove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Author_Book author_Book = await _context.Author_Book
                .Include(m => m.Book)
                .Include(m => m.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (author_Book == null)
            {
                return NotFound();
            }

            return View(author_Book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirmed(int id)
        {
            Author_Book author_Book = await _context.Author_Book
                .Include(m => m.Book)
                .FirstOrDefaultAsync(m => m.Id == id);

            int bookId = author_Book.Book.Id;

            _context.Author_Book.Remove(author_Book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = bookId });
        }
    }
}
