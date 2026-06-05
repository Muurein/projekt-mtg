using System.ComponentModel.DataAnnotations;

namespace Projekt_mtg.Models
{
    public class Deck 
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string UserId { get; set; } = "";

        public DeckFormat Format { get; set; }

        //a card can only exist on one row at a time in a deck
        public ICollection<DeckCard> DeckCards { get; set; } = new List<DeckCard>(); 
    }
}
