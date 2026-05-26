using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projekt_mtg.Models;
using projekt_mtg.Data;
using Microsoft.AspNetCore.Authorization;

namespace projekt_mtg.Controllers
{
    [Authorize]
    public class CollectionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CollectionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Mtg
        public async Task<IActionResult> Index(string searchString)
        {   
            //get user's id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //creates database-query to see if user has searched for anything and then handles the search query the user might have put in
            var query = _context.Collections
                .Include(c => c.Card)
                .Where(c => c.UserId == userId && (
                    c.OwnedQuantity > 0 ||
                    c.InDeckQuantity > 0
                ));

            if(!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.ToUpper();

                    query = query.Where(c =>
                        (c.Card!.Name ?? "").ToUpper().Contains(searchString) ||
                        (c.Card.ManaCost ?? "").ToUpper().Contains(searchString) ||
                        (c.Card.TypeLine ?? "").ToUpper().Contains(searchString) ||
                        (c.Card.Rarity ?? "").ToUpper().Contains(searchString)
                     );
            }

            var collections = await query.ToListAsync();

            return View(collections); 
        }

        // GET: Mtg/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections
                .FirstOrDefaultAsync(m => m.Id == id);
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        // GET: Mtg/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Mtg/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CardId,UserId,OwnedQuantity,WishlistQuantity,InDeckQuantity")] Collection collection)
        {
            if (ModelState.IsValid)
            {
                _context.Add(collection);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(collection);
        }

        // GET: Mtg/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections.FindAsync(id);
            if (collection == null)
            {
                return NotFound();
            }
            return View(collection);
        }

        // POST: Mtg/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CardId,UserId,OwnedQuantity,WishlistQuantity,InDeckQuantity")] Collection collection)
        {
            if (id != collection.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(collection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollectionExists(collection.Id))
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
            return View(collection);
        }

        // GET: Mtg/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections
                .FirstOrDefaultAsync(m => m.Id == id);
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        // POST: Mtg/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var collection = await _context.Collections.FindAsync(id);
            if (collection != null)
            {
                _context.Collections.Remove(collection);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CollectionExists(int id)
        {
            return _context.Collections.Any(e => e.Id == id);
        }


        //Wishlist
        public async Task<IActionResult> Wishlist(string searchString)
        {
        //get user's id
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //creates database-query to see if user has searched for anything and then handles the search query the user might have put in
        var query = _context.Collections
            .Include(c => c.Card)
            .Where(c => c.UserId == userId && (
                c.WishlistQuantity > 0
            ));

        if(!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.ToUpper();

                query = query.Where(c =>
                    (c.Card!.Name ?? "").ToUpper().Contains(searchString) ||
                    (c.Card.ManaCost ?? "").ToUpper().Contains(searchString) ||
                    (c.Card.TypeLine ?? "").ToUpper().Contains(searchString) ||
                    (c.Card.Rarity ?? "").ToUpper().Contains(searchString)
                );
        }

        var wishlist = await query.ToListAsync();

        return View(wishlist);
}
    }
}
