using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly ApplicationDbContext _context;

        public DeckController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Deck
        public async Task<IActionResult> Index()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var decks = await _context.Decks
                .Where(d => d.UserId == userId)
                .ToListAsync();

            return View(decks);
        }

        // GET: Deck/Details/5
        public async Task<IActionResult> Details(int? id, string searchString)
        {
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

            //puts card chosen as Commander first
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deck = await _context.Decks.FindAsync(id);
            if (deck == null)
            {
                return NotFound();
            }
            return View(deck);
        }

        // POST: Deck/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,UserId,Format")] Deck deck)
        {
            if (id != deck.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deck);
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deck = await _context.Decks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deck == null)
            {
                return NotFound();
            }

            return View(deck);
        }

        // POST: Deck/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deck = await _context.Decks.FindAsync(id);
            if (deck != null)
            {
                _context.Decks.Remove(deck);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeckExists(int id)
        {
            return _context.Decks.Any(e => e.Id == id);
        }


        //GET: add card
        public IActionResult AddCard(int deckId)
        {
            ViewBag.DeckId = deckId;
            return View();
        }

        
        //POST: add card
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCard(int deckId, string cardName, int quantity, bool IsCommander)
        {
            // ViewBag.DeckId = deckid;
            // return View() ;
            var card = await _context.Cards
                .FirstOrDefaultAsync(c => c.Name == cardName);

            if(card == null)
            {
                ModelState.AddModelError("", "Card not found");
                ViewBag.DeckId = deckId;
                return View();
            }

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
                    IsCommander = IsCommander
                };

                _context.DeckCards.Add(deckCard);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = deckId });

        }
    }
}
