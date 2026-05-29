using System.ComponentModel.DataAnnotations;

namespace Projekt_mtg.Models
{
    public class Deck 
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string UserId { get; set; } = "";

        public DeckFormat Format { get; set; }

        //ett kort kan vara finnas på en rad i leken
        public ICollection<DeckCard> DeckCards { get; set; } = new List<DeckCard>(); 
    }
}
