using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Razor.Language;

namespace Projekt_mtg.Models
{
    public class DeckCard
    {
        public int Id { get; set; }
        
        //FK till DeckModel
        public int DeckId { get; set; }

        //skapar ett objekt för att bättre kunna visa rätt data senare - DeckId=personnummer, Deck=hela personen
        public Deck? Deck { get; set; }

        //FK till Card
        public int CardId { get; set; }
        public Card? Card { get; set; }

        public int Quantity { get; set; }

        public bool IsCommander { get; set; }

    }
}