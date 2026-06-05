using System.ComponentModel.DataAnnotations;

namespace Projekt_mtg.Models
{
    public class Card
    {   
        //fields are nullable since come cards in the API might not contain all fields of data
        public int Id { get; set; }

        [Required(ErrorMessage = "You must provide a card name")]
        public string? Name{ get; set; }

        public string? ManaCost { get; set; }

        public string? TypeLine { get; set; }

        public string? OracleText { get; set; }

        public string? Rarity { get; set; }

        public string? ImageUri { get; set; }

        

        //one card can exist in many different users' collections
        public ICollection<Collection> Collections { get; set; } = new List<Collection>();

    }
}