using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt_mtg.Models;
using projekt_mtg.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace projekt_mtg.Controllers
{
    [Authorize]
    public class DeckController : Controller
    {   
        //connects to database
        private readonly ApplicationDbContext _context;

        public DeckController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Deck
        //shows all the decks that belong to the current user
        public async Task<IActionResult> Index()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var decks = await _context.Decks.Where(d => d.UserId == userId).ToListAsync();

            return View(decks);
        }

        // GET: Deck/Details/5
        //shows deck details and all the cards inside specific deck owned by current user
        public async Task<IActionResult> Details(int? id, string searchString)
        {
            //in case no ID was provided
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var deck = await _context.Decks
                .Include(d => d.DeckCards)
                .ThenInclude(dc => dc.Card)
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

            if (deck == null)
            {
                return NotFound();
            }

            //search for cards in deck
            if(!string.IsNullOrWhiteSpace(searchString))
            {
                deck.DeckCards = deck.DeckCards
                    .Where(dc =>
                        dc.Card!.Name.ToUpper().Contains(searchString.ToUpper()) ||
                        (dc.Card.ManaCost ?? "").ToUpper().Contains(searchString.ToUpper()) ||
                        (dc.Card.TypeLine ?? "").ToUpper().Contains(searchString.ToUpper()) ||
                        (dc.Card.Rarity ?? "").ToUpper().Contains(searchString.ToUpper())
                    )
                    .ToList();
            }

            //sorts - puts card chosen as Commander first, then shows the rest in alphabetical order
            deck.DeckCards = deck.DeckCards
                .OrderByDescending(dc => dc.IsCommander)
                .ThenBy(dc => dc.Card!.Name)
                .ToList();

            return View(deck);
        }

        // GET: Deck/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Deck/Create
        //creates new deck (to current user)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Format")] Deck deck)
        {
            if (ModelState.IsValid)
            {

                deck.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

                _context.Add(deck);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(deck);
        }

        // GET: Deck/Edit/5
        //edit-form - current user can edit their own deck of choice
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var deck = await _context.Decks.FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

            if (deck == null)
            {
                return NotFound();
            }
            return View(deck);
        }

        // POST: Deck/Edit/5
        //updates deck info
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Format")] Deck deck)
        {
            //user can only edit their own deck
            if (id != deck.Id)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var currentDeck = await _context.Decks.FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

            if(currentDeck == null)
            {
                return NotFound();
            }

            //updates deck-info, doublechecks if deck was deleted by other user or a process
            if (ModelState.IsValid)
            {
                try
                {
                    currentDeck.Name = deck.Name;
                    currentDeck.Format = deck.Format;

                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeckExists(deck.Id))
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
            return View(deck);
        }

        // GET: Deck/Delete/5
        //delete-page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var deck = await _context.Decks
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

            if (deck == null)
            {
                return NotFound();
            }

            return View(deck);
        }

        // POST: Deck/Delete/5
        //actually deletes collection from database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var deck = await _context.Decks.FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

            if (deck != null)
            {
                _context.Decks.Remove(deck);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //doublechecks if deck actaully exists
        private bool DeckExists(int id)
        {
            return _context.Decks.Any(e => e.Id == id);
        }


        //GET: add card
        //form that adds card to deck
        public async Task<IActionResult> AddCard(int deckId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var deck = await _context.Decks.FirstOrDefaultAsync(d => d.Id == deckId && d.UserId == userId);

            if(deck == null)
            {
                return NotFound();
            }

            ViewBag.DeckId = deckId;
            return View();
        }

        
        //POST: add card
        //actually adds card to deck
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCard(int deckId, string cardName, int quantity, bool IsCommander)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var deck = await _context.Decks.FirstOrDefaultAsync(d => d.Id == deckId && d.UserId == userId);

            if(deck == null)
            {
                return NotFound();
}
            var card = await _context.Cards.FirstOrDefaultAsync(c => c.Name == cardName);

            if(card == null)
            {
                ModelState.AddModelError("", "Card not found");
                ViewBag.DeckId = deckId;
                return View();
            }

            //does card already belong in deck? Yes - update quantity. No - create new row
            var existingCard = await _context.DeckCards
                .FirstOrDefaultAsync(dc => dc.DeckId == deckId && dc.CardId == card.Id);

            if(existingCard != null)
            {
                existingCard.Quantity += quantity;
            }
            else
            {
                var deckCard = new DeckCard
                {
                    DeckId = deckId,
                    CardId = card.Id,
                    Quantity = quantity,

                    //if the format is commander
                    IsCommander = IsCommander
                };

                _context.DeckCards.Add(deckCard);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = deckId });

        }
    }
}
