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
    public class AuthorsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IValidation _validation;
        private readonly IStringTransformer _stringTransformer;

        public AuthorsController(AppDbContext context, IValidation iValidation, IStringTransformer iStringTransformer)
        {
            _context = context;
            _validation = iValidation;
            _stringTransformer = iStringTransformer;
        }

        // GET: Authors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Author.ToListAsync());
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Author
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            AuthorDetails details = await GetDetails(author);

            return View(details);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details([Bind("AuthorId,BookId")] AuthorDetails details)
        {
            Author author = await _context.Author.FirstOrDefaultAsync(m => m.Id == details.AuthorId);

            if(author == null)
            {
                return NotFound();
            }

            if(ModelState.IsValid)
            {
                Book book = await _context.Book.FirstOrDefaultAsync(m => m.Id == details.BookId);

                Author_Book author_Book = new Author_Book();
                author_Book.Author = author;
                author_Book.Book = book;

                _context.Author_Book.Add(author_Book);
                await _context.SaveChangesAsync();

                ModelState.Clear();
            }

            details = await GetDetails(author);

            return View(details);
        }

        private async Task<AuthorDetails> GetDetails (Author author)
        {
            AuthorDetails details = new AuthorDetails();

            List<Author_Book> author_Books = await _context.Author_Book
                .Include(m => m.Author)
                .Include(m => m.Book)
                .Where(m => m.Author == author)
                .ToListAsync();

            List<int> booksIds = author_Books
                .Select(m => m.Book.Id)
                .ToList();

            List<Book> books = await _context.Book
                .Where(m => !booksIds.Contains(m.Id))
                .ToListAsync();

            details.Author = author;
            details.Author_Books = author_Books;
            details.Books = books;

            return details;
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Author author)
        {
            List<string> recordedNames = await _context.Author
                .Select(m => m.Name)
                .ToListAsync();

            bool duplicateValidationResult = _validation.IsDuplicate(
                _stringTransformer.CleanString(author.Name),
                _stringTransformer.CleanStrings(recordedNames)
                );

            if (duplicateValidationResult)
            {
                ModelState.AddModelError("Name", $"The name {author.Name} is already in use");
            }

            if (ModelState.IsValid)
            {
                _context.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(author);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Author.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Author author)
        {
            if (id != author.Id)
            {
                return NotFound();
            }

            Author actualAuthor = _context.Author.Find(author.Id);

            if (actualAuthor.Name != author.Name)
            {
                List<string> recordedNames = await _context.Author
                    .Select(m => m.Name)
                    .ToListAsync();

                bool duplicateValidationResult = _validation.IsDuplicate(
                    _stringTransformer.CleanString(author.Name),
                    _stringTransformer.CleanStrings(recordedNames)
                    );

                if (duplicateValidationResult)
                {
                    ModelState.AddModelError("Name", $"The name {author.Name} is already in use");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(author);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.Id))
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
            return View(author);
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Author
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _context.Author.FindAsync(id);
            _context.Author.Remove(author);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
            return _context.Author.Any(e => e.Id == id);
        }

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

            return View(author_Book);
        }

        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirmed(int id)
        {
            Author_Book author_Book = await _context.Author_Book
                .Include(m => m.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            int authorId = author_Book.Author.Id;

            _context.Author_Book.Remove(author_Book);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id=authorId });
        }

    }
}
