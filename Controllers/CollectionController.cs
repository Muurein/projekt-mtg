using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt_mtg.Models;
using projekt_mtg.Data;
using Microsoft.AspNetCore.Authorization;

namespace projekt_mtg.Controllers
{
    [Authorize]
    public class CollectionController : Controller
    {   
        //connects to database
        private readonly ApplicationDbContext _context;

        public CollectionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Collection
        //shows the collection, eg all the cards the user owns or has in decks
        public async Task<IActionResult> Index(string searchString)
        {   
            //get user's id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //creates database-query to see if user has searched for anything and then handles the search query the user might have put in
            //and only show the current user's collection
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

        // GET: Collection/Details/5
        public async Task<IActionResult> Details(int? id)
        {   
            //in case no ID was provided
            if (id == null)
            {
                return NotFound();
            }

            //find collection by ID
            var collection = await _context.Collections.FirstOrDefaultAsync(m => m.Id == id);

            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }


        // POST: Collection/Create
        //creates new collection entry
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

        // GET: Collection/Edit/5
        //show the edit form
        public async Task<IActionResult> Edit(int? id)
        {
            //in case no id was provided
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections.FindAsync(id);

            //if the entry happens to not exist
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        // POST: Collection/Edit/5
        //edit quantities
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OwnedQuantity,WishlistQuantity,InDeckQuantity")] Collection collection)
        {   
            //makes sure that the right entry is edited
            if (id != collection.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var currentCollection = await _context.Collections.FindAsync(id);
                        
                    if(currentCollection == null)
                    {   
                        return NotFound();
                    }

                    currentCollection.OwnedQuantity = collection.OwnedQuantity;
                    currentCollection.WishlistQuantity = collection.WishlistQuantity;
                    currentCollection.InDeckQuantity = collection.InDeckQuantity;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    //if collection entry was deleted by other user
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

        // GET: Collection/Delete/5
        //delete-page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections.FirstOrDefaultAsync(m => m.Id == id);

            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        // POST: Collection/Delete/5
        //actually deletes collection from database
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

        //does the collection actually exist?
        private bool CollectionExists(int id)
        {
            return _context.Collections.Any(e => e.Id == id);
        }


        //Wishlist
        //cards the user wants
        public async Task<IActionResult> Wishlist(string searchString)
        {
            //get user's id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //only show current user's cards
            //only show cards on wishlist
            var query = _context.Collections
                .Include(c => c.Card)
                .Where(c => c.UserId == userId && (
                    c.WishlistQuantity > 0
                ));

            //creates database-query to see if user has searched for anything and then handles the search query the user might have put in
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

        public IActionResult TestUser()
        {
            return Content(
                $"Authenticated: {User.Identity?.IsAuthenticated}\n" +
                $"UserId: {User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)}"
            );
        }
    }
}
