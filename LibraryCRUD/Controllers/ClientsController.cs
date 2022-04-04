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
    public class ClientsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IValidation _validation;
        private readonly IStringTransformer _stringTransformer;

        public ClientsController(AppDbContext context, IValidation iValidation, IStringTransformer iStringTransformer)
        {
            _context = context;
            _validation = iValidation;
            _stringTransformer = iStringTransformer;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            return View(await _context.Client.ToListAsync());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            ClientDetails details = await GetDetails(client);

            return View(details);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details([Bind(" ClientId,BookId")] ClientDetails details)
        {
            var client = await _context.Client.FirstOrDefaultAsync(m => m.Id == details.ClientId);

            if (client == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Book book = await _context.Book.FirstOrDefaultAsync(m => m.Id == details.BookId);

                Client_Book client_Book = new Client_Book();
                client_Book.Client = client;
                client_Book.Book = book;

                _context.Client_Book.Add(client_Book);
                await _context.SaveChangesAsync();

                ModelState.Clear();
            }

            details = await GetDetails(client);

            return View(details);
        }

        private async Task<ClientDetails> GetDetails(Client client)
        {
            ClientDetails details = new ClientDetails();

            List<Client_Book> client_Books = await _context.Client_Book
                .Where(m => m.Client == client)
                .Include(m => m.Client)
                .Include(m => m.Book)
                .ToListAsync();

            List<int> clientBooksIds = client_Books.Select(m => m.Book.Id).ToList();

            List<Book> books = await _context.Book.Where(m => !clientBooksIds.Contains(m.Id)).ToListAsync();

            details.Client = client;
            details.Client_Books = client_Books;
            details.Books = books;

            return details;
        }
        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Client client)
        {
            List<string> recordedNames = await _context.Client
                .Select(m => m.Name)
                .ToListAsync();

            bool duplicateValidationResult = _validation.IsDuplicate(
                _stringTransformer.CleanString(client.Name),
                _stringTransformer.CleanStrings(recordedNames)
                );

            if (duplicateValidationResult)
            {
                ModelState.AddModelError("Name", $"The name {client.Name} is already in use");
            }

            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            Client actualClient = _context.Client.Find(client.Id);

            if (actualClient.Name != client.Name)
            {
                List<string> recordedNames = await _context.Client
                    .Select(m => m.Name)
                    .ToListAsync();

                bool duplicateValidationResult = _validation.IsDuplicate(
                    _stringTransformer.CleanString(client.Name),
                    _stringTransformer.CleanStrings(recordedNames)
                    );

                if (duplicateValidationResult)
                {
                    ModelState.AddModelError("Name", $"The name {client.Name} is already in use");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Client.FindAsync(id);
            _context.Client.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Client.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Remove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Client_Book client_Book = await _context.Client_Book
                .Include(m => m.Book)
                .Include(m => m.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (client_Book == null)
            {
                return NotFound();
            }

            return View(client_Book);
        }

        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirm(int id)
        {
            Client_Book client_Book = await _context.Client_Book
                .Include(m => m.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            int clientId = client_Book.Client.Id;

            _context.Client_Book.Remove(client_Book);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details),new { id = clientId });
        }
    }
}
