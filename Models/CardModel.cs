using System.ComponentModel.DataAnnotations;

namespace Projekt_mtg.Models
{
    public class Card
    {
        //properties - fields in database
        public int Id { get; set; }

        [Required(ErrorMessage = "You must provide a card name")]
        public string? Name{ get; set; }

        public string? ManaCost { get; set; }

        public string? TypeLine { get; set; }

        public string? OracleText { get; set; }

        public string? Rarity { get; set; }

        public string? ImageUri { get; set; }

        

        //Ett kort kan finnas i flera rader i en collection
        public ICollection<Collection> Collections { get; set; } = new List<Collection>();

    }
}